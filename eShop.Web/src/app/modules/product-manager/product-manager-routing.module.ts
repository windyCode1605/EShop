import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ProductListComponent } from './components/list/product-list.component';
import { ProductDetailComponent } from './components/detail/product-detail.component';
import { ProductCreateEditComponent } from './components/create-edit/product-create-edit.component';
import { ProductManagerGuard } from './guards/product-manager.guard';

/**
 * Product Manager Routes
 */
const routes: Routes = [
  {
    path: '',
    component: ProductListComponent,
    canActivate: [ProductManagerGuard],
    data: { title: 'Product Manager' }
  },
  {
    path: 'detail/:id',
    component: ProductDetailComponent,
    canActivate: [ProductManagerGuard],
    data: { title: 'Product Details' }
  },
  {
    path: 'create',
    component: ProductCreateEditComponent,
    canActivate: [ProductManagerGuard],
    data: { title: 'Create Product' }
  },
  {
    path: 'edit/:id',
    component: ProductCreateEditComponent,
    canActivate: [ProductManagerGuard],
    data: { title: 'Edit Product' }
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ProductManagerRoutingModule { }
