import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () => import('./variables.component').then(m => m.VariablesComponent)
  }
];

