import { Component, inject, OnInit, PLATFORM_ID, signal } from '@angular/core';
import { isPlatformBrowser, Location } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { ClientsService, Client } from '../../services/clients.service';
import { ContractsService, Contract } from '../../services/contracts.service';
import { ToastService } from '../../services/toast.service';
import { NavigationComponent } from '../../components/navigation/navigation';
import { ContractCardComponent } from '../../components/contract-card/contract-card';

@Component({
  selector: 'app-client-detail',
  imports: [NavigationComponent, ContractCardComponent, FormsModule],
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
  private location = inject(Location);

  client = signal<Client | null>(null);
  isEditing = signal(false);
  isSaving = signal(false);
  showValidationErrors = signal(false);

  editForm = {
    firstName: '',
    lastName: '',
    email: '',
    personalId: '',
    birthDate: ''
  };

  isEmailValid(val: string): boolean {
    return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(val);
  }

  isPersonalIdValid(val: string): boolean {
    if (!val) return true;
    return /^(?:\d{5,6}\/\d{4}|\d{9,10})$/.test(val);
  }

  startEdit(c: Client): void {
    this.editForm = {
      firstName: c.firstName || '',
      lastName: c.lastName || '',
      email: c.email || '',
      personalId: c.personalId || '',
      birthDate: c.birthDate ? c.birthDate.substring(0, 10) : ''
    };
    this.showValidationErrors.set(false);
    this.isEditing.set(true);
  }

  cancelEdit(): void {
    this.isEditing.set(false);
  }

  saveClient(): void {
    this.showValidationErrors.set(true);

    const firstNameValid = this.editForm.firstName.trim().length >= 2;
    const lastNameValid = this.editForm.lastName.trim().length >= 2;
    const emailValid = this.isEmailValid(this.editForm.email.trim());
    const personalIdValid = this.isPersonalIdValid(this.editForm.personalId.trim());
    const birthDateValid = !!this.editForm.birthDate;

    if (!firstNameValid || !lastNameValid || !emailValid || !personalIdValid || !birthDateValid) {
      this.toastService.error('Please fix the validation errors.');
      return;
    }

    this.isSaving.set(true);

    let personalId = this.editForm.personalId.trim();
    if (personalId && /^\d{9,10}$/.test(personalId)) {
      if (personalId.length === 10) {
        personalId = personalId.substring(0, 6) + '/' + personalId.substring(6);
      } else {
        personalId = personalId.substring(0, 5) + '/' + personalId.substring(5);
      }
    }

    const payload = {
      firstName: this.editForm.firstName.trim(),
      lastName: this.editForm.lastName.trim(),
      email: this.editForm.email.trim(),
      personalId: personalId || null,
      birthDate: this.editForm.birthDate
    };

    const currentClient = this.client();
    if (!currentClient) return;

    this.clientsService.updateClient(currentClient.id, payload).subscribe({
      next: () => {
        this.toastService.success('Client updated successfully.');
        this.client.update(c => {
          if (!c) return null;
          return {
            ...c,
            firstName: payload.firstName,
            lastName: payload.lastName,
            email: payload.email,
            personalId: payload.personalId || undefined,
            birthDate: payload.birthDate
          };
        });
        this.isEditing.set(false);
        this.isSaving.set(false);
      },
      error: (err) => {
        console.error('[ClientDetail] Error updating client:', err);
        this.isSaving.set(false);
        const message = err?.error?.message || err?.error?.Message || 'Failed to update client.';
        this.toastService.error(message);
      }
    });
  }

  goBack(): void {
    this.location.back();
  }
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
    this.contractsService.getClientContracts(clientId).subscribe({
      next: (data) => {
        this.contracts.set(data);
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
