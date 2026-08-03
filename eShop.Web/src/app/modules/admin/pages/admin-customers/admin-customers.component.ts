import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { finalize } from 'rxjs/operators';
import { AdminCustomerService } from '../../services/admin-customer.service';
import { ICustomerListItem, IAdminCustomerFilter } from '../../models/admin-customer.model';
import { Subject } from 'rxjs';
import { debounceTime } from 'rxjs/operators';
import { HttpErrorResponse } from '@angular/common/http';

@Component({
  selector: 'app-admin-customers',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './admin-customers.component.html'
})
export class AdminCustomersComponent implements OnInit {
  private customerService = inject(AdminCustomerService);

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
}
