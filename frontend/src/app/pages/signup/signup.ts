import { Component, inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
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
  private router = inject(Router);
  private platformId = inject(PLATFORM_ID);

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
    if (!val) return false;
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
      personalId: this.personalId.trim(),
      birthDate: this.birthDate,
      phoneNumber: this.phoneNumber.trim()
    };

    this.authService.registerAdvisor(payload).subscribe({
      next: () => {
        this.toastService.success('Registration successful! Logging in...');
        
        // Auto login with the registered credentials
        this.authService.login({ email: payload.email, password: payload.password }).subscribe({
          next: (res) => {
            this.isLoading = false;
            this.isRegistered = true;
            if (isPlatformBrowser(this.platformId)) {
              if (res.token) {
                localStorage.setItem('token', res.token);
              }
              if (res.user) {
                localStorage.setItem('role', res.user.role);
                localStorage.setItem('userId', res.user.id);
              }
            }
            this.router.navigate(['/contracts']);
          },
          error: (loginErr) => {
            console.error('[Signup] Auto-login error:', loginErr);
            this.isLoading = false;
            this.isRegistered = true;
            this.toastService.warning('Registration succeeded, but automatic login failed. Please login manually.');
            this.router.navigate(['/login']);
          }
        });
      },
      error: (err) => {
        this.isLoading = false;
        let errorMessage = 'Registration failed. Please try again.';
        if (err?.error) {
          if (typeof err.error === 'string') {
            errorMessage = err.error;
          } else if (err.error.message || err.error.Message) {
            errorMessage = err.error.message || err.error.Message;
          } else if (typeof err.error === 'object') {
            const errors: string[] = [];
            for (const key in err.error) {
              if (Object.prototype.hasOwnProperty.call(err.error, key)) {
                const value = err.error[key];
                if (Array.isArray(value)) {
                  errors.push(...value);
                } else if (typeof value === 'string') {
                  errors.push(value);
                }
              }
            }
            if (errors.length > 0) {
              errorMessage = errors.join(' ');
            }
          }
        }
        this.toastService.error(errorMessage);
      }
    });
  }
}
