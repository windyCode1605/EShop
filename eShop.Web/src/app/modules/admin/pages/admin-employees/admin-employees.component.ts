import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AdminEmployeeService } from '../../services/admin-employee.service';
import { IAdminEmployee, IAdminEmployeeQuery } from '../../models/admin-employee.model';
import { AssignRoleDialogComponent } from '../../components/assign-role-dialog/assign-role-dialog.component';
import { ToastService } from '../../../../core/services/toast.service';
import { finalize } from 'rxjs';

@Component({
  selector: 'app-admin-employees',
  standalone: true,
  imports: [CommonModule, FormsModule, AssignRoleDialogComponent],
  styles: [`
    .emp-row { transition: background 150ms; }
    .emp-row:hover { background: var(--admin-table-row-hover); }
  `],
  template: `
    <div class="min-h-screen w-full" style="background:var(--admin-canvas); color:var(--admin-text-primary);">
      <div class="px-8 py-8 max-w-[1280px] mx-auto">
        
        <!-- Header -->
        <header class="flex items-end justify-between mb-8">
          <div>
            <h1 class="text-[28px] font-semibold tracking-tight mb-1.5" style="color:var(--admin-text-primary);">Nhân viên</h1>
            <p class="text-sm" style="color:var(--admin-text-secondary);">Quản lý tài khoản và phân quyền thao tác cho nội bộ</p>
          </div>
          <button class="h-[40px] px-5 rounded-[12px] text-[13px] font-medium flex items-center gap-2 transition-colors hover:opacity-90"
                  style="background:var(--admin-text-primary); color:var(--admin-canvas);">
            <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M5 12h14"/><path d="M12 5v14"/></svg>
            Thêm nhân viên
          </button>
        </header>

        <!-- Filter Bar -->
        <div class="flex items-center gap-4 mb-6">
          <!-- Keyword Search -->
          <div class="relative flex-1 max-w-[400px]">
            <svg class="absolute left-3.5 top-1/2 -translate-y-1/2 pointer-events-none" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="#71717A" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><circle cx="11" cy="11" r="8"/><path d="m21 21-4.3-4.3"/></svg>
            <input
              type="text"
              [(ngModel)]="query.keyword"
              (keyup.enter)="onSearch()"
              placeholder="Tìm kiếm theo Tên, Email hoặc Số điện thoại..."
              class="w-full pl-10 pr-4 h-[44px] rounded-[16px] text-[14px] transition-all focus:ring-2 focus:ring-zinc-200"
              style="background:var(--admin-surface); border:1px solid var(--admin-input-border); color:var(--admin-text-primary); outline: none;" />
          </div>

          <!-- Status Filter (Optional for future) -->
          <div class="h-[44px] px-4 rounded-[16px] flex items-center border cursor-pointer transition-colors"
               style="background:var(--admin-surface); border-color:var(--admin-input-border);">
            <span class="text-[13px] font-medium" style="color:var(--admin-text-secondary);">Trạng thái: Tất cả</span>
          </div>
          
          <div class="flex-1"></div>
        </div>

        <!-- Employee List (Minimalist List View) -->
        <div class="rounded-[24px] overflow-hidden shadow-sm" style="background:var(--admin-surface); border:1px solid var(--admin-border);">
          
          <!-- Header Row -->
          <div class="grid grid-cols-12 items-center px-6 py-4 text-[12px] font-semibold tracking-wider uppercase"
               style="background:var(--admin-table-header); color:var(--admin-text-muted); border-bottom:1px solid var(--admin-border);">
            <div class="col-span-4">Nhân viên</div>
            <div class="col-span-3">Liên hệ</div>
            <div class="col-span-3">Ngày tham gia</div>
            <div class="col-span-2 text-right">Thao tác</div>
          </div>

          <!-- Loading State -->
          <div *ngIf="loading()" class="p-6">
            <div *ngFor="let s of [1,2,3,4]" class="animate-pulse flex items-center h-16 border-b border-zinc-100 last:border-0">
              <div class="w-10 h-10 rounded-full bg-zinc-100 mr-4"></div>
              <div class="flex-1 space-y-2">
                <div class="h-4 bg-zinc-100 rounded w-1/4"></div>
                <div class="h-3 bg-zinc-100 rounded w-1/3"></div>
              </div>
            </div>
          </div>

          <!-- Data Rows -->
          <div *ngIf="!loading()">
            <div *ngFor="let emp of employees(); let last = last" 
                 class="emp-row grid grid-cols-12 items-center px-6 py-4"
                 [style.border-bottom]="last ? 'none' : '1px solid var(--admin-border)'">
              
              <!-- Employee Info -->
              <div class="col-span-4 flex items-center gap-4">
                <div class="w-10 h-10 rounded-full flex items-center justify-center font-semibold text-sm"
                     style="background: var(--admin-input-bg); border: 1px solid var(--admin-border); color: var(--admin-text-primary);">
                  {{ getInitials(emp.username || emp.email) }}
                </div>
                <div class="flex flex-col">
                  <span class="text-[14px] font-medium" style="color:var(--admin-text-primary);">{{ emp.username || 'N/A' }}</span>
                  <span class="text-[13px]" style="color:var(--admin-text-secondary);">{{ emp.email }}</span>
                </div>
              </div>

              <!-- Contact Info -->
              <div class="col-span-3 flex flex-col justify-center">
                <span class="text-[14px]" style="color:var(--admin-text-secondary);">{{ emp.phoneNumber || '—' }}</span>
              </div>

              <!-- Joined Date -->
              <div class="col-span-3 flex items-center">
                <span class="text-[13px]" style="color:var(--admin-text-secondary);">{{ emp.creatDate | date:'dd/MM/yyyy' }}</span>
              </div>

              <!-- Actions -->
              <div class="col-span-2 flex items-center justify-end gap-2">
                <!-- Assign Role Button -->
                <button (click)="openRoleDialog(emp)" 
                        title="Gán vai trò"
                        class="w-8 h-8 rounded-full flex items-center justify-center transition-colors hover:opacity-70"
                        style="color:var(--admin-text-primary);">
                  <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M12 22s8-4 8-10V5l-8-3-8 3v7c0 6 8 10 8 10z"/></svg>
                </button>
              </div>

            </div>
            
            <!-- Empty State -->
            <div *ngIf="employees().length === 0" class="flex flex-col items-center justify-center py-20 text-center">
              <svg width="40" height="40" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round" class="mb-4" style="color:var(--admin-text-muted);"><circle cx="11" cy="11" r="8"/><path d="m21 21-4.3-4.3"/></svg>
              <h3 class="text-[15px] font-medium mb-1" style="color:var(--admin-text-primary);">Không có dữ liệu</h3>
              <p class="text-[13px] max-w-sm" style="color:var(--admin-text-secondary);">Không tìm thấy nhân viên nào phù hợp với từ khóa tìm kiếm của bạn.</p>
            </div>
          </div>
          
        </div>
      </div>
    </div>

    <!-- Dialog -->
    <app-assign-role-dialog 
      [isOpen]="showRoleDialog"
      [employeeId]="selectedEmployee?.id || null"
      [employeeName]="selectedEmployee?.username || selectedEmployee?.email || ''"
      (close)="showRoleDialog = false"
      (confirm)="onAssignRoleConfirm($event)">
    </app-assign-role-dialog>
  `
})
export class AdminEmployeesComponent implements OnInit {
  private employeeService = inject(AdminEmployeeService);
  private toastService = inject(ToastService);

