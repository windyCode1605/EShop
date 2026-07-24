import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { OrderService } from '../../../../modules/order/services/order.service';
import { OrderDetailsModalComponent } from '../../components/order-details-modal/order-details-modal.component';
import { CancelOrderModalComponent, CancelOrderResult } from '../../components/cancel-order-modal/cancel-order-modal.component';

@Component({
  selector: 'app-profile-orders',
  standalone: true,
  imports: [CommonModule, OrderDetailsModalComponent, CancelOrderModalComponent],
  template: `
    <section class="flex flex-col gap-6">
      <div class="flex items-center justify-between mb-2">
        <h2 class="text-2xl font-medium tracking-tight text-[#18181B]">Lịch sử đơn hàng</h2>
      </div>

      <!-- Filters (Optional, keeping it simple for now) -->
      <div class="flex gap-2 overflow-x-auto pb-2 scrollbar-none">
        <button class="px-4 py-2 rounded-full text-sm font-medium transition-colors bg-zinc-900 text-white border border-transparent whitespace-nowrap">Tất cả đơn</button>
        <button class="px-4 py-2 rounded-full text-sm font-medium transition-colors bg-white text-zinc-600 border border-zinc-200 hover:border-zinc-300 hover:text-zinc-900 whitespace-nowrap">Đang xử lý</button>
        <button class="px-4 py-2 rounded-full text-sm font-medium transition-colors bg-white text-zinc-600 border border-zinc-200 hover:border-zinc-300 hover:text-zinc-900 whitespace-nowrap">Đang giao</button>
        <button class="px-4 py-2 rounded-full text-sm font-medium transition-colors bg-white text-zinc-600 border border-zinc-200 hover:border-zinc-300 hover:text-zinc-900 whitespace-nowrap">Đã hoàn thành</button>
      </div>

      <!-- Order List -->
      <div class="flex flex-col gap-4 mt-2">
        <ng-container *ngIf="!isLoading(); else skeletonList">
          
          <ng-container *ngIf="orders().length > 0; else emptyOrders">
            <!-- Order Card -->
            <div *ngFor="let order of orders()" class="bg-white border border-zinc-200 rounded-[24px] overflow-hidden hover:shadow-sm hover:border-zinc-300 transition-all duration-300 group">
              
              <!-- Card Header -->
              <div class="p-5 md:px-6 md:py-5 border-b border-zinc-100 flex flex-col sm:flex-row sm:items-center justify-between gap-4">
                <div class="flex flex-col gap-1.5">
                  <div class="flex items-center gap-3">
                    <span class="font-semibold text-zinc-900 text-base">{{ order.orderCode }}</span>
                    <span class="px-2.5 py-1 text-[10px] font-bold uppercase tracking-wider rounded-md" [ngClass]="getStatusClass(order.status)">
                      {{ getStatusText(order.status) }}
                    </span>
                  </div>
                  <span class="text-sm text-zinc-500 flex items-center gap-1.5">
                    <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><rect x="3" y="4" width="18" height="18" rx="2" ry="2"></rect><line x1="16" y1="2" x2="16" y2="6"></line><line x1="8" y1="2" x2="8" y2="6"></line><line x1="3" y1="10" x2="21" y2="10"></line></svg>
                    {{ order.createdDate | date:'dd/MM/yyyy HH:mm' }}
                  </span>
                </div>
                <div class="flex flex-col sm:items-end gap-1">
                  <span class="text-sm text-zinc-500">Tổng tiền</span>
                  <span class="font-bold text-zinc-900 text-lg">{{ order.totalAmount | number }}đ</span>
                </div>
              </div>

              <!-- Card Body (Items preview) -->
              <div class="p-5 md:p-6 bg-zinc-50/30 flex flex-col sm:flex-row items-start sm:items-center justify-between gap-6">
                <div class="flex-1 min-w-0">
                  <div class="flex items-center gap-4">
                    <div class="w-16 h-16 bg-white border border-zinc-200 rounded-xl flex items-center justify-center shrink-0 shadow-sm relative overflow-hidden">
                       <!-- Mock Image -->
                       <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round" class="text-zinc-300"><rect x="3" y="3" width="18" height="18" rx="2" ry="2"></rect><circle cx="8.5" cy="8.5" r="1.5"></circle><polyline points="21 15 16 10 5 21"></polyline></svg>
                       <div *ngIf="order.items.length > 1" class="absolute bottom-0 right-0 bg-zinc-900/80 text-white text-[10px] font-bold px-1.5 py-0.5 rounded-tl-lg">
                          +{{ order.items.length - 1 }}
                       </div>
                    </div>
                    <div class="flex flex-col min-w-0">
                      <p class="text-sm font-medium text-zinc-900 truncate max-w-xs md:max-w-md">{{ order.items[0]?.productName }}</p>
                      <p class="text-xs text-zinc-500 mt-1">Phân loại: {{ order.items[0]?.variantSKU }}</p>
                      <p class="text-xs text-zinc-500 mt-0.5">x{{ order.items[0]?.quantity }}</p>
                    </div>
                  </div>
                </div>
                <div class="flex items-center gap-3">
                  <button *ngIf="order.status === 'PENDING'" type="button" (click)="openCancelModal(order)"
                          class="shrink-0 px-5 py-2.5 bg-white border border-red-200 text-red-600 text-sm font-medium rounded-[12px] hover:border-red-300 hover:bg-red-50 transition-all duration-250 flex items-center gap-2">
                    Hủy đơn
                  </button>
                  <button type="button" (click)="openOrderDetails(order)" 
                          class="shrink-0 px-5 py-2.5 bg-white border border-zinc-200 text-zinc-900 text-sm font-medium rounded-[12px] hover:border-zinc-900 hover:bg-zinc-50 transition-all duration-250 flex items-center gap-2 group-hover:shadow-sm">
                    Xem chi tiết
                    <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="transition-transform group-hover:translate-x-0.5"><line x1="5" y1="12" x2="19" y2="12"></line><polyline points="12 5 19 12 12 19"></polyline></svg>
                  </button>
                </div>
              </div>

            </div>
          </ng-container>

        </ng-container>
      </div>
    </section>

    <!-- Empty State -->
    <ng-template #emptyOrders>
      <div class="py-20 px-8 border border-zinc-200 border-dashed rounded-[24px] text-center bg-zinc-50/50 flex flex-col items-center justify-center mt-4">
        <div class="w-14 h-14 rounded-full bg-white border border-zinc-200 flex items-center justify-center text-zinc-400 mb-5 shadow-sm">
          <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"><circle cx="9" cy="21" r="1"></circle><circle cx="20" cy="21" r="1"></circle><path d="M1 1h4l2.68 13.39a2 2 0 0 0 2 1.61h9.72a2 2 0 0 0 2-1.61L23 6H6"></path></svg>
        </div>
        <h3 class="text-zinc-900 text-lg font-medium mb-2">Chưa có đơn hàng nào</h3>
        <p class="text-sm text-zinc-500 mb-8 max-w-[250px] leading-relaxed">Bạn chưa thực hiện bất kỳ đơn hàng nào. Hãy khám phá các sản phẩm mới nhất nhé.</p>
        <a href="/product" class="px-8 py-3 bg-zinc-900 text-white text-sm font-medium rounded-[14px] hover:bg-zinc-800 hover:scale-[1.02] transition-transform duration-250 shadow-md shadow-zinc-900/10">
          Tiếp tục mua sắm
        </a>
      </div>
    </ng-template>

    <!-- Skeleton -->
    <ng-template #skeletonList>
      <div class="animate-pulse flex flex-col gap-5 mt-2">
        <div class="h-[160px] bg-zinc-100 rounded-[24px] w-full border border-zinc-100/50"></div>
        <div class="h-[160px] bg-zinc-100 rounded-[24px] w-full border border-zinc-100/50"></div>
        <div class="h-[160px] bg-zinc-100 rounded-[24px] w-full border border-zinc-100/50"></div>
      </div>
    </ng-template>

    <!-- Modal Chi tiết đơn hàng -->
    <app-order-details-modal 
      [isOpen]="isModalOpen()" 
      [order]="selectedOrder()"
      (closed)="closeModal()"
      (cancelClicked)="onCancelFromDetails($event)">
    </app-order-details-modal>

    <!-- Modal Hủy đơn hàng -->
    <app-cancel-order-modal
      [isOpen]="isCancelModalOpen()"
      [order]="cancellingOrder()"
      [isSubmitting]="isCancelling()"
      (closed)="closeCancelModal()"
      (confirmed)="onConfirmCancel($event)">
    </app-cancel-order-modal>
  `
})
export class ProfileOrdersComponent implements OnInit {
  private orderService = inject(OrderService);

