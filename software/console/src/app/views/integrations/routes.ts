import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: 'connections',
    loadComponent: () => import('./connections/connections.component').then(m => m.ConnectionsComponent)
  },
  {
    path: 'outbound',
    loadComponent: () => import('./outbound/outbound.component').then(m => m.OutboundComponent)
  }
];