  loading = signal<boolean>(false);
  employees = signal<IAdminEmployee[]>([]);
  
  query: IAdminEmployeeQuery = {
    pageNumber: 1,
    pageSize: 20,
    keyword: ''
  };

  showRoleDialog = false;
  selectedEmployee: IAdminEmployee | null = null;

  ngOnInit() {
    this.loadData();
  }

  loadData() {
    this.loading.set(true);
    this.employeeService.getEmployees(this.query).pipe(
      finalize(() => this.loading.set(false))
    ).subscribe({
      next: (res) => {
        if (res.statusCode === 200 && res.data) {
          this.employees.set(res.data.items || []);
        } else {
          this.employees.set([]);
        }
      },
      error: (err) => {
        this.toastService.error(err.message || 'Lỗi khi tải danh sách nhân viên', 'Thất bại');
        this.employees.set([]);
      }
    });
  }

  onSearch() {
    this.query.pageNumber = 1;
    this.loadData();
  }

  openRoleDialog(emp: IAdminEmployee) {
    this.selectedEmployee = emp;
    this.showRoleDialog = true;
  }

  onAssignRoleConfirm(event: { userId: number, roleId: number }) {
    this.employeeService.assignRole(event.userId, event.roleId).subscribe({
      next: (res) => {
        if (res.statusCode === 200) {
          this.toastService.success('Đã gán vai trò thành công', 'Thành công');
          this.showRoleDialog = false;
          // Có thể load lại list hoặc update state nếu cần hiển thị Role badge
        } else {
          this.toastService.error(res.message, 'Lỗi');
        }
      },
      error: (err) => {
        this.toastService.error('Có lỗi xảy ra khi gán vai trò', 'Lỗi');
      }
    });
  }

  getInitials(name?: string | null): string {
    if (!name) return '?';
    return name.substring(0, 2).toUpperCase();
  }
}
