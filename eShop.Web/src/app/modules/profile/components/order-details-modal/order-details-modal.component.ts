import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-order-details-modal',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div *ngIf="isOpen" class="fixed inset-0 z-[100] flex items-center justify-center p-4 sm:p-6" (click)="close()">
      
      <!-- Backdrop -->
      <div class="absolute inset-0 bg-zinc-900/40 backdrop-blur-sm transition-opacity"></div>
      
      <!-- Modal Content -->
      <div class="relative w-full max-w-2xl bg-white rounded-[24px] shadow-2xl overflow-hidden flex flex-col max-h-[90vh]" (click)="$event.stopPropagation()">
        
        <!-- Header -->
        <div class="flex items-center justify-between px-6 py-5 border-b border-zinc-100 bg-white z-10">
          <div class="flex items-center gap-3">
            <h3 class="text-xl font-medium tracking-tight text-zinc-900">Chi tiết đơn hàng</h3>
            <span *ngIf="order" class="px-2.5 py-1 text-xs font-semibold rounded-full" [ngClass]="getStatusClass(order.status)">
              {{ getStatusText(order.status) }}
            </span>
          </div>
          <button type="button" (click)="close()" class="p-2 text-zinc-400 hover:text-zinc-900 hover:bg-zinc-100 rounded-full transition-colors">
            <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><line x1="18" y1="6" x2="6" y2="18"></line><line x1="6" y1="6" x2="18" y2="18"></line></svg>
          </button>
        </div>

        <!-- Body -->
        <div class="flex-1 overflow-y-auto p-6 scrollbar-thin">
          <ng-container *ngIf="order; else loadingTemplate">
            
            <!-- Order Meta Info -->
            <div class="grid grid-cols-2 sm:grid-cols-4 gap-4 mb-8">
              <div class="flex flex-col gap-1">
                <span class="text-xs text-zinc-500 uppercase tracking-wider font-semibold">Mã đơn hàng</span>
                <span class="text-sm font-medium text-zinc-900">{{ order.orderCode }}</span>
              </div>
              <div class="flex flex-col gap-1">
                <span class="text-xs text-zinc-500 uppercase tracking-wider font-semibold">Ngày đặt</span>
                <span class="text-sm font-medium text-zinc-900">{{ order.createdDate | date:'dd/MM/yyyy HH:mm' }}</span>
              </div>
              <div class="flex flex-col gap-1">
                <span class="text-xs text-zinc-500 uppercase tracking-wider font-semibold">Thanh toán</span>
                <span class="text-sm font-medium text-zinc-900">{{ order.paymentMethod }}</span>
              </div>
              <div class="flex flex-col gap-1">
                <span class="text-xs text-zinc-500 uppercase tracking-wider font-semibold">Trạng thái TT</span>
                <span class="text-sm font-medium text-zinc-900">{{ order.payment?.status === 'PENDING' ? 'Chưa thanh toán' : 'Đã thanh toán' }}</span>
              </div>
            </div>

            <!-- Receiver & Shipment Info -->
            <div class="grid grid-cols-1 sm:grid-cols-2 gap-6 p-5 bg-zinc-50 rounded-[16px] mb-8 border border-zinc-100">
              <div class="flex flex-col gap-2">
                <span class="text-xs text-zinc-500 uppercase tracking-wider font-semibold flex items-center gap-1.5">
                  <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M21 10c0 7-9 13-9 13s-9-6-9-13a9 9 0 0 1 18 0z"></path><circle cx="12" cy="10" r="3"></circle></svg>
                  Giao hàng đến
                </span>
                <div class="text-sm text-zinc-800 leading-relaxed">
                  <p class="font-medium mb-0.5">{{ order.shipment?.receiverName || '---' }}</p>
                  <p>{{ order.shipment?.receiverPhone || '---' }}</p>
                  <p class="text-zinc-600 mt-1">{{ order.shippingAddress }}</p>
                </div>
              </div>
              
              <div class="flex flex-col gap-2">
                <span class="text-xs text-zinc-500 uppercase tracking-wider font-semibold flex items-center gap-1.5">
                  <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><rect x="1" y="3" width="15" height="13"></rect><polygon points="16 8 20 8 23 11 23 16 16 16 16 8"></polygon><circle cx="5.5" cy="18.5" r="2.5"></circle><circle cx="18.5" cy="18.5" r="2.5"></circle></svg>
                  Thông tin vận chuyển
                </span>
                <div class="text-sm text-zinc-800 leading-relaxed">
                  <p><span class="text-zinc-500">Đơn vị:</span> {{ order.shipment?.shippingProvider || '---' }}</p>
                  <p><span class="text-zinc-500">Mã vận đơn:</span> <span class="font-medium">{{ order.shipment?.trackingNumber || 'Chưa cập nhật' }}</span></p>
                </div>
              </div>
            </div>

            <!-- Product Items -->
            <div class="mb-8">
              <h4 class="text-sm font-semibold text-zinc-900 mb-4 tracking-tight">Sản phẩm</h4>
              <div class="flex flex-col gap-4">
                <div *ngFor="let item of order.items" class="flex gap-4 p-4 rounded-[12px] border border-zinc-100 hover:border-zinc-200 transition-colors bg-white">
                  <!-- Product Image Placeholder -->
                  <div class="w-16 h-16 bg-zinc-100 rounded-lg flex items-center justify-center shrink-0 border border-zinc-200/50">
                     <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1" stroke-linecap="round" stroke-linejoin="round" class="text-zinc-300"><rect x="3" y="3" width="18" height="18" rx="2" ry="2"></rect><circle cx="8.5" cy="8.5" r="1.5"></circle><polyline points="21 15 16 10 5 21"></polyline></svg>
                  </div>
                  <div class="flex-1 flex flex-col justify-center">
                    <div class="flex justify-between items-start gap-2">
                      <p class="text-sm font-medium text-zinc-900 leading-tight mb-1">{{ item.productName }}</p>
                      <p class="text-sm font-semibold text-zinc-900 shrink-0">{{ item.lineTotal | number }}đ</p>
                    </div>
                    <div class="flex justify-between items-center text-xs text-zinc-500 mt-auto">
                      <span>Phân loại: {{ item.variantSKU }}</span>
                      <span>SL: x{{ item.quantity }}</span>
                    </div>
                  </div>
                </div>
              </div>
            </div>

            <!-- Cost Summary -->
            <div class="flex flex-col gap-2 pt-6 border-t border-zinc-100">
              <div class="flex justify-between text-sm text-zinc-600">
                <span>Tạm tính</span>
                <span>{{ order.subtotal | number }}đ</span>
              </div>
              <div class="flex justify-between text-sm text-zinc-600">
                <span>Phí vận chuyển</span>
                <span>{{ order.shippingFee | number }}đ</span>
              </div>
              <div *ngIf="order.discountAmount > 0" class="flex justify-between text-sm text-green-600">
                <span>Giảm giá</span>
                <span>-{{ order.discountAmount | number }}đ</span>
              </div>
              <div class="flex justify-between items-center mt-2 pt-4 border-t border-zinc-100">
                <span class="text-sm font-medium text-zinc-900 uppercase tracking-wider">Tổng cộng</span>
                <span class="text-xl font-bold text-zinc-900">{{ order.totalAmount | number }}đ</span>
              </div>
            </div>

          </ng-container>
        </div>

        <!-- Footer -->
        <div class="p-4 border-t border-zinc-100 bg-zinc-50/50 flex items-center" 
             [ngClass]="order?.status === 'PENDING' ? 'justify-between' : 'justify-end'">
          <button *ngIf="order?.status === 'PENDING'" 
                  type="button" 
                  (click)="cancel()" 
                  class="px-5 py-2.5 bg-white border border-red-200 text-red-600 text-sm font-medium rounded-[12px] hover:bg-red-50 hover:border-red-300 transition-colors">
            Hủy đơn hàng
          </button>
          <button type="button" (click)="close()" class="px-6 py-2.5 bg-zinc-900 text-white text-sm font-medium rounded-[12px] hover:bg-zinc-800 transition-colors">
            Đóng
          </button>
        </div>
      </div>
    </div>

    <!-- Skeleton Loading Template -->
    <ng-template #loadingTemplate>
      <div class="animate-pulse flex flex-col gap-6">
        <div class="grid grid-cols-4 gap-4">
          <div class="h-10 bg-zinc-100 rounded-lg"></div>
          <div class="h-10 bg-zinc-100 rounded-lg"></div>
          <div class="h-10 bg-zinc-100 rounded-lg"></div>
          <div class="h-10 bg-zinc-100 rounded-lg"></div>
        </div>
        <div class="h-32 bg-zinc-100 rounded-2xl"></div>
        <div class="h-24 bg-zinc-100 rounded-xl"></div>
        <div class="h-24 bg-zinc-100 rounded-xl"></div>
      </div>
    </ng-template>
  `
})
export class OrderDetailsModalComponent {
  @Input() isOpen = false;
  @Input() order: any = null;
  @Output() closed = new EventEmitter<void>();
  @Output() cancelClicked = new EventEmitter<any>();

  close() {
    this.closed.emit();
  }

  cancel() {
    this.cancelClicked.emit(this.order);
  }

  getStatusText(status: string): string {
    const map: Record<string, string> = {
      'PENDING': 'Chờ xử lý',
      'CONFIRMED': 'Đã xác nhận',
      'SHIPPING': 'Đang giao hàng',
      'DELIVERED': 'Giao thành công',
      'RETURNED': 'Đã hoàn trả',
      'CANCELLED': 'Đã huỷ'
    };
    return map[status] || status;
  }

  getStatusClass(status: string): string {
    switch (status) {
      case 'PENDING': return 'bg-amber-100 text-amber-800';
      case 'CONFIRMED': return 'bg-blue-100 text-blue-800';
      case 'SHIPPING': return 'bg-indigo-100 text-indigo-800';
      case 'DELIVERED': return 'bg-emerald-100 text-emerald-800';
      case 'RETURNED': return 'bg-purple-100 text-purple-800';
      case 'CANCELLED': return 'bg-red-100 text-red-800';
      default: return 'bg-zinc-100 text-zinc-800';
    }
  }
}
