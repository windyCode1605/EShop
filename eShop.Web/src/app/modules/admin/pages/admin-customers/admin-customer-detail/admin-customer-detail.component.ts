import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { AdminCustomerInfoComponent } from './components/admin-customer-info/admin-customer-info.component';
import { AdminCustomerRecentOrdersComponent } from './components/admin-customer-recent-orders/admin-customer-recent-orders.component';

@Component({
  selector: 'app-admin-customer-detail',
  standalone: true,
  imports: [CommonModule, AdminCustomerInfoComponent, AdminCustomerRecentOrdersComponent],
  template: `
    <div class="page-container p-6">
      <h2 class="text-2xl font-bold mb-6">Chi tiết khách hàng</h2>

      <div *ngIf="customerId" class="grid grid-cols-1 lg:grid-cols-3 gap-6">
        
        <!-- Cột trái: Thông tin Profile (Child Component) -->
        <div class="lg:col-span-1">
          <app-admin-customer-info 
            [customerId]="customerId">
          </app-admin-customer-info>
        </div>

        <!-- Cột phải: Thống kê & Đơn hàng (Child Component) -->
        <div class="lg:col-span-2">
          <app-admin-customer-recent-orders 
            [customerId]="customerId">
          </app-admin-customer-recent-orders>
        </div>

      </div>
    </div>
  `
})
export class AdminCustomerDetailComponent implements OnInit {
  private route = inject(ActivatedRoute);

  customerId: number | null = null;

  ngOnInit() {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    if (id) {
      this.customerId = id;
    }
  }
}
