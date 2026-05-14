import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { ProductService } from '../../services/product.service';
import { IProduct } from '../../models/product.model';

/**
 * Product Detail Component (Smart Component)
 * Displays detailed information about a single product
 */
@Component({
  selector: 'app-product-detail',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './product-detail.component.html',
  styleUrls: ['./product-detail.component.scss']
})
export class ProductDetailComponent implements OnInit, OnDestroy {
  product: IProduct | null = null;
  loading: boolean = false;
  error: string | null = null;
  protected Math = Math;

  private destroy$ = new Subject<void>();

  constructor(
    private productService: ProductService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.route.paramMap
      .pipe(takeUntil(this.destroy$))
      .subscribe(params => {
        const id = params.get('id');
        if (id) {
          this.loadProduct(id);
        }
      });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  /**
   * Load product details
   */
  private loadProduct(id: string): void {
    this.loading = true;
    this.error = null;
    this.productService
      .getProductById(id)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (product) => {
          this.product = product;
          this.loading = false;
        },
        error: (err) => {
          this.error = 'Failed to load product details.';
          this.loading = false;
          console.error('Error loading product:', err);
        }
      });
  }

  /**
   * Navigate back to list
   */
  goBack(): void {
    this.router.navigate(['/product-manager']);
  }

  /**
   * Navigate to edit
   */
  editProduct(): void {
    if (this.product) {
      this.router.navigate(['/product-manager/edit', this.product.id]);
    }
  }

  /**
   * Get status badge class
   */
  getStatusClass(): string {
    return this.product?.isActive ? 'badge-success' : 'badge-danger';
  }

  /**
   * Get stock status
   */
  getStockStatus(): string {
    if (!this.product) return '';
    if (this.product.stock > 100) return 'In Stock';
    if (this.product.stock > 0) return 'Low Stock';
    return 'Out of Stock';
  }

  /**
   * Get stock status class
   */
  getStockClass(): string {
    if (!this.product) return '';
    if (this.product.stock > 100) return 'text-success';
    if (this.product.stock > 0) return 'text-warning';
    return 'text-danger';
  }
}
