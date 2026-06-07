import { isPlatformBrowser } from '@angular/common';
import { Component, inject, PLATFORM_ID, OnInit, signal } from '@angular/core';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';

@Component({
  selector: 'app-navigation',
  imports: [RouterLink, RouterLinkActive],
  templateUrl: './navigation.html',
  styleUrl: './navigation.scss'
})
export class NavigationComponent implements OnInit {
  private router = inject(Router);
  private platformId = inject(PLATFORM_ID);

  isAdmin = signal(false);
  isAdvisor = signal(false);
  advisorId = signal<string | null>(null);

  ngOnInit(): void {
    if (isPlatformBrowser(this.platformId)) {
      const role = localStorage.getItem('role');
      this.isAdmin.set(role === 'Admin');
      this.isAdvisor.set(role === 'Advisor');
      this.advisorId.set(localStorage.getItem('userId'));
    }
  }

  onLogout(): void {
    if (isPlatformBrowser(this.platformId)) {
      localStorage.removeItem('token');
      localStorage.removeItem('role');
    }

    this.router.navigate(['/login']);
  }
}
