import { Routes } from '@angular/router';
import { authGuard } from './guards/auth.guard';

export const routes: Routes = [
  { path: '', redirectTo: '/login', pathMatch: 'full' },
  { path: 'login', loadComponent: () => import('./pages/login/login').then(m => m.Login) },
  { path: 'signup', loadComponent: () => import('./pages/signup/signup').then(m => m.Signup) },
  { path: 'clients', loadComponent: () => import('./pages/clients/clients').then(m => m.Clients), canActivate: [authGuard] },
  { path: 'client/:id', loadComponent: () => import('./pages/client-detail/client-detail').then(m => m.ClientDetail), canActivate: [authGuard] },
  { path: 'advisors', loadComponent: () => import('./pages/advisors/advisors').then(m => m.Advisors), canActivate: [authGuard] },
  { path: 'advisor/:id', loadComponent: () => import('./pages/advisor-detail/advisor-detail').then(m => m.AdvisorDetail), canActivate: [authGuard] },
  { path: 'contracts', loadComponent: () => import('./pages/contracts/contracts').then(m => m.Contracts), canActivate: [authGuard] },
  { path: 'admin/contracts', loadComponent: () => import('./pages/admin-contracts/admin-contracts').then(m => m.AdminContracts), canActivate: [authGuard] },
  { path: 'contracts/new', loadComponent: () => import('./pages/contracts-new/contracts-new').then(m => m.ContractsNew), canActivate: [authGuard] },
  { path: 'contract/:id', loadComponent: () => import('./pages/contract-detail/contract-detail').then(m => m.ContractDetail), canActivate: [authGuard] }
];