  orders = signal<any[]>([]);
  isLoading = signal(true);
  
  isModalOpen = signal(false);
  selectedOrder = signal<any>(null);

  isCancelModalOpen = signal(false);
  cancellingOrder = signal<any>(null);
  isCancelling = signal(false);

  ngOnInit() {
    this.loadOrders();
  }

  loadOrders() {
    this.isLoading.set(true);
    // Truyền tham số phân trang để backend trả về danh sách items, tránh bị mảng rỗng do thiếu pageSize
    this.orderService.getMyOrders({ pageNumber: 1, pageSize: 100 }).subscribe({
      next: (res: any) => {
        this.isLoading.set(false);
        if (res.isSuccess || res.success) {
          const data = res.value || res.data;
          // data can be an object with .items array if paginated, or just the array
          if (data && Array.isArray(data.items)) {
            this.orders.set(data.items);
          } else if (Array.isArray(data)) {
            this.orders.set(data);
          } else {
            this.orders.set([]);
          }
        }
      },
      error: () => {
        this.isLoading.set(false);
      }
    });
  }

  openOrderDetails(order: any) {
    this.selectedOrder.set(order);
    this.isModalOpen.set(true);
  }

  closeModal() {
    this.isModalOpen.set(false);
    // Optional: setTimeout(() => this.selectedOrder.set(null), 300) to clear after animation
  }

  openCancelModal(order: any) {
    this.cancellingOrder.set(order);
    this.isCancelModalOpen.set(true);
  }

  onCancelFromDetails(order: any) {
    this.closeModal();
    // Wait for the details modal to close before opening cancel modal for better UX
    setTimeout(() => {
      this.openCancelModal(order);
    }, 250);
  }

  closeCancelModal() {
    this.isCancelModalOpen.set(false);
  }

  onConfirmCancel(result: CancelOrderResult) {
    this.isCancelling.set(true);
    this.orderService.cancelOrder(result.orderId, result.reason).subscribe({
      next: (res) => {
        this.isCancelling.set(false);
        if (res.isSuccess || res.success) {
          this.closeCancelModal();
          this.loadOrders(); // Reload to get updated status
        }
      },
      error: () => {
        this.isCancelling.set(false);
        // Should handle error displaying here
      }
    });
  }

  getStatusText(status: string): string {
    const map: Record<string, string> = {
      'PENDING': 'Chờ xử lý',
      'CONFIRMED': 'Đã xác nhận',
      'SHIPPING': 'Đang giao hàng',
      'DELIVERED': 'Hoàn thành',
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
