import { Routes } from '@angular/router';

export const routes: Routes = [
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
