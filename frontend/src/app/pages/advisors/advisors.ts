import { Component, inject, OnInit, PLATFORM_ID, signal, computed } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { AdvisorsService, Advisor } from '../../services/advisors.service';
import { ToastService } from '../../services/toast.service';
import { PersonaCardComponent } from '../../components/persona-card/persona-card';
import { NavigationComponent } from '../../components/navigation/navigation';

@Component({
  selector: 'app-advisors',
  imports: [PersonaCardComponent, NavigationComponent],
  templateUrl: './advisors.html',
  styleUrl: './advisors.scss'
})
export class Advisors implements OnInit {
  private advisorsService = inject(AdvisorsService);
  private platformId = inject(PLATFORM_ID);
  private toastService = inject(ToastService);

  advisors = signal<Advisor[]>([]);
  searchQuery = signal('');
  isLoading = signal(true);

  filteredAdvisors = computed(() => {
    const query = this.searchQuery().toLowerCase().trim();
    const allAdvisors = this.advisors();
    if (!query) {
      return allAdvisors;
    }
    return allAdvisors.filter(advisor => {
      const firstName = (advisor.firstName || '').toLowerCase();
      const lastName = (advisor.lastName || '').toLowerCase();
      const fullName = `${firstName} ${lastName}`;
      const email = (advisor.email || '').toLowerCase();
      const personalId = (advisor.personalId || '').toLowerCase();

      return fullName.includes(query) || 
             email.includes(query) || 
             personalId.includes(query);
    });
  });

  onSearchChange(event: Event): void {
    const target = event.target as HTMLInputElement;
    this.searchQuery.set(target.value);
  }

  deleteAdvisor(id: string): void {
    if (confirm('Are you sure you want to delete this advisor?')) {
      this.advisorsService.deleteAdvisor(id).subscribe({
        next: () => {
          this.toastService.success('Advisor deleted successfully.');
          this.advisors.update(list => list.filter(a => a.id !== id));
        },
        error: (err) => {
          console.error('[Advisors] Delete error:', err);
          const message =
            err?.error?.message ||
            err?.error?.Message ||
            `Failed to delete advisor (HTTP ${err?.status ?? 'unknown'})`;
          this.toastService.error(message);
        }
      });
    }
  }

  ngOnInit(): void {
    if (isPlatformBrowser(this.platformId)) {
      this.advisorsService.getAdvisors().subscribe({
        next: (data) => {
          console.log('[Advisors] API response count:', data.length, data);
          this.advisors.set(data);
          this.isLoading.set(false);
        },
        error: (err) => {
          console.error('[Advisors] API error:', err);
          this.isLoading.set(false);
          const message =
            err?.error?.message ||
            err?.error?.Message ||
            `Failed to load advisors (HTTP ${err?.status ?? 'unknown'})`;
          this.toastService.error(message);
        }
      });
    }
  }
}
