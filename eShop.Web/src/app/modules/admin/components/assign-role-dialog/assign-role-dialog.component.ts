import { Component, EventEmitter, Input, Output, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AdminRoleService } from '../../services/admin-role.service';
import { IRole } from '../../models/admin-role.model';

@Component({
  selector: 'app-assign-role-dialog',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div *ngIf="isOpen"
         class="fixed inset-0 z-50 flex items-center justify-center p-4"
         style="background:rgba(0,0,0,0.4); backdrop-filter:blur(4px);">
      <div class="w-full max-w-[420px] rounded-[24px] p-8 shadow-2xl transition-all"
           style="background:var(--admin-modal-bg); border:1px solid var(--admin-modal-border);">
        <h3 class="text-xl font-semibold mb-1" style="color:var(--admin-text-primary);">Gán vai trò</h3>
        <p class="text-sm mb-6" style="color:var(--admin-text-secondary);">
          Chọn vai trò thích hợp cho nhân viên <strong class="text-zinc-900">{{ employeeName }}</strong>.
        </p>

        <div class="flex flex-col gap-3 max-h-[300px] overflow-y-auto pr-2 custom-scrollbar">
          <div *ngIf="loading" class="animate-pulse flex flex-col gap-3">
            <div *ngFor="let s of [1,2,3]" class="h-12 bg-zinc-100 rounded-xl"></div>
          </div>
          
          <div *ngIf="!loading && roles.length === 0" class="text-center py-4 text-sm text-zinc-500">
            Không tìm thấy vai trò nào.
          </div>

          <label *ngFor="let role of roles" 
                 class="flex items-center gap-3 p-3 rounded-xl cursor-pointer transition-colors border"
                 [ngClass]="selectedRoleId === role.id ? 'border-zinc-900 bg-zinc-50/50' : 'border-transparent hover:bg-zinc-50'">
            <input type="radio" 
                   name="role" 
                   [value]="role.id" 
                   [(ngModel)]="selectedRoleId"
                   class="w-4 h-4 text-zinc-900 focus:ring-zinc-900 border-zinc-300" />
            <div class="flex flex-col">
              <span class="text-sm font-medium text-zinc-900">{{ role.name }}</span>
              <span class="text-xs text-zinc-500" *ngIf="role.description">{{ role.description }}</span>
            </div>
          </label>
        </div>

        <div class="mt-8 flex justify-end gap-3">
          <button (click)="closeDialog()"
                  class="px-5 h-[40px] rounded-[12px] text-[13px] font-medium transition-colors"
                  style="color:var(--admin-text-secondary); background:transparent;">Hủy</button>
          <button (click)="submit()"
                  [disabled]="!selectedRoleId"
                  class="px-6 h-[40px] rounded-[12px] text-[13px] font-semibold transition-all active:scale-95 disabled:opacity-40"
                  style="background:var(--admin-text-primary); color:var(--admin-canvas);">Lưu thay đổi</button>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .custom-scrollbar::-webkit-scrollbar { width: 4px; }
    .custom-scrollbar::-webkit-scrollbar-track { background: transparent; }
    .custom-scrollbar::-webkit-scrollbar-thumb { background: #E4E4E7; border-radius: 99px; }
  `]
})
export class AssignRoleDialogComponent implements OnInit {
  @Input() isOpen = false;
  @Input() employeeId: number | null = null;
  @Input() employeeName: string = '';
  @Input() initialRoleId: number | null = null;
  
  @Output() close = new EventEmitter<void>();
  @Output() confirm = new EventEmitter<{ userId: number, roleId: number }>();

  private roleService = inject(AdminRoleService);

  roles: IRole[] = [];
  loading = false;
  selectedRoleId: number | null = null;

  ngOnInit() {
    this.loadRoles();
  }

  ngOnChanges() {
    if (this.isOpen) {
      this.selectedRoleId = this.initialRoleId;
      if (this.roles.length === 0) {
        this.loadRoles();
      }
    }
  }

  loadRoles() {
    this.loading = true;
    this.roleService.fetchRoles().subscribe({
      next: (roles) => {
        this.roles = roles;
        this.loading = false;
      },
      error: () => {
        // Fallback for missing backend API during design phase
        this.roles = [
          { id: 1, name: 'Quản lý kho', description: 'Xem, thêm, sửa dữ liệu kho', isNew: false },
          { id: 2, name: 'Kế toán', description: 'Xem doanh thu, duyệt đơn hàng', isNew: false },
          { id: 3, name: 'Chăm sóc khách hàng', description: 'Xem danh sách KH, hỗ trợ', isNew: false }
        ];
        this.loading = false;
      }
    });
  }

  closeDialog() {
    this.close.emit();
  }

  submit() {
    if (this.employeeId && this.selectedRoleId) {
      this.confirm.emit({ userId: this.employeeId, roleId: this.selectedRoleId });
    }
  }
}
