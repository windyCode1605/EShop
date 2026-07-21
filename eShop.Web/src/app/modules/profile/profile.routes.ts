import { Routes } from '@angular/router';
import { MainLayoutComponent } from '../../layout/main-layout/main-layout.component';

import { ProfileLayoutComponent } from './profile-layout.component';

export const PROFILE_ROUTES: Routes = [
  {
    path: '',
    component: MainLayoutComponent,
    children: [
      {
        path: '',
        component: ProfileLayoutComponent,
        children: [
          { path: '', redirectTo: 'info', pathMatch: 'full' },
          {
            path: 'info',
            loadComponent: () =>
              import('./pages/info/profile-info.component').then((m) => m.ProfileInfoComponent)
          },
          {
            path: 'address',
            loadComponent: () =>
              import('./pages/address/profile-address.component').then((m) => m.ProfileAddressComponent)
          },
          {
            path: 'orders',
            loadComponent: () =>
              import('./pages/orders/profile-orders.component').then((m) => m.ProfileOrdersComponent)
          }
        ]
      }
    ]
  }
];
