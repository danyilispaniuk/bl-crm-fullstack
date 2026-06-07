import { Component, inject, OnInit, PLATFORM_ID, signal } from '@angular/core';
import { isPlatformBrowser, Location } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { AdvisorsService, Advisor } from '../../services/advisors.service';
import { ContractsService, Contract } from '../../services/contracts.service';
import { ToastService } from '../../services/toast.service';
import { NavigationComponent } from '../../components/navigation/navigation';
import { ContractCardComponent } from '../../components/contract-card/contract-card';

@Component({
  selector: 'app-advisor-detail',
  imports: [NavigationComponent, ContractCardComponent, FormsModule],
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
  private location = inject(Location);

  advisor = signal<Advisor | null>(null);
  isEditing = signal(false);
  isSaving = signal(false);
  showValidationErrors = signal(false);
  currentUserRole = signal<string | null>(null);

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

  startEdit(a: Advisor): void {
    this.editForm = {
      firstName: a.firstName || '',
      lastName: a.lastName || '',
      email: a.email || '',
      personalId: a.personalId || '',
      birthDate: a.birthDate ? a.birthDate.substring(0, 10) : ''
    };
    this.showValidationErrors.set(false);
    this.isEditing.set(true);
  }

  cancelEdit(): void {
    this.isEditing.set(false);
  }

  saveAdvisor(): void {
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

    const currentAdvisor = this.advisor();
    if (!currentAdvisor) return;

    this.advisorsService.updateAdvisor(currentAdvisor.id, payload).subscribe({
      next: () => {
        this.toastService.success('Advisor updated successfully.');
        this.advisor.update(a => {
          if (!a) return null;
          return {
            ...a,
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
        console.error('[AdvisorDetail] Error updating advisor:', err);
        this.isSaving.set(false);
        const message = err?.error?.message || err?.error?.Message || 'Failed to update advisor.';
        this.toastService.error(message);
      }
    });
  }

  goBack(): void {
    this.location.back();
  }
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
      this.currentUserRole.set(localStorage.getItem('role'));
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
