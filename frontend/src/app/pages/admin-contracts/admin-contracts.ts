import { Component, inject, OnInit, PLATFORM_ID, signal, computed } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { ContractsService, Contract } from '../../services/contracts.service';
import { ToastService } from '../../services/toast.service';
import { NavigationComponent } from '../../components/navigation/navigation';
import { ContractCardComponent } from '../../components/contract-card/contract-card';

@Component({
  selector: 'app-admin-contracts',
  imports: [NavigationComponent, ContractCardComponent, RouterLink],
  templateUrl: './admin-contracts.html',
  styleUrl: './admin-contracts.scss'
})
export class AdminContracts implements OnInit {
  private contractsService = inject(ContractsService);
  private platformId = inject(PLATFORM_ID);
  private toastService = inject(ToastService);
  private router = inject(Router);

  contracts = signal<Contract[]>([]);
  searchQuery = signal('');
  isLoading = signal(true);

  exportCsv(): void {
    this.contractsService.exportContractsCsv().subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = `contracts_${new Date().toISOString().slice(0, 10)}.csv`;
        document.body.appendChild(a);
        a.click();
        document.body.removeChild(a);
        window.URL.revokeObjectURL(url);
        this.toastService.success('CSV file exported successfully.');
      },
      error: (err) => {
        console.error('[AdminContracts] Export error:', err);
        const message = err?.error?.message || err?.error?.Message || 'Failed to export CSV.';
        this.toastService.error(message);
      }
    });
  }

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
          console.error('[AdminContracts] Delete error:', err);
          const message =
            err?.error?.message ||
            err?.error?.Message ||
            `Failed to delete contract (HTTP ${err?.status ?? 'unknown'})`;
          this.toastService.error(message);
        }
      });
    }
  }

  ngOnInit(): void {
    if (isPlatformBrowser(this.platformId)) {
      const role = localStorage.getItem('role');
      if (role === 'Advisor') {
        this.router.navigate(['/contracts']);
        return;
      }

      this.contractsService.getContracts().subscribe({
        next: (data) => {
          this.contracts.set(data);
          this.isLoading.set(false);
        },
        error: (err) => {
          console.error('[AdminContracts] API error:', err);
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
