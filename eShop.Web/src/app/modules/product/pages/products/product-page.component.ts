import { Component, OnInit, OnDestroy, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Subject, takeUntil } from 'rxjs';

import { ProductService } from '../../../product-manager/services/product.service';
import { IProduct, IProductVariant, ProductResponseDto } from '../../../product-manager/models/product.model';
import { ProductFilterModel } from '../../../product-manager/models/product-filter.model';
import { CategoryService } from '../../services/Category.service';
import { ICategory } from '../../../../core/models/category.model';

@Component({
  selector: 'app-product-page',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './product-page.component.html',
  styleUrls: ['./product-page.component.scss']
})
export class ProductPageComponent implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();

  // ── State signals ─────────────────────────────────────────────────────────
  products = signal<IProduct[]>([]);
  loading = signal<boolean>(false);
  totalCount = signal<number>(0);
  totalPages = signal<number>(0);
  currentPage = signal<number>(1);

  // ── Filter state ──────────────────────────────────────────────────────────
  filter = new ProductFilterModel({ pageIndex: 1, pageSize: 12 });
  minPrice: number | null = null;
  maxPrice: number | null = null;
  selectedSort = 'name_asc';

  // ── Category sidebar (static list; extend with API call if needed) ─────────

  selectedCategories = signal<string[]>([]);

  //  Computed 
  hasProducts = computed(() => this.products().length > 0);

  categories = this.categoryService.categories;

  constructor(private productService: ProductService, private categoryService: CategoryService) { }

  ngOnInit(): void {
    this.loadProducts();
    this.categoryService.loadCategory();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }


  loadProducts(): void {
    this.loading.set(true);
    this.productService.getProducts(this.filter)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (response: ProductResponseDto) => {
          this.products.set(response.items);
          this.totalCount.set(response.totalCount);
          this.totalPages.set(response.totalPages);
          this.currentPage.set(response.page);
          this.loading.set(false);
        },
        error: () => this.loading.set(false)
      });
  }


  // ── Filter / Sort handlers 

  onSortChange(value: string): void {
    this.selectedSort = value;
    const [sortBy, sortOrder] = value.split('_');
    this.filter.sortBy = sortBy;
    this.filter.sortOrder = sortOrder as 'asc' | 'desc';
    this.filter.pageIndex = 1;
    this.loadProducts();
  }

  onCategoryToggle(categoryName: string): void {
    const current = this.selectedCategories();
    const idx = current.indexOf(categoryName);
    this.selectedCategories.set(
      idx > -1 ? current.filter(c => c !== categoryName) : [...current, categoryName]
    );
    const selected = this.selectedCategories();
    this.filter.categoryId = selected.length > 0 ? selected[0] : undefined;
    this.filter.pageIndex = 1;
    this.loadProducts();
  }

  isCategorySelected(name: string): boolean {
    return this.selectedCategories().includes(name);
  }

  onPriceFilter(): void {
    this.filter.minPrice = this.minPrice ?? undefined;
    this.filter.maxPrice = this.maxPrice ?? undefined;
    this.filter.pageIndex = 1;
    this.loadProducts();
  }

  goToPage(page: number): void {
    if (page < 1 || page > this.totalPages()) return;
    this.filter.pageIndex = page;
    this.currentPage.set(page);
    this.loadProducts();
  }

  getPaginationRange(): number[] {
    const total = this.totalPages();
    const current = this.currentPage();
    const delta = 2;
    const range: number[] = [];
    const result: number[] = [];
    let last: number | undefined;

    for (let i = 1; i <= total; i++) {
      if (i === 1 || i === total || (i >= current - delta && i <= current + delta)) {
        range.push(i);
      }
    }
    for (const i of range) {
      if (last !== undefined) {
        if (i - last === 2) result.push(last + 1);
        else if (i - last !== 1) result.push(-1); // -1 = dots
      }
      result.push(i);
      last = i;
    }
    return result;
  }



  /** Lấy danh sách màu unique từ variants (ưu tiên cột Color tĩnh, fallback attribute 'Color'). */
  getVariantColors(product: IProduct): string[] {
    if (!product.variants?.length) return [];
    const colors = new Set<string>();

    for (const v of product.variants) {
      // 1. Dùng cột tĩnh ProductVariant.Color nếu có
      if (v.color) {
        colors.add(v.color);
        continue;
      }
      // 2. Tìm trong dynamic attributes
      const colorAttr = v.attributes?.find(
        a => a.attributeName.toLowerCase() === 'color' || a.attributeType.toLowerCase() === 'color'
      );
      if (colorAttr?.displayValue) colors.add(colorAttr.displayValue);
    }
    return [...colors];
  }

  /** Lấy danh sách size unique từ variants. */
  getVariantSizes(product: IProduct): string[] {
    if (!product.variants?.length) return [];
    const sizes = new Set<string>();

    for (const v of product.variants) {
      // 1. Dùng cột tĩnh ProductVariant.Size nếu có
      if (v.size) {
        sizes.add(v.size);
        continue;
      }
      // 2. Tìm trong dynamic attributes
      const sizeAttr = v.attributes?.find(
        a => a.attributeName.toLowerCase() === 'size'
      );
      if (sizeAttr?.displayValue) sizes.add(sizeAttr.displayValue);
    }
    return [...sizes];
  }

  /** Price range: BasePrice + min PriceAdjustment nếu có variants. */
  getMinPrice(product: IProduct): number {
    if (!product.variants?.length) return product.price;
    const min = Math.min(...product.variants.map(v => v.priceAdjustment));
    return product.price + min;
  }

  getMaxPrice(product: IProduct): number {
    if (!product.variants?.length) return product.price;
    const max = Math.max(...product.variants.map(v => v.priceAdjustment));
    return product.price + max;
  }

  hasPriceRange(product: IProduct): boolean {
    return this.getMinPrice(product) !== this.getMaxPrice(product);
  }

  /** Tổng stock từ tất cả variants. */
  getTotalStock(product: IProduct): number {
    if (!product.variants?.length) return product.stock ?? 0;
    return product.variants.reduce((sum, v) => sum + v.stockQuantity, 0);
  }

  /** Kiểm tra màu có phải CSS color value (hex, rgb...) hay tên màu. */
  isColorCode(color: string): boolean {
    return color.startsWith('#') || color.startsWith('rgb') || color.startsWith('hsl');
  }

  /** Chuyển tên màu thành CSS color. */
  colorToCss(color: string): string {
    if (this.isColorCode(color)) return color;
    // Map tên phổ biến → CSS
    const map: Record<string, string> = {
      'red': '#ef4444', 'blue': '#3b82f6', 'green': '#22c55e',
      'black': '#18181b', 'white': '#f9fafb', 'gray': '#71717a',
      'yellow': '#eab308', 'pink': '#ec4899', 'purple': '#a855f7',
      'orange': '#f97316', 'brown': '#92400e', 'navy': '#1e3a8a',
      'silver': '#94a3b8', 'gold': '#d97706', 'beige': '#e9d5b5',
    };
    return map[color.toLowerCase()] ?? '#71717a';
  }

  getProductImage(product: IProduct): string {
    return product.image
      ?? 'https://images.unsplash.com/photo-1523275335684-37898b6baf30?auto=format&fit=crop&q=80&w=800';
  }

  getSkeletonItems(): number[] {
    return Array.from({ length: 12 }, (_, i) => i);
  }
}
