import { Component, Input, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ICustomerStatistics } from '../../../../../models/admin-customer.model';
import { AdminCustomerService } from '../../../../../services/admin-customer.service';

@Component({
  selector: 'app-admin-customer-recent-orders',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './admin-customer-recent-orders.component.html'
})
export class AdminCustomerRecentOrdersComponent implements OnInit {
  @Input() customerId!: number;
  
  private customerService = inject(AdminCustomerService);
  
  statistics: ICustomerStatistics | null = null;
  isLoading = true;

  ngOnInit(): void {
    if (this.customerId) {
      this.loadStatistics();
    }
  }

  loadStatistics() {
    this.isLoading = true;
    this.customerService.getCustomerStatistics(this.customerId).subscribe({
      next: (res) => {
        this.statistics = res;
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Lỗi khi tải thống kê khách hàng', err);
        this.isLoading = false;
      }
    });
  }
}
