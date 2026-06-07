import { inject, PLATFORM_ID } from '@angular/core';
import { Router, CanActivateFn } from '@angular/router';
import { isPlatformBrowser } from '@angular/common';

export const authGuard: CanActivateFn = () => {
  const router = inject(Router);
  const platformId = inject(PLATFORM_ID);
  
  if (isPlatformBrowser(platformId)) {
    const token = localStorage.getItem('token');
    const role = localStorage.getItem('role');
    if (token && (role === 'Admin' || role === 'Advisor')) {
      return true;
    }
    return router.parseUrl('/login');
  }
  
  // On server side, we can just allow navigation or block it
  // Usually returning false or url tree blocks it
  return true;
};
