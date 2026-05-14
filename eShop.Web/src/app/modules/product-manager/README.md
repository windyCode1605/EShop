# Product Manager Module

## Overview
The Product Manager module provides a complete feature set for managing products in the eShop application. It follows the specified architecture pattern with smart (container) and dumb (presentational) components.

## Structure
```
product-manager/
├── models/
│   ├── product.model.ts
│   └── product-filter.model.ts
├── services/
│   ├── product.service.ts
│   └── product-http.service.ts
├── components/
│   ├── list/
│   │   ├── product-list.component.ts (Smart)
│   │   └── product-item.component.ts (Dumb)
│   ├── detail/
│   │   └── product-detail.component.ts (Smart)
│   ├── create-edit/
│   │   └── product-create-edit.component.ts (Smart)
│   └── filter/
│       └── product-filter.component.ts (Dumb)
├── guards/
│   └── product-manager.guard.ts
├── product-manager.module.ts
├── product-manager-routing.module.ts
└── index.ts
```

## Features

### Components

#### ProductListComponent (Smart)
- Lists all products with pagination
- Filters and search functionality
- Create, edit, delete operations
- Responsive grid layout

#### ProductDetailComponent (Smart)
- Display full product information
- Stock details
- Navigation to edit

#### ProductCreateEditComponent (Smart)
- Create new products
- Edit existing products
- Form validation
- Image upload support

#### ProductFilterComponent (Dumb)
- Search by keyword
- Filter by category, price range
- Sort by various columns
- Pagination controls

#### ProductItemComponent (Dumb)
- Display product in card format
- Quick action buttons
- Stock status indicator

### Services

#### ProductService
- State management using BehaviorSubjects
- Business logic for product operations
- Create, Read, Update, Delete operations

#### ProductHttpService
- API communication
- HTTP method implementations
- Request/Response handling

### Routes

```
/product-manager                    → ProductListComponent
/product-manager/detail/:id         → ProductDetailComponent
/product-manager/create             → ProductCreateEditComponent
/product-manager/edit/:id           → ProductCreateEditComponent
```

## Usage

### Import Module
```typescript
import { ProductManagerModule } from '@shared/modules/product-manager';

@NgModule({
  imports: [ProductManagerModule]
})
export class AdminModule { }
```

### Add to Routes
```typescript
const routes: Routes = [
  {
    path: 'product-manager',
    loadChildren: () => import('./modules/product-manager/product-manager.module').then(m => m.ProductManagerModule)
  }
];
```

### Use Service
```typescript
import { ProductService } from '@shared/modules/product-manager';

constructor(private productService: ProductService) {}

ngOnInit() {
  const filter = new ProductFilterModel({ pageSize: 20 });
  this.productService.getProducts(filter).subscribe(response => {
    // Handle products
  });
}
```

## API Integration

Update the `apiUrl` in `ProductHttpService` to match your backend API:

```typescript
private apiUrl = '/api/products'; // Change to your API endpoint
```

## Customization

### Add Permissions
Update `ProductManagerGuard` to add permission checks:

```typescript
canActivate(): boolean {
  return this.permissionService.hasPermission('PRODUCT_MANAGE');
}
```

### Extend Filtering
Modify `ProductFilterModel` to add more filter options:

```typescript
export class ProductFilterModel implements IProductFilter {
  // Add new properties
  brand?: string;
  supplier?: string;
}
```

### Customize Styling
Styles are modular - each component has its own SCSS file for easy customization.

## Form Validation

The create/edit form includes validation for:
- Required fields
- Field length constraints
- Numeric validation
- Email format (extensible)

## Dependencies

- Angular 18+
- RxJS 7.8+
- Bootstrap 5+ (for styling)
- Bootstrap Icons (optional, for enhanced UI)

## Future Enhancements

- [ ] Bulk import/export
- [ ] Advanced filters
- [ ] Product variants
- [ ] Image gallery
- [ ] Product reviews
- [ ] Wishlist integration
- [ ] Analytics dashboard
