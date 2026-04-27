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
    path: '',
    redirectTo: 'events',
    pathMatch: 'full'
  }
];
