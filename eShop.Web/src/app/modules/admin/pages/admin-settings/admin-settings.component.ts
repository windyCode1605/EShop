import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { SysvarService } from '../../../../core/services/sysvar.service';
import { SysvarResponseDto } from '../../../../core/models/sysvar.model';
import { ToastService } from '../../../../core/services/toast.service';

@Component({
  selector: 'app-admin-settings',
  standalone: true,
  imports: [CommonModule, FormsModule],
  styles: [`
    :host { display: block; }

    .adm-page {
      padding: 40px 64px;
      min-height: 100%;
      background: var(--admin-canvas);
      color: var(--admin-text-primary);
    }

    h1, h2 { color: var(--admin-text-primary); }

    /* Nav */
    .nav-btn {
      width: 100%; text-align: left;
      padding: 9px 14px; border-radius: 10px;
      font-size: 13px; font-weight: 500;
      background: transparent; border: none; cursor: pointer;
      color: var(--admin-text-secondary);
      display: flex; align-items: center; justify-content: space-between;
      transition: background 150ms, color 150ms;
    }
    .nav-btn:hover { background: var(--admin-btn-hover-bg); color: var(--admin-text-primary); }
    .nav-btn.active {
      background: var(--admin-surface-alt);
      border: 1px solid var(--admin-border);
      color: var(--admin-text-primary);
    }

    /* Section card */
    .section-card {
      background: var(--admin-surface);
      border: 1px solid var(--admin-border);
      border-radius: 20px;
      padding: 32px;
    }

    /* Input */
    .adm-input {
      width: 100%;
      background: var(--admin-input-bg);
      border: 1px solid var(--admin-input-border);
      color: var(--admin-text-primary);
      border-radius: 12px;
      padding: 11px 16px;
      font-size: 14px;
      outline: none;
      transition: border-color 150ms;
    }
    .adm-input::placeholder { color: var(--admin-text-muted); }
    .adm-input:focus { border-color: rgba(99,102,241,0.5); box-shadow: 0 0 0 1px rgba(99,102,241,0.25); }

    label { font-size: 13px; font-weight: 500; color: var(--admin-text-primary); display: block; margin-bottom: 4px; }
    .helper-text { font-size: 12px; font-weight: 400; color: var(--admin-text-secondary); margin-bottom: 8px; display: block; }

    /* Divider */
    .divider { height: 1px; background: var(--admin-border-sub); width: 100%; }

    /* Buttons */
    .btn-save {
      background: var(--admin-text-primary);
      color: var(--admin-canvas);
      padding: 8px 16px;
      border-radius: 10px;
      font-size: 13px;
      font-weight: 500;
      cursor: pointer;
      border: none;
      transition: all 150ms;
    }
    .btn-save:active { transform: translateY(1px); }
    .btn-save:disabled { opacity: 0.6; cursor: not-allowed; }
  `],
  template: `
    <div class="adm-page">

      <!-- Header -->
      <header class="mb-10">
        <h1 class="text-[28px] font-semibold tracking-tight mb-1.5">Cài đặt hệ thống</h1>
        <p style="color:var(--admin-text-secondary);font-size:14px;">Quản lý cấu hình biến môi trường (SysVar) và thông số kỹ thuật.</p>
      </header>

      <div class="grid grid-cols-1 lg:grid-cols-12 gap-10 items-start">

        <!-- Left Nav -->
        <aside class="lg:col-span-3 sticky top-10">
          <nav class="flex flex-col gap-1">
            <button class="nav-btn" [class.active]="activeTab === 'GENERAL'" (click)="activeTab = 'GENERAL'">
              Tổng quan hệ thống
              <svg *ngIf="activeTab === 'GENERAL'" width="13" height="13" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="m9 18 6-6-6-6"/></svg>
            </button>
            
            <!-- Skeleton for Nav -->
            <ng-container *ngIf="isLoading">
              <div class="h-[38px] bg-[var(--admin-border-sub)] animate-pulse rounded-[10px] mt-1 w-3/4"></div>
              <div class="h-[38px] bg-[var(--admin-border-sub)] animate-pulse rounded-[10px] mt-1 w-full"></div>
            </ng-container>

            <!-- Dynamic groups -->
            <button *ngFor="let group of groups" class="nav-btn" [class.active]="activeTab === group" (click)="activeTab = group">
              Cấu hình {{ group }}
              <svg *ngIf="activeTab === group" width="13" height="13" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="m9 18 6-6-6-6"/></svg>
            </button>
          </nav>
        </aside>

        <!-- Content -->
        <div class="lg:col-span-9 flex flex-col gap-6">

          <!-- Skeleton Loader -->
          <ng-container *ngIf="isLoading">
            <section class="section-card">
              <div class="h-5 bg-[var(--admin-border-sub)] animate-pulse rounded mb-6 w-1/4"></div>
              <div class="flex flex-col gap-6">
                <div>
                  <div class="h-4 bg-[var(--admin-border-sub)] animate-pulse rounded mb-2 w-1/3"></div>
                  <div class="h-[44px] bg-[var(--admin-border-sub)] animate-pulse rounded-xl w-full"></div>
                </div>
                <div class="divider"></div>
                <div>
                  <div class="h-4 bg-[var(--admin-border-sub)] animate-pulse rounded mb-2 w-1/4"></div>
                  <div class="h-[44px] bg-[var(--admin-border-sub)] animate-pulse rounded-xl w-full"></div>
                </div>
              </div>
            </section>
          </ng-container>

          <ng-container *ngIf="!isLoading">
            
            <!-- General Tab -->
            <section class="section-card" *ngIf="activeTab === 'GENERAL'">
              <h2 class="text-base font-semibold mb-6">Thông tin cửa hàng</h2>
              <div class="flex flex-col gap-5">
                <div class="grid grid-cols-1 md:grid-cols-2 gap-5">
                  <div>
                    <label>Tên cửa hàng</label>
                    <input type="text" class="adm-input" value="MQN EShop Premium" readonly />
                  </div>
                  <div>
                    <label>Email liên hệ</label>
                    <input type="email" class="adm-input" value="contact@mqn.com" readonly />
                  </div>
                </div>
                <div>
                  <label>Mô tả ngắn gọn</label>
                  <textarea class="adm-input" rows="3" style="resize:none;" readonly>Cửa hàng cung cấp các sản phẩm thiết kế và phát triển công nghệ cao cấp.</textarea>
                </div>
              </div>
            </section>

            <!-- Dynamic SysVar Tabs -->
            <ng-container *ngFor="let group of groups">
              <section class="section-card" *ngIf="activeTab === group">
                <h2 class="text-base font-semibold mb-6">Biến hệ thống - {{ group }}</h2>
                <div class="flex flex-col gap-6">
                  
                  <ng-container *ngFor="let v of groupedVars[group]; let last = last; let i = index">
                    <div class="flex flex-col">
                      <div class="flex items-start justify-between gap-4">
                        <div class="flex-1">
                          <label>{{ v.varName }}</label>
                          <span class="helper-text">{{ v.varDesc || 'Không có mô tả' }}</span>
                          <input [type]="getInputType(v.varName)" class="adm-input" [(ngModel)]="v.varValue" />
                        </div>
                        <div class="pt-[26px]">
                          <button class="btn-save" [disabled]="savingVarId === v.id" (click)="saveSysVar(v)">
                            {{ savingVarId === v.id ? 'Đang lưu...' : 'Lưu' }}
                          </button>
                        </div>
                      </div>
                    </div>
                    <div *ngIf="!last" class="divider"></div>
                  </ng-container>

                </div>
              </section>
            </ng-container>

          </ng-container>

        </div>
      </div>
    </div>
  `
})
export class AdminSettingsComponent implements OnInit {
  sysVars: SysvarResponseDto[] = [];
  groups: string[] = [];
  groupedVars: Record<string, SysvarResponseDto[]> = {};
  
