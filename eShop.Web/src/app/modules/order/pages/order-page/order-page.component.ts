import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { OrderService } from '../../services/order.service';
import { ReviewModalComponent } from '../../components/review-modal/review-modal.component';

@Component({
  selector: 'app-order-page',
  standalone: true,
  imports: [CommonModule, RouterLink, ReviewModalComponent],
  templateUrl: './order-page.component.html',
  styleUrls: ['./order-page.component.scss']
})
export class OrderPageComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private orderService = inject(OrderService);

  order = signal<any>(null);
  isLoading = signal<boolean>(true);
  hasError = signal<boolean>(false);
  
  isReviewModalOpen = signal<boolean>(false);
  selectedProductForReview = signal<any>(null);

  private statusFlow = ['PENDING', 'CONFIRMED', 'PROCESSING', 'SHIPPING', 'DELIVERED'];

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
        const data = res.value || res.data;
        if ((res.isSuccess || res.success) && data) {
          this.order.set(data);
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
      'PENDING': 'Chờ xác nhận',
      'CONFIRMED': 'Đã xác nhận',
      'PROCESSING': 'Đang xử lý',
      'SHIPPING': 'Đang giao hàng',
      'DELIVERED': 'Đã giao',
      'RETURNED': 'Trả hàng',
      'CANCELLED': 'Đã huỷ'
    };
    return map[status] || status;
  }

  isStatusActive(stepStatus: string): boolean {
    const currentStatus = this.order()?.status;
    if (!currentStatus || currentStatus === 'CANCELLED' || currentStatus === 'RETURNED') return false;
    
    // Group CONFIRMED and PROCESSING together for the timeline
    if (stepStatus === 'PROCESSING' && currentStatus === 'CONFIRMED') return true;
    
    return currentStatus === stepStatus;
  }

  isStatusPast(stepStatus: string): boolean {
    const currentStatus = this.order()?.status;
    if (!currentStatus || currentStatus === 'CANCELLED' || currentStatus === 'RETURNED') return false;

    const currentIndex = this.statusFlow.indexOf(currentStatus);
    let stepIndex = this.statusFlow.indexOf(stepStatus);
    
    // Special handling for PROCESSING which covers CONFIRMED too
    if (stepStatus === 'PROCESSING') {
      stepIndex = Math.max(this.statusFlow.indexOf('CONFIRMED'), this.statusFlow.indexOf('PROCESSING'));
    }

    return currentIndex > stepIndex;
  }

  openReview(item: any) {
    this.selectedProductForReview.set(item);
    this.isReviewModalOpen.set(true);
  }

  closeReview() {
    this.isReviewModalOpen.set(false);
    setTimeout(() => this.selectedProductForReview.set(null), 300); // Wait for animation
  }
}
