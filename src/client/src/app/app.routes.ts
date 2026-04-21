import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: 'events',
    loadComponent: () => import('./features/events/events-list.component').then(m => m.EventsListComponent)
  },
  {
    path: '',
    redirectTo: 'events',
    pathMatch: 'full'
  }
];
