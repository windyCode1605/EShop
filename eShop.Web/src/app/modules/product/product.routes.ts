import { Routes } from '@angular/router';
import { ProductPageComponent } from './pages/products/product-page.component';
import { ProductDetailComponent } from './pages/product-detail/product-detail.component';

export const PRODUCT_ROUTES: Routes = [
  {
    path: '',
    component: ProductPageComponent
  },
  {
    path: ':id',
    component: ProductDetailComponent
  }
];
