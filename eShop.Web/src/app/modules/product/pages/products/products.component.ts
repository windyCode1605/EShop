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
    <div class="max-w-[1280px] mx-auto px-4 py-16 lg:py-24">
      <div class="flex flex-col md:flex-row md:items-end justify-between mb-16 gap-6">
        <div class="max-w-2xl">
          <span class="text-xs uppercase tracking-[0.2em] text-[#71717A] font-medium mb-4 block">The Collections</span>
          <h1 class="text-4xl lg:text-5xl font-semibold text-[#18181B] tracking-tight mb-4">Our Products</h1>
          <p class="text-[#71717A] text-lg">Curated essentials for the modern lifestyle.</p>
        </div>
      </div>

      <div *ngIf="loading" class="flex justify-center items-center h-64">
        <div class="text-[#71717A] animate-pulse font-medium tracking-wide">Loading collection...</div>
      </div>

      <div *ngIf="error" class="bg-red-50 text-red-500 p-6 rounded-[16px] text-center border border-red-100">
        {{ error }}
      </div>

      <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-8 lg:gap-12" *ngIf="!loading && products.length > 0">
        <!-- Product Card -->
        <article *ngFor="let product of products" class="group cursor-pointer flex flex-col">
          <div class="relative aspect-[4/5] bg-[#F4F4F5] rounded-[24px] overflow-hidden mb-6">
            <!-- Replace with real product image if available, using placeholder for now -->
            <img src="https://images.unsplash.com/photo-1523275335684-37898b6baf30?auto=format&fit=crop&q=80&w=600" 
                 [alt]="product.name" 
                 class="w-full h-full object-cover transition-transform duration-700 group-hover:scale-105" />
            
            <div class="absolute inset-0 bg-black/5 opacity-0 group-hover:opacity-100 transition-opacity duration-500"></div>

            <!-- Quick actions on hover -->
            <div class="absolute bottom-4 left-4 right-4 flex gap-2 opacity-0 group-hover:opacity-100 transform translate-y-4 group-hover:translate-y-0 transition-all duration-300">
              <button 
                (click)="addToCart($event, product)"
                [disabled]="product.stock === 0 || isAddingToCart"
                class="w-full py-3 bg-[#2563EB] text-white text-sm font-medium rounded-xl hover:bg-[#1D4ED8] transition-colors disabled:opacity-50 disabled:cursor-not-allowed">
                {{ product.stock === 0 ? 'Out of Stock' : 'Add to Cart' }}
              </button>
            </div>
            
            <!-- Badges -->
            <div *ngIf="product.stock === 0" class="absolute top-4 left-4 bg-white/90 backdrop-blur px-3 py-1 rounded-full text-xs font-medium text-[#18181B]">
              Sold Out
            </div>
          </div>

          <div class="flex-1 flex flex-col">
            <div class="flex justify-between items-start gap-4 mb-2">
              <h3 class="text-lg font-semibold text-[#18181B] group-hover:text-gray-600 transition-colors line-clamp-1">
                {{ product.name }}
              </h3>
              <strong class="text-lg font-medium text-[#18181B] whitespace-nowrap">
                {{ product.price | currency:'VND':'symbol':'1.0-0' }}
              </strong>
            </div>
            <p class="text-sm text-[#71717A] mb-3">{{ product.categoryName }}</p>
          </div>
        </article>
      </div>

      <div *ngIf="!loading && products.length === 0" class="flex flex-col items-center justify-center h-64 text-[#71717A]">
        <svg xmlns="http://www.w3.org/2000/svg" width="48" height="48" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1" stroke-linecap="round" stroke-linejoin="round" class="mb-4 opacity-50"><line x1="22" y1="12" x2="2" y2="12"></line><path d="M5.45 5.11 2 12v6a2 2 0 0 0 2 2h16a2 2 0 0 0 2-2v-6l-3.45-6.89A2 2 0 0 0 16.76 4H7.24a2 2 0 0 0-1.79 1.11z"></path><line x1="6" y1="16" x2="6.01" y2="16"></line><line x1="10" y1="16" x2="10.01" y2="16"></line></svg>
        <p class="text-lg">No products available at the moment.</p>
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
  loading = false;
  error = '';
  isAddingToCart = false;

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
      error: () => {
        this.error = 'Failed to load products. Please try again later.';
        this.loading = false;
      }
    });
  }

  addToCart(event: Event, product: IProduct) {
    event.stopPropagation(); // Prevent navigating to detail page if we add one

    // 1. Kiểm tra đăng nhập trước khi thêm vào giỏ
    if (!this.tokenService.getToken()) {
      // Nếu chưa đăng nhập, lưu ReturnUrl và chuyển hướng đến Login
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
        if (res.isSuccess) {
          alert('Đã thêm sản phẩm vào giỏ hàng!');
        } else {
          alert(this.cartService.getErrorMessage(res.errorCode, res.otherData));
        }
      },
      error: (err) => {
        this.isAddingToCart = false;
        alert(err.otherData || 'Có lỗi xảy ra khi thêm vào giỏ.');
      }
    });
  }
}
