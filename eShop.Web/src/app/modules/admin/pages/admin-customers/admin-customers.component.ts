import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { finalize } from 'rxjs/operators';
import { AdminCustomerService } from '../../services/admin-customer.service';
import { ICustomerListItem, IAdminCustomerFilter } from '../../models/admin-customer.model';
import { Subject } from 'rxjs';
import { debounceTime } from 'rxjs/operators';
import { HttpErrorResponse } from '@angular/common/http';
import { ToastService } from '../../../../core/services/toast.service';
import { ApiResponse } from '../../../../core/models';

@Component({
  selector: 'app-admin-customers',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './admin-customers.component.html'
})
export class AdminCustomersComponent implements OnInit {
  private customerService = inject(AdminCustomerService);
  private toastService = inject(ToastService);

  customers = signal<ICustomerListItem[]>([]);
  totalCount = signal<number>(0);
  hasNext = signal<boolean>(false);
  hasPrev = signal<boolean>(false);
  loading = signal<boolean>(false);
  errorMessage = signal<string>('');

  filter: IAdminCustomerFilter = {
    pageIndex: 1,
    pageSize: 10,
    keyword: '',
    isActive: undefined,
    fromDate: '',
    toDate: ''
  };

  private searchSubject = new Subject<string>();

  ngOnInit(): void {
    this.searchSubject.pipe(
      debounceTime(500)
    ).subscribe(() => {
      this.filter.pageIndex = 1;
      this.loadData();
    });

    this.loadData();
  }

  loadData() {
    this.loading.set(true);
    this.errorMessage.set('');

    this.customerService.getCustomers(this.filter)
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: (res) => {
          this.customers.set(res.items);
          this.totalCount.set(res.totalCount);
          this.hasNext.set(res.hasNext);
          this.hasPrev.set(res.hasPrev);
        },
        error: (err: HttpErrorResponse) => {
          console.error('Error fetching customers:', err);
          this.errorMessage.set(err.error?.message || 'Đã xảy ra lỗi khi tải danh sách khách hàng. Vui lòng thử lại sau.');
        }
      });
  }

  onSearchChange(value: string) {
    this.searchSubject.next(value);
  }

  min(a: number, b: number): number {
    return Math.min(a, b);
  }

  nextPage() {
    if (this.hasNext()) {
      this.filter.pageIndex++;
      this.loadData();
    }
  }

  prevPage() {
    if (this.hasPrev()) {
      this.filter.pageIndex--;
      this.loadData();
    }
  }

  showConfirmModal = false;
  confirmCustomer: ICustomerListItem | null = null;
  confirmActionType: 'lock' | 'unlock' = 'lock';

  openConfirmModal(event: Event, c: ICustomerListItem) {
    event.stopPropagation();
    this.confirmCustomer = c;
    this.confirmActionType = c.isActive ? 'lock' : 'unlock';
    this.showConfirmModal = true;
  }

  closeConfirmModal() {
    this.showConfirmModal = false;
    this.confirmCustomer = null;
  }

  executeCustomerStatusChange() {
    if (!this.confirmCustomer) return;
    
    const c = this.confirmCustomer;
    const action = c.isActive ? this.customerService.lockCustomer(c.id) : this.customerService.unlockCustomer(c.id);
    
    action.subscribe({
      next: (res: ApiResponse<boolean>) => {
        if (res.success) {
          c.isActive = !c.isActive;
          this.toastService.showApiResponse(res);
        } else {
          this.toastService.showApiResponse(res);
        }
        this.closeConfirmModal();
      },
      error: (err: any) => {
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
