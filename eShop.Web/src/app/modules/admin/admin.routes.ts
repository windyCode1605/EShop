import { Routes } from '@angular/router';

export const ADMIN_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () => import('./layout/admin-layout.component').then((m) => m.AdminLayoutComponent),
    children: [
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
      {
        path: 'dashboard',
        loadComponent: () => import('./pages/admin-dashboard/admin-dashboard.component').then((m) => m.AdminDashboardComponent)
      },
      {
        path: 'orders',
        loadComponent: () => import('./pages/admin-orders/admin-orders.component').then((m) => m.AdminOrdersComponent)
      },
      {
        path: 'products',
        loadComponent: () => import('./pages/admin-products/admin-products.component').then((m) => m.AdminProductsComponent)
      },
      {
        path: 'categories',
        loadComponent: () => import('./pages/admin-categories/admin-categories.component').then((m) => m.AdminCategoriesComponent)
      }
    ]
  }
];

