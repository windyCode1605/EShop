import { Routes } from '@angular/router';
import { OrderPageComponent } from './pages/order-page/order-page.component';
import { OrderHistoryComponent } from './pages/order-history/order-history.component';

export const ORDER_ROUTES: Routes = [
  {
    path: '',
    component: OrderHistoryComponent
  },
  {
    path: ':id',
    component: OrderPageComponent
  }
];
