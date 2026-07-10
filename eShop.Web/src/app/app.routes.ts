import { Routes } from '@angular/router';
import { DashboardComponent } from './modules/dashboard/pages/dashboard/dashboard.component';
import { ProductsComponent } from './modules/product/pages/products/products.component';
import { authGuard } from './core/guards/auth.guard';
import { roleGuard } from './core/guards/role.guard';

export const routes: Routes = [
  { path: '', redirectTo: '/dashboard', pathMatch: 'full' },
  { path: 'login', redirectTo: '/account/login', pathMatch: 'full' },
  { path: 'dashboard', component: DashboardComponent, canActivate: [authGuard] },
  { path: 'products', component: ProductsComponent, canActivate: [authGuard] },
  {
    path: 'account',
    loadChildren: () => import('./modules/account/account.routes').then((m) => m.ACCOUNT_ROUTES)
  },
  {
    path: 'product',
    canActivate: [authGuard],
    loadChildren: () => import('./modules/product/product.routes').then((m) => m.PRODUCT_ROUTES)
  },
  {
    path: 'order',
    canActivate: [authGuard],
    loadChildren: () => import('./modules/order/order.routes').then((m) => m.ORDER_ROUTES)
  },
  {
    path: 'cart',
    canActivate: [authGuard],
    loadChildren: () => import('./modules/Cart/cart.routes').then((m) => m.CART_ROUTES)
  },


  {
    path: 'product-manager',
    canActivate: [authGuard],
    loadChildren: () => import('./modules/product-manager/product-manager.module').then(m => m.ProductManagerModule)
  },
  {
    path: 'checkout',
    canActivate: [authGuard],
    loadChildren: () => import('./modules/checkout/checkout.routes').then((m) => m.CHECKOUT_ROUTES)
  },
  {
    path: 'admin',
    canActivate: [authGuard, roleGuard], // Tạm thời comment lại để xem demo
    data: { roles: ['Admin'] },
    loadComponent: () => import('./modules/admin/layout/admin-layout.component').then((m) => m.AdminLayoutComponent),
    children: [
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
      {
        path: 'dashboard',
        loadComponent: () => import('./modules/admin/pages/admin-dashboard/admin-dashboard.component').then((m) => m.AdminDashboardComponent)
      },
      {
        path: 'orders',
        loadComponent: () => import('./modules/admin/pages/admin-orders/admin-orders.component').then((m) => m.AdminOrdersComponent)
      }
    ]
  },
  { path: '**', redirectTo: '/dashboard' }
];

// Trigger reload
