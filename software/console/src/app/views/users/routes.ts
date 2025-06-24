import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: 'users',
    loadComponent: () => import('./users.component').then(m => m.DashboardComponent)
  }
];

