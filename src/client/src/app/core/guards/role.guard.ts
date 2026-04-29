import { inject } from '@angular/core';
import { Router, type CanActivateFn } from '@angular/router';
import { AuthService } from '../services/auth.service';

export const roleGuard: CanActivateFn = (route) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  const requiredRole = route.data?.['role'];

  if (!authService.isLoggedIn()) {
    return router.parseUrl('/login');
  }

  if (requiredRole && authService.hasRole(requiredRole)) {
    return true;
  }

  return router.parseUrl('/forbidden');
};
