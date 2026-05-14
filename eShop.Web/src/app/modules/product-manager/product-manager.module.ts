import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { ProductManagerRoutingModule } from './product-manager-routing.module';
import { ProductListComponent } from './components/list/product-list.component';
import { ProductDetailComponent } from './components/detail/product-detail.component';
import { ProductCreateEditComponent } from './components/create-edit/product-create-edit.component';
import { ProductFilterComponent } from './components/filter/product-filter.component';
import { ProductItemComponent } from './components/list/product-item.component';
import { ProductService } from './services/product.service';
import { ProductHttpService } from './services/product-http.service';
import { ProductManagerGuard } from './guards/product-manager.guard';

/**
 * Product Manager Module
 * Feature module for product management
 * 
 * Components:
 * - ProductListComponent: Display list of products
 * - ProductDetailComponent: Display product details
 * - ProductCreateEditComponent: Create/Edit products
 * - ProductFilterComponent: Filter products
 * - ProductItemComponent: Display individual product
 * 
 * Services:
 * - ProductService: Business logic
 * - ProductHttpService: API communication
 * 
 * Guards:
 * - ProductManagerGuard: Route protection
 */
@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    ProductManagerRoutingModule,
    ProductListComponent,
    ProductDetailComponent,
    ProductCreateEditComponent,
    ProductFilterComponent,
    ProductItemComponent
  ],
  providers: [
    ProductService,
    ProductHttpService,
    ProductManagerGuard
  ]
})
export class ProductManagerModule { }
