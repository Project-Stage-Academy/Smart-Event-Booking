import { inject } from '@angular/core';
import { Router, type CanActivateFn } from '@angular/router';
import { AuthService } from '../services/auth.service';

export const roleGuard: CanActivateFn = (route) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  if (!authService.isLoggedIn()) {
    return router.createUrlTree(['/login']);
  }

  const role = route.data?.['role'];
  const roles = route.data?.['roles'];

  let requiredRoles: string[] = [];

  if (typeof role === 'string') {
    requiredRoles.push(role);
  } else if (Array.isArray(roles)) {
    requiredRoles = roles.filter(r => typeof r === 'string');
  }

  if (requiredRoles.length === 0) {
    console.warn(`RoleGuard: No valid role settings found for the route. Access denied.`);
    return router.createUrlTree(['/forbidden']);
  }

  const hasAccess = requiredRoles.some(r => authService.hasRole(r));

  if (hasAccess) {
    return true;
  }

  return router.createUrlTree(['/forbidden']);
};
