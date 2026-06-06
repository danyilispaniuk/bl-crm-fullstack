import { Component, inject, OnInit, PLATFORM_ID, signal, computed } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { ClientsService, Client } from '../../services/clients.service';
import { ToastService } from '../../services/toast.service';
import { PersonaCardComponent } from '../../components/persona-card/persona-card';
import { NavigationComponent } from '../../components/navigation/navigation';

@Component({
  selector: 'app-clients',
  imports: [PersonaCardComponent, NavigationComponent],
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
