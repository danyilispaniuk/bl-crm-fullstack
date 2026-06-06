import { Component, inject, OnInit, PLATFORM_ID, signal } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { AdvisorsService, Advisor } from '../../services/advisors.service';
import { ContractsService, Contract } from '../../services/contracts.service';
import { ToastService } from '../../services/toast.service';
import { NavigationComponent } from '../../components/navigation/navigation';
import { ContractCardComponent } from '../../components/contract-card/contract-card';

@Component({
  selector: 'app-advisor-detail',
  imports: [NavigationComponent, ContractCardComponent, RouterLink],
  templateUrl: './advisor-detail.html',
  styleUrl: './advisor-detail.scss'
})
export class AdvisorDetail implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private advisorsService = inject(AdvisorsService);
  private contractsService = inject(ContractsService);
  private platformId = inject(PLATFORM_ID);
  private toastService = inject(ToastService);

  advisor = signal<Advisor | null>(null);
  contracts = signal<Contract[]>([]);
  isLoadingAdvisor = signal(true);
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
        this.fetchAdvisor(id);
        this.fetchContracts(id);
      } else {
        this.toastService.error('Invalid advisor ID.');
        this.router.navigate(['/advisors']);
      }
    }
  }

  fetchAdvisor(id: string): void {
    this.advisorsService.getAdvisor(id).subscribe({
      next: (data) => {
        this.advisor.set(data);
        this.isLoadingAdvisor.set(false);
      },
      error: (err) => {
        console.error('[AdvisorDetail] Error loading advisor:', err);
        this.isLoadingAdvisor.set(false);
        const message = err?.error?.message || err?.error?.Message || 'Failed to load advisor details.';
        this.toastService.error(message);
        this.router.navigate(['/advisors']);
      }
    });
  }

  fetchContracts(advisorId: string): void {
    this.contractsService.getContracts().subscribe({
      next: (data) => {
        const advisorContracts = data.filter(c => c.contractManagerId === advisorId);
        this.contracts.set(advisorContracts);
        this.isLoadingContracts.set(false);
      },
      error: (err) => {
        console.error('[AdvisorDetail] Error loading contracts:', err);
        this.isLoadingContracts.set(false);
        this.toastService.error('Failed to load advisor contracts.');
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
          console.error('[AdvisorDetail] Error deleting contract:', err);
          const message = err?.error?.message || err?.error?.Message || 'Failed to delete contract.';
          this.toastService.error(message);
        }
      });
    }
  }
}
