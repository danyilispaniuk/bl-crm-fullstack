import { Component, inject, OnInit, PLATFORM_ID, ChangeDetectorRef } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../services/auth.service';
import { ToastService } from '../../services/toast.service';

@Component({
  selector: 'app-login',
  imports: [FormsModule, RouterLink],
  templateUrl: './login.html',
  styleUrl: './login.scss'
})
export class Login implements OnInit {
  private router = inject(Router);
  private authService = inject(AuthService);
  private platformId = inject(PLATFORM_ID);
  private toastService = inject(ToastService);
  private cdr = inject(ChangeDetectorRef);

  email = '';
  password = '';
  isLoading = false;

  ngOnInit() {
    if (isPlatformBrowser(this.platformId)) {
      const role = localStorage.getItem('role');
      const token = localStorage.getItem('token');
      if (token) {
        if (role === 'Admin') {
          this.router.navigate(['/admin/contracts']);
        } else if (role === 'Advisor') {
          this.router.navigate(['/contracts']);
        }
      }
    }
  }

  onLogin() {
    if (!this.email.trim()) {
      this.toastService.error('Please enter your email address.');
      return;
    }
    if (!this.password.trim()) {
      this.toastService.error('Please enter your password.');
      return;
    }

    this.isLoading = true;
    this.cdr.markForCheck();

    this.authService.login({ email: this.email, password: this.password }).subscribe({
      next: (res) => {
        this.isLoading = false;
        this.cdr.markForCheck();
        if (isPlatformBrowser(this.platformId)) {
          if (res.user?.role !== 'Admin' && res.user?.role !== 'Advisor') {
            this.toastService.error('Access denied. Only administrators and advisors are allowed to log in.');
            return;
          }
          if (res.token) {
            localStorage.setItem('token', res.token);
          }
          if (res.user) {
            localStorage.setItem('role', res.user.role);
            localStorage.setItem('userId', res.user.id);
          }
        }
        const targetRoute = res.user?.role === 'Advisor' ? '/contracts' : '/admin/contracts';
        this.router.navigate([targetRoute]);
      },
      error: (err) => {
        this.isLoading = false;
        this.cdr.markForCheck();
        const message =
          err?.error?.message ||
          err?.error?.Message ||
          'Invalid email or password. Please try again.';
        this.toastService.error(message);
      }
    });
  }
}
