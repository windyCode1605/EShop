import { Routes } from '@angular/router';
import { DashboardComponent } from './modules/dashboard/pages/dashboard/dashboard.component';
import { ProductsComponent } from './modules/product/pages/products/products.component';

export const routes: Routes = [
  { path: '', redirectTo: '/dashboard', pathMatch: 'full' },
  { path: 'login', redirectTo: '/account/login', pathMatch: 'full' },
  { path: 'dashboard', component: DashboardComponent },
  { path: 'products', component: ProductsComponent },
  {
    path: 'account',
    loadChildren: () => import('./modules/account/account.routes').then((m) => m.ACCOUNT_ROUTES)
  },
  {
    path: 'product',
    loadChildren: () => import('./modules/product/product.routes').then((m) => m.PRODUCT_ROUTES)
  },
  {
    path: 'order',
    loadChildren: () => import('./modules/order/order.routes').then((m) => m.ORDER_ROUTES)
  },
  
  // Product Manager Module
  {
    path: 'product-manager',
    loadChildren: () => import('./modules/product-manager/product-manager.module').then(m => m.ProductManagerModule)
  },
  
  { path: '**', redirectTo: '/product-manager' }
];
