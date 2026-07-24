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
      },
      {
        path: 'customers',
        loadComponent: () => import('./pages/admin-customers/admin-customers.component').then((m) => m.AdminCustomersComponent)
      },
      {
        path: 'settings',
        loadComponent: () => import('./pages/admin-settings/admin-settings.component').then((m) => m.AdminSettingsComponent)
      },
      {
        path: 'roles',
        loadComponent: () => import('./pages/admin-roles/admin-roles.component').then((m) => m.AdminRolesComponent)
      }
    ]
  }
];

// Trigger reload
