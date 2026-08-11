import {
  Component, OnInit, OnDestroy, signal, computed,
  HostListener
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { Subject, takeUntil, finalize } from 'rxjs';

import { ProductService } from '../../../product-manager/services/product.service';
import { IProduct, IProductVariant, IVariantAttribute, ProductResponseDto } from '../../../product-manager/models/product.model';
import { ProductFilterModel } from '../../../product-manager/models/product-filter.model';
import { CategoryService } from '../../services/category.service';
import { CartService } from '../../../cart/services/cart.service';
import { environment } from '../../../../../environments/environment';

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

  // ── Product data ───────────────────────────────────────────────────────
  products = signal<IProduct[]>([]);
  loading = signal<boolean>(false);
  totalCount = signal<number>(0);
  totalPages = signal<number>(0);
  currentPage = signal<number>(1);

  filter = new ProductFilterModel({ pageIndex: 1, pageSize: 12, isActive: undefined });
  minPrice: number | null = null;
  maxPrice: number | null = null;
  selectedSort = 'name_asc';
  selectedCategoryId = signal<number | null>(null);

  hasProducts = computed(() => this.products().length > 0);
  categories = this.categoryService.categories;

  // ── Cart ──────────────────────────────────────────────────────────────
  private selectedQuantityMap = signal<Record<number | string, number>>({});
  addingToCart = signal<Record<number | string, boolean>>({});
  cartTotalItems = this.cartService.totalItems;

  // ── Toast ─────────────────────────────────────────────────────────────
  toasts = signal<ToastMessage[]>([]);
  private toastIdCounter = 0;

  // ── Filter Dropdown ───────────────────────────────────────────────────
  activeDropdown = signal<string | null>(null);
  private dropdownCloseTimer: ReturnType<typeof setTimeout> | null = null;

  // ── Variant Quick-Add Modal ───────────────────────────────────────────
  quickAddProduct = signal<IProduct | null>(null);
  selectedColor = signal<string | null>(null);
  selectedSize = signal<string | null>(null);
  popupQuantity = signal<number>(1);
  isAddingToCartModal = signal(false);

  /** All unique colors of modal product */
  modalColors = computed<string[]>(() => {
    const p = this.quickAddProduct();
    if (!p?.variants?.length) return [];
    const colors = new Set<string>();
    for (const v of p.variants) {
      const c = this.extractAttr(v, 'color');
      if (c) colors.add(c);
    }
    return [...colors];
  });

  /** All sizes with availability flag based on selected color */
  modalSizes = computed<{ value: string; available: boolean }[]>(() => {
    const p = this.quickAddProduct();
    if (!p?.variants?.length) return [];
    const color = this.selectedColor();
    const sizeMap = new Map<string, boolean>();
    for (const v of p.variants) {
      const s = this.extractAttr(v, 'size');
      if (!s) continue;
      const c = this.extractAttr(v, 'color');
      const colorMatch = !color || c === color;
      const inStock = v.stockQuantity > 0;
      if (!sizeMap.has(s)) sizeMap.set(s, false);
      if (colorMatch && inStock) sizeMap.set(s, true);
    }
    return [...sizeMap.entries()].map(([value, available]) => ({ value, available }));
  });

  /** Variant matching selected color + size */
  selectedVariant = computed<IProductVariant | null>(() => {
    const p = this.quickAddProduct();
    if (!p?.variants?.length) return null;
    const color = this.selectedColor();
    const size = this.selectedSize();
    return p.variants.find(v => {
      const c = this.extractAttr(v, 'color');
      const s = this.extractAttr(v, 'size');
      const colorOk = !color || c === color;
      const sizeOk = !size || s === size;
      return colorOk && sizeOk && v.stockQuantity > 0;
    }) ?? null;
  });

  /** Displayed price in modal */
  modalPrice = computed<string>(() => {
    const p = this.quickAddProduct();
    if (!p) return '';
    const variant = this.selectedVariant();
    if (variant) return this.formatPrice(p.price + variant.priceAdjustment);
    if (!p.variants?.length) return this.formatPrice(p.price);
    const min = this.getMinPrice(p);
    const max = this.getMaxPrice(p);
    if (min === max) return this.formatPrice(min);
    return `${this.formatPrice(min)} – ${this.formatPrice(max)}`;
  });

  /** Max qty for modal (from selected variant or total) */
  modalMaxStock = computed<number>(() => {
    const variant = this.selectedVariant();
    if (variant) return variant.stockQuantity;
    const p = this.quickAddProduct();
    if (!p) return 0;
    return this.getTotalStock(p);
  });

  /** Whether modal CTA is enabled */
  canAddToCart = computed<boolean>(() => {
    const p = this.quickAddProduct();
    if (!p?.variants?.length) return false;
    const hasColors = this.modalColors().length > 0;
    const hasSizes = this.modalSizes().length > 0;
    const colorOk = !hasColors || !!this.selectedColor();
    const sizeOk = !hasSizes || !!this.selectedSize();
    return colorOk && sizeOk && !!this.selectedVariant();
  });

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
    document.body.style.overflow = '';
  }

  // ── Close dropdown when clicking outside ──────────────────────────────
  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent): void {
    const target = event.target as HTMLElement;
    if (!target.closest('.filter-item')) {
      this.activeDropdown.set(null);
    }
  }

  // ── Products ──────────────────────────────────────────────────────────
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
    this.activeDropdown.set(null);
    this.loadProducts();
  }

  onCategoryToggle(categoryId: number): void {
    const current = this.selectedCategoryId();
    // Click cùng danh mục đang chọn → bỏ chọn (xem tất cả)
    const next = current === categoryId ? null : categoryId;
    this.selectedCategoryId.set(next);
    this.filter.categoryId = next ?? undefined;
    this.filter.pageIndex = 1;
    this.loadProducts();
  }

  isCategorySelected(id: number): boolean {
    return this.selectedCategoryId() === id;
  }

  onPriceFilter(): void {
    this.filter.minPrice = this.minPrice ?? undefined;
    this.filter.maxPrice = this.maxPrice ?? undefined;
    this.filter.pageIndex = 1;
    this.activeDropdown.set(null);
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

  getSortLabel(): string {
    const map: Record<string, string> = {
      'name_asc': 'Tên A–Z',
      'name_desc': 'Tên Z–A',
      'price_asc': 'Giá thấp → cao',
      'price_desc': 'Giá cao → thấp',
      'createdAt_desc': 'Mới nhất',
    };
    return map[this.selectedSort] ?? 'Sắp xếp';
  }

  // ── Filter Dropdown ───────────────────────────────────────────────────
  openDropdown(name: string, event: MouseEvent): void {
    event.stopPropagation();
    if (this.dropdownCloseTimer) {
      clearTimeout(this.dropdownCloseTimer);
      this.dropdownCloseTimer = null;
    }
    this.activeDropdown.set(name);
  }

  scheduleCloseDropdown(): void {
    this.dropdownCloseTimer = setTimeout(() => {
      this.activeDropdown.set(null);
    }, 180);
  }

  cancelCloseDropdown(): void {
    if (this.dropdownCloseTimer) {
      clearTimeout(this.dropdownCloseTimer);
      this.dropdownCloseTimer = null;
    }
  }

  isDropdownOpen(name: string): boolean {
    return this.activeDropdown() === name;
  }

  // ── Variant helpers ────────────────────────────────────────────────────
  private extractAttr(variant: IProductVariant, type: string): string | null {
    if (type === 'color' && variant.color) return variant.color;
    if (type === 'size' && variant.size) return variant.size;
    const attr = variant.attributes?.find(
      a => a.attributeName.toLowerCase() === type || a.attributeType.toLowerCase() === type
    );
    return attr?.displayValue ?? null;
  }

  getVariantColors(product: IProduct): string[] {
    if (!product.variants?.length) return [];
    const colors = new Set<string>();
    for (const v of product.variants) {
      const c = this.extractAttr(v, 'color');
      if (c) colors.add(c);
    }
    return [...colors];
  }

  getVariantSizes(product: IProduct): string[] {
    if (!product.variants?.length) return [];
    const sizes = new Set<string>();
    for (const v of product.variants) {
      const s = this.extractAttr(v, 'size');
      if (s) sizes.add(s);
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

  getProductImage(product: any): string {
    const relativeUrl = product.imageUrls?.[0] || product.image;
    if (relativeUrl) {
      if (relativeUrl.startsWith('http')) return relativeUrl;
      const baseUrl = environment.apiBaseUrl.replace(/\/$/, '');
      const path = relativeUrl.startsWith('/') ? relativeUrl : `/${relativeUrl}`;
      return `${baseUrl}${path}`;
    }
    return 'https://images.unsplash.com/photo-1523275335684-37898b6baf30?auto=format&fit=crop&q=80&w=800';
  }

  formatPrice(value: number): string {
    return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(value);
  }

  getSkeletonItems(): number[] {
    return Array.from({ length: 12 }, (_, i) => i);
  }

  // ── Quick-Add Modal ───────────────────────────────────────────────────
  openQuickAdd(product: IProduct, event?: Event): void {
    if (event) event.stopPropagation();
    this.selectedColor.set(null);
    this.selectedSize.set(null);
    this.popupQuantity.set(1);
    this.quickAddProduct.set(product);
    document.body.style.overflow = 'hidden';
  }

  closeQuickAdd(): void {
    this.quickAddProduct.set(null);
    document.body.style.overflow = '';
  }

  closeQuickAddOnBackdrop(event: MouseEvent): void {
    if ((event.target as HTMLElement).classList.contains('modal-backdrop')) {
      this.closeQuickAdd();
    }
  }

  selectModalColor(color: string): void {
    this.selectedColor.set(color);
    const sizes = this.modalSizes();
    const current = this.selectedSize();
    if (current) {
      const stillAvail = sizes.find(s => s.value === current && s.available);
      if (!stillAvail) this.selectedSize.set(null);
    }
  }

  selectModalSize(size: string, available: boolean): void {
    if (!available) return;
    this.selectedSize.set(size);
  }

  increasePopupQty(): void {
    if (this.popupQuantity() < this.modalMaxStock()) {
      this.popupQuantity.update(q => q + 1);
    }
  }

  decreasePopupQty(): void {
    if (this.popupQuantity() > 1) {
      this.popupQuantity.update(q => q - 1);
    }
  }

  onModalAddToCart(): void {
    if (!this.canAddToCart() || this.isAddingToCartModal()) return;
    const product = this.quickAddProduct();
    if (!product) return;
    const variant = this.selectedVariant();
    if (!variant) return;

    this.isAddingToCartModal.set(true);
    this.cartService.addToCart({ productVariantId: variant.id, quantity: this.popupQuantity() })
      .pipe(
        takeUntil(this.destroy$),
        finalize(() => this.isAddingToCartModal.set(false))
      )
      .subscribe({
        next: (res) => {
          if (res.success) {
            this.showToast('success', `Đã thêm "${product.name}" vào giỏ hàng.`);
            this.closeQuickAdd();
          } else {
            this.showToast('error', this.cartService.getErrorMessage(res.statusCode, res.message));
          }
        },
        error: (err) => {
          const errorCode = err?.statusCode ?? 500;
          this.showToast('error', this.cartService.getErrorMessage(errorCode, err?.message || err?.errors?.[0]));
        }
      });
  }

  // ── Legacy cart helpers (kept for compatibility) ──────────────────────
  getQuantity(product: IProduct): number {
    return this.selectedQuantityMap()[product.id] || 1;
  }

  isAddingToCart(product: IProduct): boolean {
    return !!this.addingToCart()[product.id];
  }

  // ── Toast ─────────────────────────────────────────────────────────────
  private showToast(type: 'success' | 'error', text: string): void {
    const id = ++this.toastIdCounter;
    this.toasts.update(t => [...t, { id, type, text }]);
    setTimeout(() => this.dismissToast(id), 3000);
  }

  dismissToast(id: number): void {
    this.toasts.update(t => t.filter(x => x.id !== id));
  }
}
