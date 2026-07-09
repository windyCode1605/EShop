import { Routes } from '@angular/router';
import { OrderPageComponent } from './pages/order-page/order-page.component';

export const ORDER_ROUTES: Routes = [
  {
    path: ':id',
    component: OrderPageComponent
  }
];
