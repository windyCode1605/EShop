import { Routes } from '@angular/router';
import { authGuard } from '../../core/guards/auth.guard';
import { permissionGuard } from '../../core/guards/permission.guard';
import { PERMISSIONS } from '../../core/constants/permissions.const';

export const ADMIN_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () => import('./layout/admin-layout.component').then((m) => m.AdminLayoutComponent),
    canActivate: [authGuard],
    children: [
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
      {
        path: 'dashboard',
        loadComponent: () => import('./pages/admin-dashboard/admin-dashboard.component').then((m) => m.AdminDashboardComponent),
        canActivate: [permissionGuard],
        data: { permissions: [PERMISSIONS.DASHBOARD.VIEW] }
      },
      {
        path: 'orders',
        loadComponent: () => import('./pages/admin-orders/admin-orders.component').then((m) => m.AdminOrdersComponent),
        canActivate: [permissionGuard],
        data: { permissions: [PERMISSIONS.SALES.ORDERS_VIEW] }
      },
      {
        path: 'products',
        loadComponent: () => import('./pages/admin-products/admin-products.component').then((m) => m.AdminProductsComponent),
        canActivate: [permissionGuard],
        data: { permissions: [PERMISSIONS.CATALOG.PRODUCTS_VIEW] }
      },
      {
        path: 'categories',
        loadComponent: () => import('./pages/admin-categories/admin-categories.component').then((m) => m.AdminCategoriesComponent),
        canActivate: [permissionGuard],
        data: { permissions: [PERMISSIONS.CATALOG.CATEGORIES_VIEW] }
      },
      {
        path: 'customers',
        loadComponent: () => import('./pages/admin-customers/admin-customers.component').then((m) => m.AdminCustomersComponent),
        canActivate: [permissionGuard],
        data: { permissions: [PERMISSIONS.CUSTOMERS.VIEW] }
      },
      {
        path: 'customers/detail/:id',
        loadComponent: () => import('./pages/admin-customers/admin-customer-detail/admin-customer-detail.component').then((m) => m.AdminCustomerDetailComponent),
        canActivate: [permissionGuard],
        data: { permissions: [PERMISSIONS.CUSTOMERS.VIEW] }
      },
      {
        path: 'settings',
        loadComponent: () => import('./pages/admin-settings/admin-settings.component').then((m) => m.AdminSettingsComponent),
        canActivate: [permissionGuard],
        data: { permissions: [PERMISSIONS.SYSTEM.SETTINGS_VIEW] }
      },
      {
        path: 'roles',
        loadComponent: () => import('./pages/admin-roles/admin-roles.component').then((m) => m.AdminRolesComponent),
        canActivate: [permissionGuard],
        data: { permissions: [PERMISSIONS.IDENTITY.ROLES_VIEW, PERMISSIONS.IDENTITY.ROLES_MANAGE] }
      }
    ]
  }
];

