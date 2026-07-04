import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ProductService } from '../../services/product.service';
import { IProduct, ProductResponseDto } from '../../../product-manager/models/product.model';
import { ProductFilterModel } from '../../../product-manager/models/product-filter.model';

@Component({
  selector: 'app-products',
  standalone: true,
  imports: [CommonModule],
  template: `
    <section class="page-shell catalog">
      <div class="catalog__header">
        <h2 class="section-title">Products</h2>
        <p class="section-copy">A stripped-down catalog shell with only custom styling.</p>
      </div>

      <div *ngIf="loading" class="catalog__state">Loading...</div>
      <div *ngIf="error" class="catalog__state catalog__state--error">{{ error }}</div>

      <div class="app-grid app-grid--cards" *ngIf="!loading && products.length > 0">
        <article class="catalog-card surface" *ngFor="let product of products">
          <h3 class="catalog-card__title">{{ product.name }}</h3>
          <p class="catalog-card__copy">{{ product.description || 'No description' }}</p>
          <div class="catalog-card__meta">
            <span>{{ product.categoryName }}</span>
            <strong>{{ product.price | currency:'USD':'symbol':'1.0-0' }}</strong>
          </div>
          <button class="app-button" [disabled]="product.stock === 0">Add to Cart</button>
        </article>
      </div>

      <div *ngIf="!loading && products.length === 0" class="catalog__state">
        No products available
      </div>
    </section>
  `,
  styles: [
    `
      .catalog {
        padding-block: 3rem 4rem;
      }

      .catalog__header {
        display: grid;
        gap: 0.5rem;
        margin-block-end: 1.5rem;
      }

      .catalog__state {
        padding: 1rem 1.25rem;
        border-radius: var(--radius-md);
        background: rgba(19, 28, 39, 0.92);
        border: 1px solid var(--color-border);
        color: var(--color-text-secondary);
      }

      .catalog__state--error {
        color: var(--color-error);
      }

      .catalog-card {
        padding: 1.25rem;
        display: grid;
        gap: 0.75rem;
      }

      .catalog-card__title {
        margin: 0;
        font-size: 1.1rem;
      }

      .catalog-card__copy {
        margin: 0;
        color: var(--color-text-secondary);
      }

      .catalog-card__meta {
        display: flex;
        align-items: center;
        justify-content: space-between;
        gap: 1rem;
        color: var(--color-text-secondary);
      }
    `
  ]
})
export class ProductsComponent implements OnInit {
  products: IProduct[] = [];
  loading = false;
  error = '';

  constructor(private productService: ProductService) { }

  ngOnInit(): void {
    this.loadProducts();
  }

  loadProducts(): void {
    this.loading = true;
    this.error = '';
    const filter = new ProductFilterModel({ pageIndex: 1, pageSize: 20 });
    this.productService.getProducts(filter).subscribe({
      next: (data: ProductResponseDto) => {
        this.products = data.items;
        this.loading = false;
      },
      error: (error) => {
        this.error = 'Failed to load products';
        this.loading = false;
      }
    });
  }
}
