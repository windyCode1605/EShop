import { Component, EventEmitter, Output, Input, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ProductFilterModel } from '../../models/product-filter.model';

/**
 * Product Filter Component (Dumb Component)
 * Handles product filtering UI
 */
@Component({
  selector: 'app-product-filter',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './product-filter.component.html',
  styleUrls: ['./product-filter.component.scss']
})
export class ProductFilterComponent implements OnInit {
  @Input() filter: ProductFilterModel = new ProductFilterModel();
  @Input() categories: any[] = [];
  @Output() filterChanged = new EventEmitter<ProductFilterModel>();
  @Output() filterReset = new EventEmitter<void>();

  ngOnInit(): void {
    // Initialize if needed
  }

  /**
   * Apply filter
   */
  applyFilter(): void {
    this.filter.pageIndex = 1; // Reset to first page
    this.filterChanged.emit(this.filter);
  }

  /**
   * Reset filter to defaults
   */
  resetFilter(): void {
    this.filter.reset();
    this.filterReset.emit();
  }

  /**
   * Handle page change
   */
  onPageChange(page: number): void {
    this.filter.pageIndex = page;
    this.filterChanged.emit(this.filter);
  }

  /**
   * Handle page size change
   */
  onPageSizeChange(size: number): void {
    this.filter.pageSize = size;
    this.filter.pageIndex = 1;
    this.filterChanged.emit(this.filter);
  }

  /**
   * Handle sort change
   */
  onSortChange(sortBy: string, sortOrder: 'asc' | 'desc'): void {
    this.filter.sortBy = sortBy;
    this.filter.sortOrder = sortOrder;
    this.filterChanged.emit(this.filter);
  }
}
