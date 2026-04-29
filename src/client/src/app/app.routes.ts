import { Routes } from '@angular/router';
import { loggedOutOnlyGuard } from './core/guards/logged-out-only.guard';

export const routes: Routes = [
  {
    path: 'login',
    canActivate: [loggedOutOnlyGuard],
    loadComponent: () => import('./features/auth/login/login.component').then(m => m.LoginComponent)
  },
  {
    path: 'register',
    canActivate: [loggedOutOnlyGuard],
    loadComponent: () => import('./features/auth/register/register.component').then(m => m.RegisterComponent)
  },
  {
    path: 'events',
    loadComponent: () => import('./features/events/events-list.component').then(m => m.EventsListComponent)
  },
  {
    path: 'events/:id',
    loadComponent: () => import('./features/events/event-details/event-details.component').then(m => m.EventDetailsComponent)
  },
  {
    path: 'register',
    loadComponent: () => import('./features/auth/register/register.component').then(m => m.RegisterComponent)
  },
  {
    path: 'forbidden',
    loadComponent: () => import('./features/errors/forbidden.component').then(m => m.ForbiddenComponent)
  },
  {
    path: 'server-error',
    loadComponent: () => import('./features/errors/server-error.component').then(m => m.ServerErrorComponent)
  },
  {
    path: 'not-found',
    loadComponent: () => import('./features/errors/not-found.component').then(m => m.NotFoundComponent)
  },
  {
    path: '',
    redirectTo: 'events',
    pathMatch: 'full'
  },
  {
    path: '**',
    redirectTo: 'not-found',
    pathMatch: 'full'
  }
];
