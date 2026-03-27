import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ProductService } from '../services/product.service';
import { ProductDto } from '../models/index';

@Component({
  selector: 'app-products',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="container mt-5">
      <h2>Products</h2>
      <div *ngIf="loading" class="text-center">
        <div class="spinner-border" role="status">
          <span class="visually-hidden">Loading...</span>
        </div>
      </div>
      <div *ngIf="error" class="alert alert-danger">{{ error }}</div>
      <div class="row" *ngIf="!loading && products.length > 0">
        <div class="col-md-4 mb-4" *ngFor="let product of products">
          <div class="card h-100">
            <img [src]="product.imageUrl" class="card-img-top" [alt]="product.name">
            <div class="card-body">
              <h5 class="card-title">{{ product.name }}</h5>
              <p class="card-text">{{ product.description }}</p>
              <p class="card-text"><strong>\${{ product.price }}</strong></p>
              <p class="card-text" *ngIf="product.stock > 0">
                <small class="text-muted">Stock: {{ product.stock }}</small>
              </p>
              <p class="card-text" *ngIf="product.stock === 0">
                <small class="text-danger">Out of stock</small>
              </p>
            </div>
            <div class="card-footer">
              <button class="btn btn-primary w-100" [disabled]="!product.isAvailable">
                Add to Cart
              </button>
            </div>
          </div>
        </div>
      </div>
      <div *ngIf="!loading && products.length === 0" class="alert alert-info">
        No products available
      </div>
    </div>
  `,
  styles: [`
    .card {
      box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
      transition: transform 0.2s;
    }
    .card:hover {
      transform: translateY(-5px);
    }
  `]
})
export class ProductsComponent implements OnInit {
  products: ProductDto[] = [];
  loading = false;
  error = '';

  constructor(private productService: ProductService) { }

  ngOnInit(): void {
    this.loadProducts();
  }

  loadProducts(): void {
    this.loading = true;
    this.error = '';
    this.productService.getProducts().subscribe({
      next: (data) => {
        this.products = data;
        this.loading = false;
      },
      error: (error) => {
        this.error = 'Failed to load products';
        this.loading = false;
      }
    });
  }
}
