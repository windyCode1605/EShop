import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { IProduct } from '../../models/product.model';

/**
 * Product Item Component (Dumb Component)
 * Displays a single product in a card format
 */
@Component({
  selector: 'app-product-item',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './product-item.component.html',
  styleUrls: ['./product-item.component.scss']
})
export class ProductItemComponent {
  @Input() product!: IProduct;
  @Output() view = new EventEmitter<IProduct>();
  @Output() addToCart = new EventEmitter<IProduct>();

  // Expose Math for template use
  protected Math = Math;

  /**
   * Emit view event
   */
  onView(): void {
    this.view.emit(this.product);
  }

  /**
   * Emit add-to-cart event
   */
  onAddToCart(): void {
    this.addToCart.emit(this.product);
  }

  /**
   * Get product status badge class
   */
  getStatusClass(): string {
    return this.product.isActive ? 'badge-success' : 'badge-secondary';
  }

  /**
   * Get stock status
   */
  getStockStatus(): string {
    if (this.product.stock > 100) return 'In Stock';
    if (this.product.stock > 0) return 'Low Stock';
    return 'Out of Stock';
  }

  /**
   * Get stock status class
   */
  getStockClass(): string {
    if (this.product.stock > 100) return 'text-success';
    if (this.product.stock > 0) return 'text-warning';
    return 'text-danger';
  }
}
