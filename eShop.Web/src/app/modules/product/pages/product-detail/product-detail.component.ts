import { Component, OnInit, OnDestroy, signal, computed, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { Subject, takeUntil, finalize } from 'rxjs';
import { ProductService } from '../../services/product.service';
import { IProduct, IProductVariant, IVariantAttribute } from '../../../product-manager/models/product.model';
import { CartService } from '../../../cart/services/cart.service';

interface ToastMessage {
  id: number;
  type: 'success' | 'error';
  text: string;
}

@Component({
  selector: 'app-product-detail',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './product-detail.component.html',
  styleUrls: ['./product-detail.component.scss']
})
export class ProductDetailComponent implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();
  private route = inject(ActivatedRoute);
  private productService = inject(ProductService);
  private cartService = inject(CartService);

  // ── State ──────────────────────────────────────────────────────────
  product = signal<IProduct | null>(null);
  loading = signal<boolean>(false);
  error = signal<string | null>(null);

  selectedVariant = signal<IProductVariant | null>(null);
  selectedAttributes = signal<Record<string, string>>({});
  quantity = signal<number>(1);
  addingToCart = signal<boolean>(false);

  showVariantPanel = signal<boolean>(false);
  activeAccordion = signal<string | null>('description');

  activeImageIndex = signal<number>(0);

  toasts = signal<ToastMessage[]>([]);
  private toastIdCounter = 0;

  // ── Computed ────────────────────────────────────────────────────────
  finalPrice = computed(() => {
    const p = this.product();
    const v = this.selectedVariant();
    if (!p) return 0;
    return p.price + (v?.priceAdjustment ?? 0);
  });

  stockAvailable = computed(() => {
    const v = this.selectedVariant();
    const p = this.product();
    if (v) return v.stockQuantity;
    return p?.stock ?? 0;
  });

  isInStock = computed(() => this.stockAvailable() > 0);

  attributeGroups = computed(() => {
    const p = this.product();
    if (!p?.variants?.length) return [];
    const groups: Record<string, Set<string>> = {};
    for (const v of p.variants) {
      for (const a of v.attributes ?? []) {
        if (!groups[a.attributeName]) groups[a.attributeName] = new Set();
        groups[a.attributeName].add(a.displayValue);
      }
    }
    return Object.entries(groups).map(([name, values]) => ({ name, values: [...values] }));
  });

  productImages = computed(() => {
    const p = this.product();
    const v = this.selectedVariant();
    
    // 1. Variant image (if available)
    if (v?.imageUrls && v.imageUrls.length > 0) return v.imageUrls;

    // Fallback editorial images per product type
    const fallbacks = [
      'https://images.unsplash.com/photo-1523275335684-37898b6baf30?auto=format&fit=crop&q=80&w=800',
      'https://images.unsplash.com/photo-1585386959984-a4155224a1ad?auto=format&fit=crop&q=80&w=800',
      'https://images.unsplash.com/photo-1556228578-8c89e6adf883?auto=format&fit=crop&q=80&w=800',
      'https://images.unsplash.com/photo-1491553895911-0055eca6402d?auto=format&fit=crop&q=80&w=800',
    ];
    // 2. Product images
    if (p?.imageUrls && p.imageUrls.length > 0) return p.imageUrls;
    // 3. Fallback
    if (p?.image) return [p.image, ...fallbacks.slice(0, 3)];
    return fallbacks;
  });

  // ── Lifecycle ───────────────────────────────────────────────────────
  ngOnInit(): void {
    this.route.params.pipe(takeUntil(this.destroy$)).subscribe(params => {
      const id = params['id'];
      if (id) this.loadProduct(id);
    });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  // ── Data loading ────────────────────────────────────────────────────
  private loadProduct(id: string | number): void {
    this.loading.set(true);
    this.error.set(null);
    this.productService.getProductById(id)
      .pipe(takeUntil(this.destroy$), finalize(() => this.loading.set(false)))
      .subscribe({
        next: (product) => {
          this.product.set(product);
          // Auto-select first in-stock variant
          const firstInStock = product.variants?.find(v => v.stockQuantity > 0) ?? product.variants?.[0] ?? null;
          this.selectedVariant.set(firstInStock);
          if (firstInStock) this.syncAttributeSelection(firstInStock);
        },
        error: () => this.error.set('Không tìm thấy sản phẩm.')
      });
  }

  // ── Variant Selection ───────────────────────────────────────────────
  private syncAttributeSelection(variant: IProductVariant): void {
    const attrs: Record<string, string> = {};
    for (const a of variant.attributes ?? []) {
      attrs[a.attributeName] = a.displayValue;
    }
    this.selectedAttributes.set(attrs);
  }

  selectAttributeValue(groupName: string, value: string): void {
    this.selectedAttributes.update(prev => ({ ...prev, [groupName]: value }));
    this.matchVariantToSelection();
  }

  private matchVariantToSelection(): void {
    const p = this.product();
    if (!p?.variants?.length) return;
    const selected = this.selectedAttributes();
    const matched = p.variants.find(v =>
      v.attributes.every(a => selected[a.attributeName] === a.displayValue)
    );
    this.selectedVariant.set(matched ?? null);
    this.quantity.set(1);
  }

  isAttributeSelected(groupName: string, value: string): boolean {
    return this.selectedAttributes()[groupName] === value;
  }

  isAttributeAvailable(groupName: string, value: string): boolean {
    const p = this.product();
    if (!p?.variants?.length) return true;
    const current = this.selectedAttributes();
    return p.variants.some(v =>
      v.attributes.some(a => a.attributeName === groupName && a.displayValue === value) &&
      Object.entries(current)
        .filter(([k]) => k !== groupName)
        .every(([k, val]) => v.attributes.some(a => a.attributeName === k && a.displayValue === val))
    );
  }

  selectVariantDirectly(variant: IProductVariant): void {
    this.selectedVariant.set(variant);
    this.syncAttributeSelection(variant);
    this.showVariantPanel.set(false);
    this.quantity.set(1);
  }

  // ── Color helpers ────────────────────────────────────────────────────
  isColorGroup(groupName: string): boolean {
    return groupName.toLowerCase().includes('color') || groupName.toLowerCase().includes('màu');
  }

  colorToCss(value: string): string {
    if (value.startsWith('#') || value.startsWith('rgb') || value.startsWith('hsl')) return value;
    const map: Record<string, string> = {
      'red': '#ef4444', 'blue': '#3b82f6', 'green': '#22c55e', 'black': '#18181b',
      'white': '#f9fafb', 'gray': '#71717a', 'grey': '#71717a', 'yellow': '#eab308',
      'pink': '#ec4899', 'purple': '#a855f7', 'orange': '#f97316', 'brown': '#92400e',
      'navy': '#1e3a8a', 'silver': '#94a3b8', 'gold': '#d97706', 'beige': '#e9d5b5',
      'slate': '#475569', 'teal': '#14b8a6', 'cyan': '#06b6d4', 'indigo': '#6366f1',
      'rose': '#f43f5e', 'amber': '#f59e0b', 'lime': '#84cc16', 'emerald': '#10b981',
      'đen': '#18181b', 'trắng': '#f9fafb', 'đỏ': '#ef4444', 'xanh': '#3b82f6',
      'vàng': '#eab308', 'hồng': '#ec4899', 'cam': '#f97316', 'nâu': '#92400e',
    };
    return map[value.toLowerCase()] ?? '#94a3b8';
  }

  // ── Quantity ──────────────────────────────────────────────────────────
  increaseQty(): void {
    const max = this.stockAvailable();
    if (this.quantity() < max) this.quantity.update(q => q + 1);
  }

  decreaseQty(): void {
    if (this.quantity() > 1) this.quantity.update(q => q - 1);
  }

  // ── Cart ──────────────────────────────────────────────────────────────
  onAddToCart(): void {
    const variant = this.selectedVariant();
    if (!variant || this.addingToCart()) return;
    if (variant.stockQuantity <= 0) {
      this.showToast('error', 'Sản phẩm này đã hết hàng.');
      return;
    }
    this.addingToCart.set(true);
    this.cartService.addToCart({ productVariantId: variant.id, quantity: this.quantity() })
      .pipe(takeUntil(this.destroy$), finalize(() => this.addingToCart.set(false)))
      .subscribe({
        next: (res) => {
          if (res.success) {
            this.showToast('success', `Đã thêm vào giỏ hàng thành công.`);
          } else {
            this.showToast('error', this.cartService.getErrorMessage(res.statusCode, res.message));
          }
        },
        error: (err) => {
          this.showToast('error', this.cartService.getErrorMessage(err?.statusCode ?? 500, err?.message || err?.errors?.[0]));
        }
      });
  }

  // ── Accordion ─────────────────────────────────────────────────────────
  toggleAccordion(key: string): void {
    this.activeAccordion.update(cur => cur === key ? null : key);
  }

  isAccordionOpen(key: string): boolean {
    return this.activeAccordion() === key;
  }

  // ── Image Gallery ─────────────────────────────────────────────────────
  setActiveImage(index: number): void {
    this.activeImageIndex.set(index);
  }

  // ── Toast ─────────────────────────────────────────────────────────────
  private showToast(type: 'success' | 'error', text: string): void {
    const id = ++this.toastIdCounter;
    this.toasts.update(t => [...t, { id, type, text }]);
    setTimeout(() => this.dismissToast(id), 3500);
  }

  dismissToast(id: number): void {
    this.toasts.update(t => t.filter(x => x.id !== id));
  }

  // ── Price formatter ───────────────────────────────────────────────────
  formatPrice(amount: number): string {
    return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(amount);
  }

  // ── Skeleton helper ───────────────────────────────────────────────────
  getSkeletonItems(count: number): number[] {
    return Array.from({ length: count }, (_, i) => i);
  }
}
