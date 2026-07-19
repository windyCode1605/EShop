import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { OrderService } from '../../services/order.service';

export interface OrderItem {
  id: number;
  productName: string;
  variantSKU: string;
  quantity: number;
  lineTotal: number;
}

export interface Order {
  id: number;
  orderCode: string;
  createdDate: string;
  status: string;
  totalAmount: number;
  items: OrderItem[];
}

@Component({
  selector: 'app-order-history',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './order-history.component.html',
  styleUrls: ['./order-history.component.scss']
})
export class OrderHistoryComponent implements OnInit {
  private orderService = inject(OrderService);

  orders = signal<Order[]>([]);
  isLoading = signal<boolean>(true);
  hasError = signal<boolean>(false);
  totalItems = signal<number>(0);

  ngOnInit() {
    this.fetchOrders();
  }

  fetchOrders() {
    this.isLoading.set(true);
    this.hasError.set(false);

    // Fetch page 1, size 10 for demo. We can add pagination later.
    const params = { pageNumber: 1, pageSize: 20 };

    this.orderService.getMyOrders(params).subscribe({
      next: (res) => {
        if (res.isSuccess && res.value) {
          this.orders.set(res.value.items || []);
          this.totalItems.set(res.value.totalItems || 0);
        } else {
          this.hasError.set(true);
        }
        this.isLoading.set(false);
      },
      error: (err) => {
        console.error(err);
        this.hasError.set(true);
        this.isLoading.set(false);
      }
    });
  }

  getStatusConfig(status: string): { text: string, type: 'pending' | 'processing' | 'shipping' | 'delivered' | 'cancelled' } {
    switch (status) {
      case 'PENDING': return { text: 'Chờ xác nhận', type: 'pending' };
      case 'CONFIRMED': return { text: 'Đã xác nhận', type: 'processing' };
      case 'PROCESSING': return { text: 'Đang xử lý', type: 'processing' };
      case 'SHIPPING': return { text: 'Đang giao hàng', type: 'shipping' };
      case 'DELIVERED': return { text: 'Đã giao', type: 'delivered' };
      case 'RETURNED': return { text: 'Trả hàng', type: 'cancelled' };
      case 'CANCELLED': return { text: 'Đã huỷ', type: 'cancelled' };
      default: return { text: status, type: 'pending' };
    }
  }
}