  activeTab: string = 'GENERAL';
  isLoading: boolean = true;
  savingVarId: number | null = null;

  private sysvarService = inject(SysvarService);
  private toast = inject(ToastService);

  // Danh sách các biến cần ép kiểu số trên giao diện
  private readonly numericVars = [
    'LOGIN_MAX_TURN', 
    'OTP_MAX_TURN', 
    'SECOND', 
    'OTP_LENGTH', 
    'OTP_RESEND_COOLDOWN'
  ];

  getInputType(varName: string): string {
    return this.numericVars.includes(varName) ? 'number' : 'text';
  }

  ngOnInit() {
    this.fetchData();
  }

  fetchData() {
    this.isLoading = true;
    this.sysvarService.getAllSysVars().subscribe({
      next: (res) => {
        if (res.success && res.data) {
          this.sysVars = res.data;
          this.groupData();
          if (this.groups.length > 0 && this.activeTab !== 'GENERAL') {
             // Keep active tab if exists
             if (!this.groups.includes(this.activeTab)) {
               this.activeTab = 'GENERAL';
             }
          }
        }
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Lỗi khi lấy SysVar', err);
        this.isLoading = false;
      }
    });
  }

  groupData() {
    this.groupedVars = {};
    this.groups = [];
    
    for (const v of this.sysVars) {
      const g = v.grName || 'UNGROUPED';
      if (!this.groupedVars[g]) {
        this.groupedVars[g] = [];
        this.groups.push(g);
      }
      this.groupedVars[g].push(v);
    }
  }

  saveSysVar(v: SysvarResponseDto) {
    this.savingVarId = v.id;
    this.sysvarService.updateSysVar(v.id, { varValue: v.varValue }).subscribe({
      next: (res) => {
        this.savingVarId = null;
        this.toast.showApiResponse(res, { defaultSuccessMessage: 'Lưu cấu hình thành công!' });
      },
      error: (err) => {
        this.savingVarId = null;
        console.error('Lỗi hệ thống khi lưu SysVar', err);
        this.toast.error('Lỗi hệ thống khi lưu cấu hình');
      }
    });
  }
}
