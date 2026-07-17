import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule, Router } from '@angular/router';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { ProductService } from '../../services/product.service';
import { ProductFilterModel } from '../../models/product-filter.model';
import { IProduct } from '../../models/product.model';
import { ProductItemComponent } from './product-item.component';

/**
 * Product List Component (Smart/Container Component)
 * Handles product list logic, filtering, and state management
 */
@Component({
  selector: 'app-product-list',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    RouterModule,
    ProductItemComponent
  ],
  templateUrl: './product-list.component.html',
  styleUrls: ['./product-list.component.scss']
})
export class ProductListComponent implements OnInit, OnDestroy {
  products: IProduct[] = [];
  loading: boolean = false;
  error: string | null = null;
  currentFilter: ProductFilterModel;
  totalPages: number = 0;
  totalItems: number = 0;
  categories: { id: number; name: string }[] = [];
  featuredCategories: { id: number | null; name: string }[] = [{ id: null, name: 'Tất cả' }];
  selectedCategory: number | null = null;
  selectedProduct: IProduct | null = null;
  cartCount: number = 0;
  sortValue: 'popular' | 'priceAsc' | 'priceDesc' | 'newest' = 'popular';

  private destroy$ = new Subject<void>();

  constructor(
    private productService: ProductService,
    private router: Router
  ) {
    this.currentFilter = new ProductFilterModel();
  }

  ngOnInit(): void {
    this.loadProducts();
    this.subscribeToProducts();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  /**
   * Load products with current filter
   */
  loadProducts(): void {
    this.error = null;
    this.productService
      .getProducts(this.currentFilter)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (response) => {
          this.totalPages = response.totalPages;
          this.totalItems = response.totalCount;
        },
        error: (err) => {
          this.error = 'Failed to load products. Please try again.';
          console.error('Error loading products:', err);
        }
      });
  }

  /**
   * Subscribe to products from service
   */
  private subscribeToProducts(): void {
    this.productService.products$
      .pipe(takeUntil(this.destroy$))
      .subscribe(products => {
        this.products = products;
        const uniqueCategories = new Map<number, string>();
        products.forEach(p => {
          if (p.categoryId && p.categoryName) {
            uniqueCategories.set(p.categoryId, p.categoryName);
          }
        });
        this.categories = Array.from(uniqueCategories.entries()).map(([id, name]) => ({ id, name }));
        this.featuredCategories = [{ id: null, name: 'Tất cả' }, ...this.categories];
        if (!this.selectedProduct && products.length > 0) {
          this.selectedProduct = products[0];
        }
        if (this.selectedProduct && products.length > 0) {
          const matched = products.find((p) => p.id === this.selectedProduct?.id);
          if (matched) {
            this.selectedProduct = matched;
          }
        }
      });

    this.productService.loading$
      .pipe(takeUntil(this.destroy$))
      .subscribe(loading => {
        this.loading = loading;
      });
  }

  /**
   * Handle filter change
   */
  onFilterChanged(filter: ProductFilterModel): void {
    this.currentFilter = filter;
    this.loadProducts();
  }

  /**
   * Handle storefront category chip click
   */
  onCategoryChipChange(category: { id: number | null; name: string }): void {
    this.selectedCategory = category.id;
    this.currentFilter.pageIndex = 1;
    this.currentFilter.categoryId = category.id ?? undefined;
    this.loadProducts();
  }

  /**
   * Handle search input for storefront
   */
  onKeywordChange(keyword: string): void {
    this.currentFilter.keyword = keyword?.trim() ? keyword.trim() : undefined;
    this.currentFilter.pageIndex = 1;
    this.loadProducts();
  }

  /**
   * Handle storefront sort dropdown
   */
  onSortChange(sort: 'popular' | 'priceAsc' | 'priceDesc' | 'newest'): void {
    this.sortValue = sort;
    this.currentFilter.pageIndex = 1;

    switch (sort) {
      case 'priceAsc':
        this.currentFilter.sortBy = 'price';
        this.currentFilter.sortOrder = 'asc';
        break;
      case 'priceDesc':
        this.currentFilter.sortBy = 'price';
        this.currentFilter.sortOrder = 'desc';
        break;
      case 'newest':
        this.currentFilter.sortBy = 'createdAt';
        this.currentFilter.sortOrder = 'desc';
        break;
      default:
        this.currentFilter.sortBy = 'name';
        this.currentFilter.sortOrder = 'asc';
        break;
    }

    this.loadProducts();
  }

  /**
   * Handle filter reset
   */
  onFilterReset(): void {
    this.currentFilter = new ProductFilterModel();
    this.selectedCategory = null;
    this.sortValue = 'popular';
    this.loadProducts();
  }

  /**
   * Handle product edit
   */
  onProductEdit(product: IProduct): void {
    this.router.navigate(['/product-manager/edit', product.id]);
  }

  /**
   * Handle product delete
   */
  onProductDelete(productId: string | number): void {
    this.productService
      .deleteProduct(productId)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.loadProducts();
        },
        error: (err) => {
          this.error = 'Failed to delete product.';
          console.error('Error deleting product:', err);
        }
      });
  }

  /**
   * Handle product view
   */
  onProductView(product: IProduct): void {
    this.selectedProduct = product;
  }

  /**
   * Add selected product to cart (UI state)
   */
  onProductAddToCart(product: IProduct): void {
    this.cartCount += 1;
    this.selectedProduct = product;
  }

  /**
   * Navigate to create product
   */
  createProduct(): void {
    this.router.navigate(['/product-manager/create']);
  }

  /**
   * Get empty message
   */
  getEmptyMessage(): string {
    if (this.currentFilter.hasFilters()) {
      return 'No products found matching your filters.';
    }
    return 'No products available.';
  }

  /**
   * TrackBy function for ngFor optimization
   */
  trackByProductId(index: number, product: IProduct): string | number {
    return product.id;
  }

  /**
   * Navigate to previous page
   */
  previousPage(): void {
    if (this.currentFilter.pageIndex > 1) {
      const newFilter = new ProductFilterModel({
        ...this.currentFilter,
        pageIndex: this.currentFilter.pageIndex - 1
      });
      this.onFilterChanged(newFilter);
    }
  }

  /**
   * Navigate to specific page
   */
  goToPage(page: number): void {
    const newFilter = new ProductFilterModel({
      ...this.currentFilter,
      pageIndex: page
    });
    this.onFilterChanged(newFilter);
  }

  /**
   * Navigate to next page
   */
  nextPage(): void {
    if (this.currentFilter.pageIndex < this.totalPages) {
      const newFilter = new ProductFilterModel({
        ...this.currentFilter,
        pageIndex: this.currentFilter.pageIndex + 1
      });
      this.onFilterChanged(newFilter);
    }
  }
}
