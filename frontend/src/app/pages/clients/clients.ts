import { Component, inject, OnInit, PLATFORM_ID, signal, computed } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ClientsService, Client } from '../../services/clients.service';
import { ToastService } from '../../services/toast.service';
import { PersonaCardComponent } from '../../components/persona-card/persona-card';
import { NavigationComponent } from '../../components/navigation/navigation';

@Component({
  selector: 'app-clients',
  imports: [PersonaCardComponent, NavigationComponent, FormsModule],
  templateUrl: './clients.html',
  styleUrl: './clients.scss'
})
export class Clients implements OnInit {
  private clientsService = inject(ClientsService);
  private platformId = inject(PLATFORM_ID);
  private toastService = inject(ToastService);

  clients = signal<Client[]>([]);
  searchQuery = signal('');
  isLoading = signal(true);

  // Modal signals
  isModalOpen = signal(false);
  email = signal('');
  firstName = signal('');
  lastName = signal('');
  personalId = signal('');
  birthDate = signal('');
  phoneNumber = signal('');
  showValidationErrors = signal(false);

  openModal(): void {
    this.email.set('');
    this.firstName.set('');
    this.lastName.set('');
    this.personalId.set('');
    this.birthDate.set('');
    this.phoneNumber.set('');
    this.showValidationErrors.set(false);
    this.isModalOpen.set(true);
  }

  closeModal(): void {
    this.isModalOpen.set(false);
  }

  isEmailValid(val: string): boolean {
    return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(val);
  }

  isPhoneValid(val: string): boolean {
    return /^\+?\d{9,15}$/.test(val);
  }

  isPersonalIdValid(val: string): boolean {
    if (!val) return false;
    return /^(?:\d{5,6}\/\d{4}|\d{9,10})$/.test(val);
  }

  isFormValid(): boolean {
    return (
      this.isEmailValid(this.email()) &&
      this.isPhoneValid(this.phoneNumber()) &&
      this.isPersonalIdValid(this.personalId()) &&
      this.firstName().trim().length >= 2 &&
      this.lastName().trim().length >= 2 &&
      !!this.birthDate()
    );
  }

  saveClient(): void {
    this.showValidationErrors.set(true);
    if (!this.isFormValid()) {
      this.toastService.error('Please fix validation errors.');
      return;
    }

    const payload = {
      email: this.email().trim(),
      firstName: this.firstName().trim(),
      lastName: this.lastName().trim(),
      personalId: this.personalId().trim(),
      birthDate: this.birthDate(),
      phoneNumber: this.phoneNumber().trim()
    };

    this.clientsService.createClient(payload).subscribe({
      next: (created) => {
        this.toastService.success('Client created successfully.');
        this.clients.update(list => [created, ...list]);
        this.closeModal();
      },
      error: (err) => {
        console.error('[Clients] Create error:', err);
        const message = err?.error?.message || err?.error?.Message || 'Failed to create client.';
        this.toastService.error(message);
      }
    });
  }

  filteredClients = computed(() => {
    const query = this.searchQuery().toLowerCase().trim();
    const allClients = this.clients();
    if (!query) {
      return allClients;
    }
    return allClients.filter(client => {
      const firstName = (client.firstName || '').toLowerCase();
      const lastName = (client.lastName || '').toLowerCase();
      const fullName = `${firstName} ${lastName}`;
      const email = (client.email || '').toLowerCase();
      const personalId = (client.personalId || '').toLowerCase();

      return fullName.includes(query) || 
             email.includes(query) || 
             personalId.includes(query);
    });
  });

  onSearchChange(event: Event): void {
    const target = event.target as HTMLInputElement;
    this.searchQuery.set(target.value);
  }

  deleteClient(id: string): void {
    if (confirm('Are you sure you want to delete this client?')) {
      this.clientsService.deleteClient(id).subscribe({
        next: () => {
          this.toastService.success('Client deleted successfully.');
          this.clients.update(list => list.filter(c => c.id !== id));
        },
        error: (err) => {
          console.error('[Clients] Delete error:', err);
          const message =
            err?.error?.message ||
            err?.error?.Message ||
            `Failed to delete client (HTTP ${err?.status ?? 'unknown'})`;
          this.toastService.error(message);
        }
      });
    }
  }

  ngOnInit(): void {
    if (isPlatformBrowser(this.platformId)) {
      this.clientsService.getClients().subscribe({
        next: (data) => {
          console.log('[Clients] API response count:', data.length, data);
          this.clients.set(data);
          this.isLoading.set(false);
        },
        error: (err) => {
          console.error('[Clients] API error:', err);
          this.isLoading.set(false);
          const message =
            err?.error?.message ||
            err?.error?.Message ||
            `Failed to load clients (HTTP ${err?.status ?? 'unknown'})`;
          this.toastService.error(message);
        }
      });
    }
  }
}
