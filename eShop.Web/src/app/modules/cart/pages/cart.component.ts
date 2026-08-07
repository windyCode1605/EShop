import { Component, computed, OnInit, signal } from '@angular/core';
import { CommonModule, CurrencyPipe } from '@angular/common';
import { RouterModule } from '@angular/router';
import { CartService } from '../services/cart.service';
import { CartSummaryDto, CartItemDto } from '../models/cart.models';

@Component({
  selector: 'app-cart-page',
  standalone: true,
  imports: [CommonModule, RouterModule],
  providers: [CurrencyPipe],
  templateUrl: './cart.component.html',
  styleUrls: ['./cart.component.scss']
})
export class CartPageComponent implements OnInit {
  // Local state signals
  readonly cartSummary = signal<CartSummaryDto | null>(null);
  readonly cartItems = computed(() => this.cartSummary()?.items || []);
  readonly hasItems = computed(() => this.cartItems().length > 0);
  
  // Computed Order Summary
  readonly subtotal = computed(() => this.cartSummary()?.subtotal || 0);
  readonly shippingFee = signal<number>(0); // Giả định freeship hoặc có logic tính phí sau
  readonly estimatedTotal = computed(() => this.subtotal() + this.shippingFee());

  readonly isLoading = signal<boolean>(true);
  readonly errorMessage = signal<string>('');

  constructor(private readonly cartService: CartService) {}

  ngOnInit(): void {
    this.loadCart();
  }

  loadCart(): void {
    this.isLoading.set(true);
    this.errorMessage.set('');
    
    this.cartService.getMyCart().subscribe({
      next: (res) => {
        if (res.success && res.data) {
          this.cartSummary.set(res.data);
        } else {
          this.errorMessage.set(res.message || 'Không thể lấy thông tin giỏ hàng.');
        }
        this.isLoading.set(false);
      },
      error: (err) => {
        this.errorMessage.set(err.message || err.errors?.[0] || 'Đã xảy ra lỗi khi tải giỏ hàng.');
        this.isLoading.set(false);
      }
    });
  }

  // Tạm thời mock logic update UI tại client để thấy tương tác ngay, 
  // Sau này gọi API update/remove từ cartService
  increaseQuantity(item: CartItemDto): void {
    if (item.quantity >= item.maxQuantity) return;
    const newQty = item.quantity + 1;
    this.updateItemQuantity(item, newQty);
  }

  decreaseQuantity(item: CartItemDto): void {
    if (item.quantity <= 1) return;
    const newQty = item.quantity - 1;
    this.updateItemQuantity(item, newQty);
  }

  private updateItemQuantity(item: CartItemDto, newQuantity: number): void {
    // Optimistic UI update
    const prevQty = item.quantity;
    item.quantity = newQuantity;
    item.lineTotal = item.quantity * item.unitPrice;
    this.recalculateMockSubtotal();

    this.cartService.updateCartItem(item.id, newQuantity).subscribe({
      next: (res) => {
        if (!res.success) {
          // Revert on failure
          item.quantity = prevQty;
          item.lineTotal = item.quantity * item.unitPrice;
          this.recalculateMockSubtotal();
          this.errorMessage.set(res.message || 'Không thể cập nhật số lượng.');
        } else {
           // Có thể gọi lại loadCart() để đồng bộ hoàn toàn với server nếu server có logic tính phí đặc biệt
           // this.loadCart();
        }
      },
      error: () => {
        // Revert on failure
        item.quantity = prevQty;
        item.lineTotal = item.quantity * item.unitPrice;
        this.recalculateMockSubtotal();
        this.errorMessage.set('Đã xảy ra lỗi khi cập nhật số lượng.');
      }
    });
  }

  removeItem(item: CartItemDto): void {
    const summary = this.cartSummary();
    if (!summary) return;
    
    // Lưu lại trạng thái cũ để revert nếu cần (Optimistic UI)
    const oldItems = [...summary.items];
    const newItems = summary.items.filter(x => x.id !== item.id);
    
    // Update local UI ngay lập tức
    this.cartSummary.set({
      ...summary,
      items: newItems,
      totalItems: newItems.reduce((acc, curr) => acc + curr.quantity, 0),
      subtotal: newItems.reduce((acc, curr) => acc + curr.lineTotal, 0)
    });

    this.cartService.removeCartItem(item.id).subscribe({
      next: (res) => {
        if (!res.success) {
          // Revert nếu lỗi
          this.cartSummary.set({
            ...summary,
            items: oldItems
          });
          this.recalculateMockSubtotal();
          this.errorMessage.set(res.message || 'Không thể xóa sản phẩm.');
        }
      },
      error: () => {
        // Revert nếu lỗi
        this.cartSummary.set({
            ...summary,
            items: oldItems
        });
        this.recalculateMockSubtotal();
        this.errorMessage.set('Đã xảy ra lỗi khi xóa sản phẩm.');
      }
    });
  }

  private recalculateMockSubtotal(): void {
    const summary = this.cartSummary();
    if (!summary) return;

    this.cartSummary.set({
      ...summary,
      totalItems: summary.items.reduce((acc, curr) => acc + curr.quantity, 0),
      subtotal: summary.items.reduce((acc, curr) => acc + curr.lineTotal, 0)
    });
  }
}
