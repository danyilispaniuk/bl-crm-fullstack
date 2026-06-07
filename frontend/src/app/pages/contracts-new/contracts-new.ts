import { Component, inject, OnInit, PLATFORM_ID, signal, computed } from '@angular/core';
import { isPlatformBrowser, Location } from '@angular/common';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { ContractsService } from '../../services/contracts.service';
import { ClientsService } from '../../services/clients.service';
import { AdvisorsService } from '../../services/advisors.service';
import { ToastService } from '../../services/toast.service';
import { NavigationComponent } from '../../components/navigation/navigation';

@Component({
  selector: 'app-contracts-new',
  imports: [NavigationComponent, FormsModule],
  templateUrl: './contracts-new.html',
  styleUrl: './contracts-new.scss'
})
export class ContractsNew implements OnInit {
  private contractsService = inject(ContractsService);
  private clientsService = inject(ClientsService);
  private advisorsService = inject(AdvisorsService);
  private platformId = inject(PLATFORM_ID);
  private toastService = inject(ToastService);
  private router = inject(Router);
  private location = inject(Location);

  // Form signals
  registrationNumber = signal('');
  institution = signal('');
  startDate = signal('');
  validityDate = signal('');
  endDate = signal('');
  clientId = signal('');
  contractManagerId = signal('');
  selectedParticipantIds = signal<string[]>([]);
  showValidationErrors = signal(false);

  // Input text models
  clientInputText = signal('');
  advisorInputText = signal('');
  participantInputText = signal('');

  // Search query signals for lookups
  clientSearchQuery = signal('');
  advisorSearchQuery = signal('');
  participantSearchQuery = signal('');

  // Dropdown open states
  isClientDropdownOpen = signal(false);
  isAdvisorDropdownOpen = signal(false);
  isParticipantDropdownOpen = signal(false);

  // Lookup signals
  clientsLookup = signal<{ id: string; fullName: string }[]>([]);
  advisorsLookup = signal<{ id: string; fullName: string }[]>([]);

  // Computed filtered lists
  filteredClientsLookup = computed(() => {
    const query = this.clientSearchQuery().toLowerCase().trim();
    const lookups = this.clientsLookup();
    if (!query) return lookups;
    return lookups.filter(c => c.fullName.toLowerCase().includes(query));
  });

  filteredAdvisorsLookup = computed(() => {
    const query = this.advisorSearchQuery().toLowerCase().trim();
    const lookups = this.advisorsLookup();
    if (!query) return lookups;
    return lookups.filter(a => a.fullName.toLowerCase().includes(query));
  });

  filteredParticipantsLookup = computed(() => {
    const query = this.participantSearchQuery().toLowerCase().trim();
    const advisors = this.advisorsLookup();
    const managerId = this.contractManagerId();
    const selectedIds = this.selectedParticipantIds();

    // Filter out primary manager and already selected participants
    let available = advisors.filter(a => a.id !== managerId && !selectedIds.includes(a.id));
    if (!query) return available;
    return available.filter(a => a.fullName.toLowerCase().includes(query));
  });

  // Computed list of selected participants objects
  selectedParticipantsList = computed(() => {
    const ids = this.selectedParticipantIds();
    return this.advisorsLookup().filter(a => ids.includes(a.id));
  });

  // Computed names
  selectedClientName = computed(() => {
    const selectedId = this.clientId();
    const match = this.clientsLookup().find(c => c.id === selectedId);
    return match ? match.fullName : '';
  });

  selectedManagerName = computed(() => {
    const selectedId = this.contractManagerId();
    const match = this.advisorsLookup().find(a => a.id === selectedId);
    return match ? match.fullName : '';
  });

  openClientDropdown(): void {
    this.isClientDropdownOpen.set(true);
    this.isAdvisorDropdownOpen.set(false);
    this.isParticipantDropdownOpen.set(false);
    this.clientSearchQuery.set('');
  }

