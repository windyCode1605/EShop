import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { ProductService } from '../../services/product.service';
import { IProduct, ProductResponseDto } from '../../../product-manager/models/product.model';
import { ProductFilterModel } from '../../../product-manager/models/product-filter.model';
import { CartService } from '../../../cart/services/cart.service';
import { TokenService } from '../../../../core/service-proxies/token.service';

@Component({
  selector: 'app-products',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="bg-[#F8F7F5] min-h-screen py-12 lg:py-20 px-4 sm:px-6 lg:px-8">
      <div class="max-w-[1280px] mx-auto">
        
        <!-- Header Section: Editorial & High Spatial Tension -->
        <header class="mb-12 md:mb-16">
          <div class="flex flex-col md:flex-row md:items-end justify-between gap-8 pb-8 border-b border-[#E5E3DF]">
            <div class="max-w-2xl">
              <span class="inline-flex items-center gap-2 px-3 py-1 rounded-full text-[10px] font-semibold uppercase tracking-[0.2em] text-[#A8885E] bg-[#C5A882]/10 border border-[#C5A882]/30 mb-4">
                <span class="w-1.5 h-1.5 rounded-full bg-[#C5A882]"></span>
                BỘ SƯU TẬP 2025
              </span>
              <h1 class="text-3xl sm:text-4xl lg:text-5xl font-bold text-[#1A1917] tracking-tight mb-4 font-serif italic">
                Sản Phẩm <em class="not-italic font-sans font-semibold text-[#C5A882]">Đặc Sắc</em>
              </h1>
              <p class="text-[#6B6864] text-base sm:text-lg leading-relaxed">
                Tuyển tập những sáng tác độc bản, chế tác tinh xảo dành cho phong cách sống hiện đại.
              </p>
            </div>

            <!-- Categories / Quick Filter Pills -->
            <div class="flex items-center gap-2 overflow-x-auto pb-2 md:pb-0 scrollbar-none">
              <button 
                *ngFor="let cat of categories" 
                (click)="selectCategory(cat)"
                [class]="selectedCategory === cat 
                  ? 'px-4 py-2 rounded-full bg-[#1A1917] text-white text-xs font-medium transition-all shadow-sm whitespace-nowrap' 
                  : 'px-4 py-2 rounded-full bg-white text-[#6B6864] hover:text-[#1A1917] hover:bg-[#F4F2EE] text-xs font-medium border border-[#E5E3DF] transition-all whitespace-nowrap'">
                {{ cat }}
              </button>
            </div>
          </div>

          <!-- Metadata Status Bar -->
          <div class="flex items-center justify-between pt-4 text-xs text-[#9D9890]">
            <span>Hiển thị {{ filteredProducts.length }} sản phẩm</span>
            <div class="flex items-center gap-1.5">
              <span class="w-2 h-2 rounded-full bg-emerald-500 animate-pulse"></span>
              <span>Hàng sẵn sàng giao ngay</span>
            </div>
          </div>
        </header>

        <!-- Skeleton Loading State -->
        <div *ngIf="loading" class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6 lg:gap-8">
          <div *ngFor="let i of [1,2,3,4,5,6,7,8]" class="p-2.5 bg-white rounded-[28px] border border-[#E5E3DF] animate-pulse">
            <div class="aspect-[4/5] bg-[#E5E3DF] rounded-[20px] mb-4"></div>
            <div class="h-4 bg-[#E5E3DF] rounded-full w-2/3 mb-2"></div>
            <div class="h-4 bg-[#E5E3DF] rounded-full w-1/3"></div>
          </div>
        </div>

        <!-- Error State -->
        <div *ngIf="error" class="p-6 bg-red-50/80 backdrop-blur border border-red-200 rounded-[24px] text-center text-red-600 font-medium my-8">
          {{ error }}
        </div>

        <!-- Product Grid: Double-Bezel Card Architecture -->
        <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6 lg:gap-8" *ngIf="!loading && filteredProducts.length > 0">
          <article 
            *ngFor="let product of filteredProducts" 
            (click)="navigateToDetail(product)"
            class="group cursor-pointer p-2.5 bg-white hover:bg-[#FAF9F6] rounded-[28px] border border-[#E5E3DF] hover:border-[#C5A882]/50 shadow-[0_4px_20px_rgba(0,0,0,0.02)] hover:shadow-[0_16px_36px_rgba(26,25,23,0.08)] transition-all duration-500 flex flex-col">
            
            <!-- Inner Core Image Container -->
            <div class="relative aspect-[4/5] bg-[#F4F2EE] rounded-[20px] overflow-hidden mb-4">
              <img 
                [src]="getProductImage(product)" 
                [alt]="product.name" 
                class="w-full h-full object-cover group-hover:scale-105 transition-transform duration-700 ease-[cubic-bezier(0.32,0.72,0,1)]" />
              
              <!-- Subtle gradient overlay on hover -->
              <div class="absolute inset-0 bg-gradient-to-t from-black/50 via-transparent to-transparent opacity-0 group-hover:opacity-100 transition-opacity duration-500"></div>

              <!-- Top Badges -->
              <div class="absolute top-3 left-3 flex flex-col gap-1 z-10">
                <span *ngIf="product.stock === 0" class="px-3 py-1 rounded-full text-[10px] font-bold uppercase tracking-wider bg-black/80 text-white backdrop-blur">
                  Hết hàng
                </span>
                <span *ngIf="product.stock > 0 && product.stock <= 5" class="px-3 py-1 rounded-full text-[10px] font-bold uppercase tracking-wider bg-amber-500 text-white shadow-sm">
                  Chỉ còn {{ product.stock }}
                </span>
              </div>

              <!-- Nested CTA "Button-in-Button" Architecture (Bottom Hover Action) -->
              <div class="absolute bottom-3 left-3 right-3 opacity-0 group-hover:opacity-100 transform translate-y-3 group-hover:translate-y-0 transition-all duration-300 z-10">
                <button 
                  (click)="addToCart($event, product)"
                  [disabled]="product.stock === 0 || isAddingToCart"
                  class="w-full py-2.5 px-4 rounded-full bg-[#1A1917]/95 backdrop-blur-md text-white text-xs font-semibold tracking-wider flex items-center justify-between hover:bg-black active:scale-[0.98] transition-all disabled:opacity-50 disabled:cursor-not-allowed">
                  <span>{{ product.stock === 0 ? 'Tạm hết hàng' : 'Thêm vào giỏ' }}</span>
                  <!-- Nested Trailing Icon Circle -->
                  <div class="w-6 h-6 rounded-full bg-white/20 flex items-center justify-center text-white group-hover:bg-white group-hover:text-[#1A1917] transition-all">
                    <svg width="12" height="12" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round">
                      <path d="M5 12h14M12 5l7 7-7 7"/>
                    </svg>
                  </div>
                </button>
              </div>
            </div>

            <!-- Product Info Container -->
            <div class="px-2 pb-2 flex-1 flex flex-col justify-between">
              <div>
                <div class="flex items-center justify-between mb-1">
                  <span class="text-[10px] font-bold text-[#A8885E] uppercase tracking-[0.14em]">
                    {{ product.categoryName || 'ATELIER' }}
                  </span>
                  <span class="text-[11px] text-[#9D9890] flex items-center gap-1">
                    <svg width="12" height="12" viewBox="0 0 24 24" fill="#C5A882" stroke="none"><polygon points="12 2 15.09 8.26 22 9.27 17 14.14 18.18 21.02 12 17.77 5.82 21.02 7 14.14 2 9.27 8.91 8.26 12 2"/></svg>
                    4.9
                  </span>
                </div>

                <h3 class="text-base font-semibold text-[#1A1917] group-hover:text-[#C5A882] transition-colors line-clamp-1 mb-2">
                  {{ product.name }}
                </h3>
              </div>

              <div class="flex items-center justify-between pt-2 border-t border-[#F4F2EE]">
                <span class="text-base font-bold text-[#1A1917]">
                  {{ product.price | currency:'VND':'symbol':'1.0-0' }}
                </span>
                <span class="text-[11px] font-medium text-[#71717A] bg-[#F4F4F5] px-2 py-0.5 rounded-md">
                  Chính hãng
                </span>
              </div>
            </div>

          </article>
        </div>

        <!-- Empty State -->
        <div *ngIf="!loading && filteredProducts.length === 0" class="p-16 bg-white rounded-[32px] border border-[#E5E3DF] text-center max-w-md mx-auto my-12 shadow-sm">
          <div class="w-16 h-16 rounded-full bg-[#F4F2EE] text-[#1A1917] flex items-center justify-center mx-auto mb-4">
            <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8"><circle cx="11" cy="11" r="8"/><path d="m21 21-4.3-4.3"/></svg>
          </div>
          <h3 class="text-lg font-bold text-[#1A1917] mb-2">Không tìm thấy sản phẩm</h3>
          <p class="text-sm text-[#6B6864] mb-6">Hiện chưa có sản phẩm nào thuộc danh mục này.</p>
          <button (click)="selectCategory('Tất cả')" class="px-6 py-2.5 rounded-full bg-[#1A1917] text-white text-xs font-semibold uppercase tracking-wider hover:bg-black transition-all">
            Xem tất cả sản phẩm
          </button>
        </div>

      </div>
    </div>
  `
})
export class ProductsComponent implements OnInit {
  private productService = inject(ProductService);
  private cartService = inject(CartService);
  private tokenService = inject(TokenService);
  private router = inject(Router);

  products: IProduct[] = [];
  categories: string[] = ['Tất cả', 'Thời trang', 'Phụ kiện', 'Đồ trang trí', 'Nổi bật'];
  selectedCategory = 'Tất cả';
  loading = false;
  error = '';
  isAddingToCart = false;

  get filteredProducts(): IProduct[] {
    if (this.selectedCategory === 'Tất cả') {
      return this.products;
    }
    return this.products.filter(p => p.categoryName === this.selectedCategory);
  }

  getProductImage(product: IProduct): string {
    if (product.imageUrls && product.imageUrls.length > 0 && product.imageUrls[0]) {
      return product.imageUrls[0];
    }
    if (product.image) {
      return product.image;
    }
    return 'https://images.unsplash.com/photo-1523275335684-37898b6baf30?auto=format&fit=crop&q=80&w=600';
  }

  ngOnInit(): void {
    this.loadProducts();
  }

  selectCategory(cat: string) {
    this.selectedCategory = cat;
  }

  navigateToDetail(product: IProduct) {
    this.router.navigate(['/product', product.id]);
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
      error: () => {
        this.error = 'Không thể tải danh sách sản phẩm. Vui lòng thử lại sau.';
        this.loading = false;
      }
    });
  }

  addToCart(event: Event, product: IProduct) {
    event.stopPropagation(); // Ngăn điều hướng sang trang chi tiết khi nhấn nút mua

    // 1. Kiểm tra đăng nhập trước khi thêm vào giỏ
    if (!this.tokenService.getToken()) {
      this.router.navigate(['/account/login'], {
        queryParams: { returnUrl: this.router.url }
      });
      return;
    }

    const variantId = product.variants && product.variants.length > 0
      ? product.variants[0].id
      : Number(product.id);

    // 2. Tiến hành thêm vào giỏ
    this.isAddingToCart = true;
    this.cartService.addToCart({
      productVariantId: variantId,
      quantity: 1
    }).subscribe({
      next: (res) => {
        this.isAddingToCart = false;
        if (res.success) {
          alert('Đã thêm sản phẩm vào giỏ hàng!');
        } else {
          alert(this.cartService.getErrorMessage(res.statusCode, res.message));
        }
      },
      error: (err) => {
        this.isAddingToCart = false;
        alert(err.message || err.errors?.[0] || 'Có lỗi xảy ra khi thêm vào giỏ.');
      }
    });
  }
}

