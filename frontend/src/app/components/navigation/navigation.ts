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

  ngOnInit(): void {
    if (isPlatformBrowser(this.platformId)) {
      this.isAdmin.set(localStorage.getItem('role') === 'Admin');
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
