import { Component, inject, OnInit, PLATFORM_ID, signal } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { ContractsService, Contract } from '../../services/contracts.service';
import { Client } from '../../services/clients.service';
import { ToastService } from '../../services/toast.service';
import { NavigationComponent } from '../../components/navigation/navigation';

@Component({
  selector: 'app-contract-detail',
  imports: [NavigationComponent, RouterLink],
  templateUrl: './contract-detail.html',
  styleUrl: './contract-detail.scss'
})
export class ContractDetail implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private contractsService = inject(ContractsService);
  private platformId = inject(PLATFORM_ID);
  private toastService = inject(ToastService);

  contract = signal<Contract | null>(null);
  isLoading = signal(true);

  ngOnInit(): void {
    if (isPlatformBrowser(this.platformId)) {
      const id = this.route.snapshot.paramMap.get('id');
      if (id) {
        this.fetchContract(id);
      } else {
        this.toastService.error('Invalid contract ID.');
        this.router.navigate(['/contracts']);
      }
    }
  }

  fetchContract(id: string): void {
    this.contractsService.getContract(id).subscribe({
      next: (data) => {
        this.contract.set(data);
        this.isLoading.set(false);
      },
      error: (err) => {
        console.error('[ContractDetail] Error loading contract:', err);
        this.isLoading.set(false);
        const message = err?.error?.message || err?.error?.Message || 'Failed to load contract details.';
        this.toastService.error(message);
        this.router.navigate(['/contracts']);
      }
    });
  }

  deleteContract(): void {
    const currentContract = this.contract();
    if (!currentContract) return;

    if (confirm('Are you sure you want to delete this contract?')) {
      this.contractsService.deleteContract(currentContract.id).subscribe({
        next: () => {
          this.toastService.success('Contract deleted successfully.');
          this.router.navigate(['/contracts']);
        },
        error: (err) => {
          console.error('[ContractDetail] Error deleting contract:', err);
          const message = err?.error?.message || err?.error?.Message || 'Failed to delete contract.';
          this.toastService.error(message);
        }
      });
    }
  }

  getOtherParticipants(c: Contract): Client[] {
    if (!c.participants) return [];
    return c.participants.filter(p => p.id !== c.clientId && p.id !== c.contractManagerId);
  }
}
