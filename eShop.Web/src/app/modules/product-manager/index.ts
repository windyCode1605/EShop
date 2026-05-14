/**
 * Product Manager Module Public API
 */

// Models
export { IProduct, ProductModel } from './models/product.model';
export { ProductResponseDto, ProductCreateUpdateDto } from './models/product.model';
export { IProductFilter, ProductFilterModel } from './models/product-filter.model';

// Services
export { ProductService } from './services/product.service';
export { ProductHttpService } from './services/product-http.service';

// Components
export { ProductListComponent } from './components/list/product-list.component';
export { ProductDetailComponent } from './components/detail/product-detail.component';
export { ProductCreateEditComponent } from './components/create-edit/product-create-edit.component';
export { ProductFilterComponent } from './components/filter/product-filter.component';
export { ProductItemComponent } from './components/list/product-item.component';

// Guards
export { ProductManagerGuard } from './guards/product-manager.guard';

// Module
export { ProductManagerModule } from './product-manager.module';
export { ProductManagerRoutingModule } from './product-manager-routing.module';
