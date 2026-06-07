import { Component, inject, OnInit, PLATFORM_ID, signal, computed } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
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
  private router = inject(Router);

  isAdvisor = signal(false);
  contracts = signal<Contract[]>([]);
  managedContracts = signal<Contract[]>([]);
  participatingContracts = signal<Contract[]>([]);
  searchQuery = signal('');
  isLoading = signal(true);
  hiddenContractIds = signal<Set<string>>(new Set());

  private filterList(list: Contract[], query: string): Contract[] {
    const hiddenSet = this.hiddenContractIds();
    const visibleList = list.filter(c => !hiddenSet.has(c.id));
    if (!query) return visibleList;
    return visibleList.filter(contract => {
      const regNum = (contract.registrationNumber || '').toLowerCase();
      const inst = (contract.institution || '').toLowerCase();
      const client = (contract.clientName || '').toLowerCase();
      const manager = (contract.contractManagerName || '').toLowerCase();

      return regNum.includes(query) ||
        inst.includes(query) ||
        client.includes(query) ||
        manager.includes(query);
    });
  }

  hideContract(id: string): void {
    const hiddenSet = new Set(this.hiddenContractIds());
    hiddenSet.add(id);
    this.hiddenContractIds.set(hiddenSet);
    localStorage.setItem('hiddenContractIds', JSON.stringify(Array.from(hiddenSet)));
    this.toastService.success('Contract hidden.');
  }

  showAllHidden(): void {
    this.hiddenContractIds.set(new Set());
    localStorage.removeItem('hiddenContractIds');
    this.toastService.success('All hidden contracts are visible now.');
  }

  filteredAllContracts = computed(() => {
    const query = this.searchQuery().toLowerCase().trim();
    return this.filterList(this.contracts(), query);
  });

  filteredManagedContracts = computed(() => {
    const query = this.searchQuery().toLowerCase().trim();
    return this.filterList(this.managedContracts(), query);
  });

  filteredParticipatingContracts = computed(() => {
    const query = this.searchQuery().toLowerCase().trim();
    return this.filterList(this.participatingContracts(), query);
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
          this.managedContracts.update(list => list.filter(c => c.id !== id));
          this.participatingContracts.update(list => list.filter(c => c.id !== id));
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
      const hidden = localStorage.getItem('hiddenContractIds');
      if (hidden) {
        try {
          this.hiddenContractIds.set(new Set(JSON.parse(hidden)));
        } catch (e) {
          console.error('Failed to parse hidden contract IDs', e);
        }
      }

      const role = localStorage.getItem('role');
      let userId = localStorage.getItem('userId');

      if (!userId && role === 'Advisor') {
        const token = localStorage.getItem('token');
        if (token) {
          try {
            const payloadBase64 = token.split('.')[1];
            const payloadJson = window.atob(payloadBase64);
            const payload = JSON.parse(payloadJson);
            userId = payload.nameid || payload.sub || null;
            if (userId) {
              localStorage.setItem('userId', userId);
            }
          } catch (e) {
            console.error('Failed to parse JWT token', e);
          }
        }
      }

      if (role === 'Admin') {
        this.router.navigate(['/admin/contracts']);
        return;
      }

      this.isAdvisor.set(true);
      if (userId) {
        this.contractsService.getAdvisorContracts(userId).subscribe({
          next: (data) => {
            this.managedContracts.set(data.managedContracts || []);
            this.participatingContracts.set(data.participatingContracts || []);
            this.isLoading.set(false);
          },
          error: (err) => {
            console.error('[Contracts] API error:', err);
            this.isLoading.set(false);
            const message =
              err?.error?.message ||
              err?.error?.Message ||
              `Failed to load advisor contracts (HTTP ${err?.status ?? 'unknown'})`;
            this.toastService.error(message);
          }
        });
      } else {
        this.isLoading.set(false);
      }
    }
  }
}
