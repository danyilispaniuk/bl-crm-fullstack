import { Component, inject, OnInit, PLATFORM_ID, signal } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { ClientsService, Client } from '../../services/clients.service';
import { ContractsService, Contract } from '../../services/contracts.service';
import { ToastService } from '../../services/toast.service';
import { NavigationComponent } from '../../components/navigation/navigation';
import { ContractCardComponent } from '../../components/contract-card/contract-card';

@Component({
  selector: 'app-client-detail',
  imports: [NavigationComponent, ContractCardComponent, RouterLink],
  templateUrl: './client-detail.html',
  styleUrl: './client-detail.scss'
})
export class ClientDetail implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private clientsService = inject(ClientsService);
  private contractsService = inject(ContractsService);
  private platformId = inject(PLATFORM_ID);
  private toastService = inject(ToastService);

  client = signal<Client | null>(null);
  contracts = signal<Contract[]>([]);
  isLoadingClient = signal(true);
  isLoadingContracts = signal(true);

  calculateAge(birthDateStr?: string): number | null {
    if (!birthDateStr) return null;
    const birthDate = new Date(birthDateStr);
    const today = new Date();
    let age = today.getFullYear() - birthDate.getFullYear();
    const m = today.getMonth() - birthDate.getMonth();
    if (m < 0 || (m === 0 && today.getDate() < birthDate.getDate())) {
      age--;
    }
    return age;
  }

  ngOnInit(): void {
    if (isPlatformBrowser(this.platformId)) {
      const id = this.route.snapshot.paramMap.get('id');
      if (id) {
        this.fetchClient(id);
        this.fetchContracts(id);
      } else {
        this.toastService.error('Invalid client ID.');
        this.router.navigate(['/clients']);
      }
    }
  }

  fetchClient(id: string): void {
    this.clientsService.getClient(id).subscribe({
      next: (data) => {
        this.client.set(data);
        this.isLoadingClient.set(false);
      },
      error: (err) => {
        console.error('[ClientDetail] Error loading client:', err);
        this.isLoadingClient.set(false);
        const message = err?.error?.message || err?.error?.Message || 'Failed to load client details.';
        this.toastService.error(message);
        this.router.navigate(['/clients']);
      }
    });
  }

  fetchContracts(clientId: string): void {
    this.contractsService.getContracts().subscribe({
      next: (data) => {
        const clientContracts = data.filter(c => c.clientId === clientId);
        this.contracts.set(clientContracts);
        this.isLoadingContracts.set(false);
      },
      error: (err) => {
        console.error('[ClientDetail] Error loading contracts:', err);
        this.isLoadingContracts.set(false);
        this.toastService.error('Failed to load client contracts.');
      }
    });
  }

  deleteContract(id: string): void {
    if (confirm('Are you sure you want to delete this contract?')) {
      this.contractsService.deleteContract(id).subscribe({
        next: () => {
          this.toastService.success('Contract deleted successfully.');
          this.contracts.update(list => list.filter(c => c.id !== id));
        },
        error: (err) => {
          console.error('[ClientDetail] Error deleting contract:', err);
          const message = err?.error?.message || err?.error?.Message || 'Failed to delete contract.';
          this.toastService.error(message);
        }
      });
    }
  }
}
