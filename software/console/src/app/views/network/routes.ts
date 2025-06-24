import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: 'summary',
    loadComponent: () => import('./summary/network-summary.component').then(m => m.NetworkSummaryComponent)
  },
  {
    path: 'ethernet',
    loadComponent: () => import('./ethernet/network-ethernet.component').then(m => m.NetworkEthernetComponent)
  },
  {
    path: 'wifi',
    loadComponent: () => import('./wifi/network-wifi.component').then(m => m.NetworkWiFiComponent)
  }
];

