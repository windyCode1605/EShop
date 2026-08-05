import { Component, Input, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ICustomerDetail } from '../../../../../models/admin-customer.model';
import { AdminCustomerService } from '../../../../../services/admin-customer.service';
import { ToastService } from '../../../../../../../core/services/toast.service';
import { ApiResponse } from '../../../../../../../core/models';

@Component({
  selector: 'app-admin-customer-info',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './admin-customer-info.component.html'
})
export class AdminCustomerInfoComponent implements OnInit {
  @Input() customerId!: number;

  private customerService = inject(AdminCustomerService);
  private toastService = inject(ToastService);

  customer: ICustomerDetail | null = null;
  isLoading = true;

  ngOnInit(): void {
    if (this.customerId) {
      this.loadCustomerInfo();
    }
  }

  loadCustomerInfo() {
    this.isLoading = true;
    this.customerService.getCustomerDetail(this.customerId).subscribe({
      next: (res) => {
        this.customer = res;
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Lỗi khi tải thông tin khách hàng', err);
        this.isLoading = false;
      }
    });
  }

  showConfirmModal = false;
  confirmActionType: 'lock' | 'unlock' = 'lock';

  openConfirmModal(isLocked: boolean) {
    if (!this.customer) return;
    this.confirmActionType = isLocked ? 'lock' : 'unlock';
    this.showConfirmModal = true;
  }

  closeConfirmModal() {
    this.showConfirmModal = false;
  }

  executeCustomerStatusChange() {
    if (!this.customer) return;
    
    const isLocked = this.confirmActionType === 'lock';
    const action = isLocked ? this.customerService.lockCustomer(this.customer.id) : this.customerService.unlockCustomer(this.customer.id);

    action.subscribe({
      next: (res: ApiResponse<boolean>) => {
        if (res.success) {
          this.customer!.isActive = !isLocked;
          this.toastService.showApiResponse(res);
        } else {
          this.toastService.showApiResponse(res);
        }
        this.closeConfirmModal();
      },
      error: (err: any) => {
        console.error('Lỗi khi cập nhật trạng thái', err);
        if (err.error) {
          this.toastService.showApiResponse(err.error);
        } else {
          this.toastService.error('Đã xảy ra lỗi không xác định');
        }
        this.closeConfirmModal();
      }
    });
  }
}
