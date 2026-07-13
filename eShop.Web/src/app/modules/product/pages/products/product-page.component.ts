import { Component, OnInit, OnDestroy, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { Subject, takeUntil, finalize } from 'rxjs';

import { ProductService } from '../../../product-manager/services/product.service';
import { IProduct, IProductVariant, ProductResponseDto } from '../../../product-manager/models/product.model';
import { ProductFilterModel } from '../../../product-manager/models/product-filter.model';
import { CategoryService } from '../../services/category.service';
import { CartService } from '../../../cart/services/cart.service';


interface ToastMessage {
  id: number;
  type: 'success' | 'error';
  text: string;
}

@Component({
  selector: 'app-product-page',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './product-page.component.html',
  styleUrls: ['./product-page.component.scss']
})
export class ProductPageComponent implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();

  products = signal<IProduct[]>([]);
  loading = signal<boolean>(false);
  totalCount = signal<number>(0);
  totalPages = signal<number>(0);
  currentPage = signal<number>(1);

  filter = new ProductFilterModel({ pageIndex: 1, pageSize: 12 });
  minPrice: number | null = null;
  maxPrice: number | null = null;
  selectedSort = 'name_asc';
  selectedCategories = signal<string[]>([]);

  hasProducts = computed(() => this.products().length > 0);
  categories = this.categoryService.categories;


  private selectedQuantityMap = signal<Record<number | string, number>>({});
  addingToCart = signal<Record<number | string, boolean>>({});

  cartTotalItems = this.cartService.totalItems;

  toasts = signal<ToastMessage[]>([]);
  private toastIdCounter = 0;

  constructor(
    private productService: ProductService,
    private categoryService: CategoryService,
    private cartService: CartService
  ) { }

  ngOnInit(): void {
    this.loadProducts();
    this.categoryService.loadCategory();
    this.cartService.getMyCart().subscribe();
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
        else if (i - last !== 1) result.push(-1);
      }
      result.push(i);
      last = i;
    }
    return result;
  }

  // ── Variant display helpers (giữ nguyên) ───────────────────────────────
  getVariantColors(product: IProduct): string[] {
    if (!product.variants?.length) return [];
    const colors = new Set<string>();
    for (const v of product.variants) {
      if (v.color) { colors.add(v.color); continue; }
      const colorAttr = v.attributes?.find(
        a => a.attributeName.toLowerCase() === 'color' || a.attributeType.toLowerCase() === 'color'
      );
      if (colorAttr?.displayValue) colors.add(colorAttr.displayValue);
    }
    return [...colors];
  }

  getVariantSizes(product: IProduct): string[] {
    if (!product.variants?.length) return [];
    const sizes = new Set<string>();
    for (const v of product.variants) {
      if (v.size) { sizes.add(v.size); continue; }
      const sizeAttr = v.attributes?.find(a => a.attributeName.toLowerCase() === 'size');
      if (sizeAttr?.displayValue) sizes.add(sizeAttr.displayValue);
    }
    return [...sizes];
  }

  getMinPrice(product: IProduct): number {
    if (!product.variants?.length) return product.price;
    return product.price + Math.min(...product.variants.map(v => v.priceAdjustment));
  }

  getMaxPrice(product: IProduct): number {
    if (!product.variants?.length) return product.price;
    return product.price + Math.max(...product.variants.map(v => v.priceAdjustment));
  }

  hasPriceRange(product: IProduct): boolean {
    return this.getMinPrice(product) !== this.getMaxPrice(product);
  }

  getTotalStock(product: IProduct): number {
    if (!product.variants?.length) return product.stock ?? 0;
    return product.variants.reduce((sum, v) => sum + v.stockQuantity, 0);
  }

  isColorCode(color: string): boolean {
    return color.startsWith('#') || color.startsWith('rgb') || color.startsWith('hsl');
  }

  colorToCss(color: string): string {
    if (this.isColorCode(color)) return color;
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

  // ── Cart Actions ───────────────────────────────────────────────────
  getQuantity(product: IProduct): number {
    return this.selectedQuantityMap()[product.id] || 1;
  }

  increaseQuantity(product: IProduct, event: Event): void {
    event.stopPropagation();
    const current = this.getQuantity(product);
    const maxStock = this.getTotalStock(product);
    if (current < maxStock) {
      this.selectedQuantityMap.update(m => ({ ...m, [product.id]: current + 1 }));
    }
  }

  decreaseQuantity(product: IProduct, event: Event): void {
    event.stopPropagation();
    const current = this.getQuantity(product);
    if (current > 1) {
      this.selectedQuantityMap.update(m => ({ ...m, [product.id]: current - 1 }));
    }
  }

  isAddingToCart(product: IProduct): boolean {
    return !!this.addingToCart()[product.id];
  }

  // ── Add to cart ──────────────────────────────────────────────────────────
  onAddToCart(product: IProduct, event?: Event): void {
    if (event) event.stopPropagation();

    if (this.isAddingToCart(product)) return; // chống double-click / double-submit

    const variants = product.variants ?? [];
    // Ở trang danh sách, mặc định lấy variant đầu tiên còn hàng
    const variant = variants.length > 0
      ? (variants.find(v => v.stockQuantity > 0) ?? variants[0])
      : null;

    if (!variant) {
      this.showToast('error', 'Sản phẩm này chưa có thông tin phiên bản.');
      return;
    }

    if (variant.stockQuantity <= 0) {
      this.showToast('error', 'Sản phẩm này đã hết hàng.');
      return;
    }

    this.addingToCart.update(m => ({ ...m, [product.id]: true }));
    const quantity = this.getQuantity(product);

    this.cartService.addToCart({ productVariantId: variant.id, quantity })
      .pipe(
        takeUntil(this.destroy$),
        finalize(() => this.addingToCart.update(m => ({ ...m, [product.id]: false })))
      )
      .subscribe({
        next: (res) => {
          if (res.isSuccess) {
            this.showToast('success', `Đã thêm "${product.name}" vào giỏ hàng.`);
          } else {
            this.showToast('error', this.cartService.getErrorMessage(res.errorCode, res.otherData));
          }
        },
        error: (err) => {
          const errorCode = err?.errorCode ?? -1;
          this.showToast('error', this.cartService.getErrorMessage(errorCode, err?.otherData));
        }
      });
  }

  private showToast(type: 'success' | 'error', text: string): void {
    const id = ++this.toastIdCounter;
    this.toasts.update(t => [...t, { id, type, text }]);
    setTimeout(() => this.dismissToast(id), 3000);
  }

  dismissToast(id: number): void {
    this.toasts.update(t => t.filter(x => x.id !== id));
  }
}
