import { Routes } from '@angular/router';
import { DashboardComponent } from './modules/dashboard/pages/dashboard/dashboard.component';
import { ProductsComponent } from './modules/product/pages/products/products.component';
import { authGuard } from './core/guards/auth.guard';
import { roleGuard } from './core/guards/role.guard';

export const routes: Routes = [
  { path: '', redirectTo: '/dashboard', pathMatch: 'full' },
  { path: 'login', redirectTo: '/account/login', pathMatch: 'full' },
  {
    path: 'dashboard',
    canActivate: [authGuard],
    loadChildren: () => import('./modules/dashboard/dashboard.routes').then(m => m.DASHBOARD_ROUTES)
  },
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
    loadChildren: () => import('./modules/cart/cart.routes').then((m) => m.CART_ROUTES)
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
    canActivate: [authGuard, roleGuard],
    loadChildren: () => import('./modules/admin/admin.routes').then((m) => m.ADMIN_ROUTES)
  },
  { path: '**', redirectTo: '/dashboard' }
];

// Trigger reload