  openAdvisorDropdown(): void {
    this.isAdvisorDropdownOpen.set(true);
    this.isClientDropdownOpen.set(false);
    this.isParticipantDropdownOpen.set(false);
    this.advisorSearchQuery.set('');
  }

  onClientInput(val: string): void {
    this.clientInputText.set(val);
    this.clientSearchQuery.set(val);
    this.isClientDropdownOpen.set(true);
  }

  onAdvisorInput(val: string): void {
    this.advisorInputText.set(val);
    this.advisorSearchQuery.set(val);
    this.isAdvisorDropdownOpen.set(true);
  }

  onParticipantInput(val: string): void {
    this.participantInputText.set(val);
    this.participantSearchQuery.set(val);
    this.isParticipantDropdownOpen.set(true);
  }

  selectClient(id: string): void {
    this.clientId.set(id);
    const match = this.clientsLookup().find(c => c.id === id);
    this.clientInputText.set(match ? match.fullName : '');
    this.isClientDropdownOpen.set(false);
  }

  selectManager(id: string): void {
    this.contractManagerId.set(id);
    const match = this.advisorsLookup().find(a => a.id === id);
    this.advisorInputText.set(match ? match.fullName : '');
    this.isAdvisorDropdownOpen.set(false);
    this.selectedParticipantIds.update(ids => ids.filter(x => x !== id));
  }

  addParticipant(id: string): void {
    this.selectedParticipantIds.update(ids => [...ids, id]);
    this.participantInputText.set('');
    this.participantSearchQuery.set('');
    this.isParticipantDropdownOpen.set(false);
  }

  removeParticipant(id: string): void {
    this.selectedParticipantIds.update(ids => ids.filter(x => x !== id));
  }

  closeDropdownsAndRestore(): void {
    this.isClientDropdownOpen.set(false);
    this.isAdvisorDropdownOpen.set(false);
    this.isParticipantDropdownOpen.set(false);
    this.clientInputText.set(this.selectedClientName());
    this.advisorInputText.set(this.selectedManagerName());
    this.participantInputText.set('');
    this.participantSearchQuery.set('');
  }

  goBack(): void {
    this.location.back();
  }

  saveContract(): void {
    const regNum = this.registrationNumber().trim();
    const inst = this.institution().trim();
    const start = this.startDate();
    const validity = this.validityDate();
    const client = this.clientId();
    const manager = this.contractManagerId();

    if (
      !regNum || regNum.length < 5 || regNum.length > 50 ||
      !inst ||
      !start ||
      !validity ||
      !client ||
      !manager
    ) {
      this.showValidationErrors.set(true);
      this.toastService.error('Please fix validation errors before saving.');
      return;
    }

    const request = {
      registrationNumber: regNum,
      institution: inst,
      startDate: start,
      validityDate: validity,
      endDate: this.endDate() ? this.endDate() : null,
      clientId: client,
      contractManagerId: manager,
      participantIds: this.selectedParticipantIds()
    };

    this.contractsService.createContract(request).subscribe({
      next: () => {
        this.toastService.success('Contract created successfully.');
        this.router.navigate(['/contracts']);
      },
      error: (err) => {
        console.error('[ContractsNew] Create error:', err);
        const message =
          err?.error?.message ||
          err?.error?.Message ||
          `Failed to create contract (HTTP ${err?.status ?? 'unknown'})`;
        this.toastService.error(message);
      }
    });
  }

  ngOnInit(): void {
    if (isPlatformBrowser(this.platformId)) {
      this.clientsService.getClientsLookup().subscribe({
        next: (data) => {
          this.clientsLookup.set(data);
        },
        error: (err) => {
          console.error('[ContractsNew] Clients lookup error:', err);
        }
      });

      this.advisorsService.getAdvisorsLookup().subscribe({
        next: (data) => {
          this.advisorsLookup.set(data);
        },
        error: (err) => {
          console.error('[ContractsNew] Advisors lookup error:', err);
        }
      });
    }
  }
}
