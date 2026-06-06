import { Routes } from '@angular/router';
import { authGuard } from './guards/auth.guard';

export const routes: Routes = [
  { path: '', redirectTo: '/login', pathMatch: 'full' },
  { path: 'login', loadComponent: () => import('./pages/login/login').then(m => m.Login) },
  { path: 'clients', loadComponent: () => import('./pages/clients/clients').then(m => m.Clients), canActivate: [authGuard] },
  { path: 'advisors', loadComponent: () => import('./pages/advisors/advisors').then(m => m.Advisors), canActivate: [authGuard] },
  { path: 'contracts', loadComponent: () => import('./pages/contracts/contracts').then(m => m.Contracts), canActivate: [authGuard] }
];
