import { Component, inject } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../services/auth.service';
import { ToastService } from '../../services/toast.service';

@Component({
  selector: 'app-signup',
  imports: [FormsModule, RouterLink],
  templateUrl: './signup.html',
  styleUrl: './signup.scss'
})
export class Signup {
  private authService = inject(AuthService);
  private toastService = inject(ToastService);

  email = '';
  password = '';
  firstName = '';
  lastName = '';
  personalId = '';
  birthDate = '';
  phoneNumber = '';
  
  isLoading = false;
  showValidationErrors = false;
  isRegistered = false;

  isEmailValid(val: string): boolean {
    return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(val);
  }

  isPhoneValid(val: string): boolean {
    return /^\+?\d{9,15}$/.test(val);
  }

  isPersonalIdValid(val: string): boolean {
    if (!val) return true;
    return /^(?:\d{5,6}\/\d{4}|\d{9,10})$/.test(val);
  }

  isFormValid(): boolean {
    return (
      this.isEmailValid(this.email) &&
      this.password.length >= 8 &&
      this.isPhoneValid(this.phoneNumber) &&
      this.isPersonalIdValid(this.personalId) &&
      this.firstName.trim().length >= 2 &&
      this.lastName.trim().length >= 2 &&
      !!this.birthDate
    );
  }

  onSignup(): void {
    this.showValidationErrors = true;
    if (!this.isFormValid()) {
      this.toastService.error('Please fix the validation errors before submitting.');
      return;
    }

    this.isLoading = true;
    const payload = {
      email: this.email.trim(),
      password: this.password,
      firstName: this.firstName.trim(),
      lastName: this.lastName.trim(),
      personalId: this.personalId.trim() || null,
      birthDate: this.birthDate,
      phoneNumber: this.phoneNumber.trim()
    };

    this.authService.registerAdvisor(payload).subscribe({
      next: () => {
        this.isLoading = false;
        this.isRegistered = true;
        this.toastService.success('Registration successful!');
      },
      error: (err) => {
        this.isLoading = false;
        const message = err?.error?.message || err?.error?.Message || 'Registration failed. Please try again.';
        this.toastService.error(message);
      }
    });
  }
}
