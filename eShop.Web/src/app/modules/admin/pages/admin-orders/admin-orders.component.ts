import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AdminOrderService, IUpdateOrderStatus } from '../../services/admin-order.service';

@Component({
  selector: 'app-admin-orders',
  standalone: true,
  imports: [CommonModule, FormsModule],
  styleUrl: './admin-orders.component.scss',
  templateUrl: './admin-orders.component.html'
})
export class AdminOrdersComponent implements OnInit {
  isModalOpen = false;
  selectedOrderId: string | number = '';
  selectedOrderCode: string = '';
  currentStatus = '';
  selectedStatus = '';
  
  trackingNumber = '';
  shippingProvider = '';
  reason = '';
  isSaving = false;
  errorMessage = '';

  readonly validTransitions: Record<string, string[]> = {
    'PENDING': ['CONFIRMED', 'PROCESSING', 'CANCELLED'],
    'CONFIRMED': ['PROCESSING', 'CANCELLED'],
    'PROCESSING': ['SHIPPING', 'CANCELLED'],
    'SHIPPING': ['DELIVERED', 'RETURNED']
  };

  readonly statusLabels: Record<string, string> = {
    'PENDING': 'Pending',
    'CONFIRMED': 'Confirmed',
    'PROCESSING': 'Processing',
    'SHIPPING': 'Shipping',
    'DELIVERED': 'Delivered',
    'CANCELLED': 'Cancelled',
    'RETURNED': 'Returned'
  };

  get availableNextStatuses(): string[] {
    const next = this.validTransitions[this.currentStatus] || [];
    return [this.currentStatus, ...next];
  }

  expandedOrderId = signal<number | string | null>(null);

  constructor(
    public OrderService: AdminOrderService
  ) { }

  toggleExpand(orderId: number | string) {
    if (this.expandedOrderId() === orderId) {
      this.expandedOrderId.set(null);
    } else {
      this.expandedOrderId.set(orderId);
    }
  }

  ngOnInit(): void {
    this.OrderService.loadOrder()
  }
  
  openModal(orderId: string | number, orderCode: string, currentStatus: string) {
    this.selectedOrderId = orderId;
    this.selectedOrderCode = orderCode;
    this.currentStatus = currentStatus.toUpperCase();
    this.selectedStatus = this.currentStatus;
    this.trackingNumber = '';
    this.shippingProvider = '';
    this.reason = '';
    this.errorMessage = '';
    this.isModalOpen = true;
  }

  closeModal() {
    if (this.isSaving) return;
    this.isModalOpen = false;
  }

  onStatusChange(event: Event) {
    const selectElement = event.target as HTMLSelectElement;
    this.selectedStatus = selectElement.value;
  }

  saveStatus() {
    if (!this.selectedOrderId) return;
    
    this.errorMessage = '';
    this.isSaving = true;
    
    const payload: IUpdateOrderStatus = {
      newStatus: this.selectedStatus,
    };

    if (this.selectedStatus === 'SHIPPING') {
      if (!this.shippingProvider || !this.trackingNumber) {
        this.errorMessage = 'Vui lòng chọn đơn vị vận chuyển và nhập mã vận đơn.';
        this.isSaving = false;
        return;
      }
      payload.shippingProvider = this.shippingProvider;
      payload.trackingNumber = this.trackingNumber;
    } else if (this.selectedStatus === 'CANCELLED') {
      if (!this.reason.trim()) {
        this.errorMessage = 'Vui lòng nhập lý do hủy đơn.';
        this.isSaving = false;
        return;
      }
      payload.reason = this.reason;
    }

    this.OrderService.updateOrderStatus(this.selectedOrderId, payload).subscribe({
      next: () => {
        // Update local state
        const currentOrders = this.OrderService.Orders();
        const updatedOrders = currentOrders.map(o => {
          if (o.id === this.selectedOrderId) {
            return { ...o, status: this.selectedStatus };
          }
          return o;
        });
        this.OrderService.Orders.set(updatedOrders);
        this.isSaving = false;
        this.closeModal();
      },
      error: (err) => {
        console.error('Error updating status:', err);
        this.isSaving = false;
        
        if (err.error?.listParam && err.error.listParam.length > 0) {
          this.errorMessage = err.error.listParam.join('\n');
        } else if (err.error?.message) {
          this.errorMessage = err.error.message;
        } else {
          this.errorMessage = 'Có lỗi xảy ra khi cập nhật trạng thái đơn hàng.';
        }
      }
    });
  }
}
