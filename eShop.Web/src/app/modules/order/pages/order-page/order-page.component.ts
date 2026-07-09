import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { OrderService } from '../../services/order.service';

@Component({
  selector: 'app-order-page',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './order-page.component.html',
  styleUrls: ['./order-page.component.scss']
})
export class OrderPageComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private orderService = inject(OrderService);

  order = signal<any>(null);
  isLoading = signal<boolean>(true);
  hasError = signal<boolean>(false);


  private statusFlow = ['Pending', 'Processing', 'Shipped', 'Delivered'];

  ngOnInit() {
    this.route.paramMap.subscribe(params => {
      const orderId = Number(params.get('id'));
      if (orderId) {
        this.fetchOrder(orderId);
      } else {
        this.hasError.set(true);
        this.isLoading.set(false);
      }
    });
  }

  fetchOrder(id: number) {
    this.isLoading.set(true);
    this.hasError.set(false);
    this.orderService.getOrderById(id).subscribe({
      next: (res) => {
        if (res.isSuccess && res.value) {
          this.order.set(res.value);
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

  getStatusText(status: string): string {
    const map: Record<string, string> = {
      'Pending': 'Chờ xác nhận',
      'Processing': 'Đang xử lý',
      'Shipped': 'Đang giao hàng',
      'Delivered': 'Đã giao',
      'Cancelled': 'Đã huỷ'
    };
    return map[status] || status;
  }

  isStatusActive(stepStatus: string): boolean {
    const currentStatus = this.order()?.status;
    if (!currentStatus || currentStatus === 'Cancelled') return false;
    return currentStatus === stepStatus;
  }

  isStatusPast(stepStatus: string): boolean {
    const currentStatus = this.order()?.status;
    if (!currentStatus || currentStatus === 'Cancelled') return false;

    const currentIndex = this.statusFlow.indexOf(currentStatus);
    const stepIndex = this.statusFlow.indexOf(stepStatus);

    return currentIndex > stepIndex;
  }
}
