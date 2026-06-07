import { Component, inject, OnInit, PLATFORM_ID, signal, computed } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { RouterLink } from '@angular/router';
import { ContractsService, Contract } from '../../services/contracts.service';
import { ToastService } from '../../services/toast.service';
import { NavigationComponent } from '../../components/navigation/navigation';
import { ContractCardComponent } from '../../components/contract-card/contract-card';

@Component({
  selector: 'app-contracts',
  imports: [NavigationComponent, ContractCardComponent, RouterLink],
  templateUrl: './contracts.html',
  styleUrl: './contracts.scss'
})
export class Contracts implements OnInit {
  private contractsService = inject(ContractsService);
  private platformId = inject(PLATFORM_ID);
  private toastService = inject(ToastService);

  contracts = signal<Contract[]>([]);
  searchQuery = signal('');
  isLoading = signal(true);

  filteredContracts = computed(() => {
    const query = this.searchQuery().toLowerCase().trim();
    const allContracts = this.contracts();
    if (!query) {
      return allContracts;
    }
    return allContracts.filter(contract => {
      const regNum = (contract.registrationNumber || '').toLowerCase();
      const inst = (contract.institution || '').toLowerCase();
      const client = (contract.clientName || '').toLowerCase();
      const manager = (contract.contractManagerName || '').toLowerCase();

      return regNum.includes(query) ||
        inst.includes(query) ||
        client.includes(query) ||
        manager.includes(query);
    });
  });

  onSearchChange(event: Event): void {
    const target = event.target as HTMLInputElement;
    this.searchQuery.set(target.value);
  }

  deleteContract(id: string): void {
    if (confirm('Are you sure you want to delete this contract?')) {
      this.contractsService.deleteContract(id).subscribe({
        next: () => {
          this.toastService.success('Contract deleted successfully.');
          this.contracts.update(list => list.filter(c => c.id !== id));
        },
        error: (err) => {
          console.error('[Contracts] Delete error:', err);
          const message =
            err?.error?.message ||
            err?.error?.Message ||
            `Failed to delete contract (HTTP ${err?.status ?? 'unknown'})`;
          this.toastService.error(message);
        }
      });
    }
  }

  isContractActive(validityDateStr: string, endDateStr?: string): boolean {
    const today = new Date();
    today.setHours(0, 0, 0, 0);

    if (endDateStr) {
      const endDate = new Date(endDateStr);
      if (endDate < today) return false;
    }

    const validityDate = new Date(validityDateStr);
    return validityDate >= today;
  }

  ngOnInit(): void {
    if (isPlatformBrowser(this.platformId)) {
      this.contractsService.getContracts().subscribe({
        next: (data) => {
          console.log('[Contracts] API response count:', data.length, data);
          this.contracts.set(data);
          this.isLoading.set(false);
        },
        error: (err) => {
          console.error('[Contracts] API error:', err);
          this.isLoading.set(false);
          const message =
            err?.error?.message ||
            err?.error?.Message ||
            `Failed to load contracts (HTTP ${err?.status ?? 'unknown'})`;
          this.toastService.error(message);
        }
      });
    }
  }
}
