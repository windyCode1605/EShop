import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

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
      &:hover { background: var(--admin-btn-hover-bg); color: var(--admin-text-primary); }
      &.active {
        background: var(--admin-surface-alt);
        border: 1px solid var(--admin-border);
        color: var(--admin-text-primary);
      }
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
      &::placeholder { color: var(--admin-text-muted); }
      &:focus { border-color: rgba(99,102,241,0.5); box-shadow: 0 0 0 1px rgba(99,102,241,0.25); }
    }

    label { font-size: 12px; font-weight: 500; color: var(--admin-text-secondary); display: block; margin-bottom: 6px; }

    /* Divider */
    .divider { height: 1px; background: var(--admin-border-sub); width: 100%; }

    /* Toggle */
    .toggle-track {
      width: 44px; height: 24px; border-radius: 999px;
      position: relative; cursor: pointer; border: none;
      transition: background 200ms;
      &.on  { background: #6366F1; }
      &.off { background: var(--admin-input-border); border: 1px solid var(--admin-btn-border); }
    }
    .toggle-dot {
      position: absolute; top: 4px;
      width: 16px; height: 16px; border-radius: 50%;
      background: #fff; box-shadow: 0 1px 3px rgba(0,0,0,.3);
      transition: transform 200ms;
    }
    .toggle-track.on  .toggle-dot { transform: translateX(20px); }
    .toggle-track.off .toggle-dot { transform: translateX(3px); }
  `],
  template: `
    <div class="adm-page">

      <!-- Header -->
      <header class="mb-10">
        <h1 class="text-[28px] font-semibold tracking-tight mb-1.5">Cài đặt hệ thống</h1>
        <p style="color:var(--admin-text-secondary);font-size:14px;">Quản lý cấu hình, bảo mật và thông tin hiển thị của EShop.</p>
      </header>

      <div class="grid grid-cols-1 lg:grid-cols-12 gap-10 items-start">

        <!-- Left Nav -->
        <aside class="lg:col-span-3 sticky top-10">
          <nav class="flex flex-col gap-1">
            <button class="nav-btn active">
              Tổng quan
              <svg width="13" height="13" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="m9 18 6-6-6-6"/></svg>
            </button>
            <button class="nav-btn">
              Nhận diện thương hiệu
              <svg width="13" height="13" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="m9 18 6-6-6-6"/></svg>
            </button>
            <button class="nav-btn">
              Bảo mật
              <svg width="13" height="13" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="m9 18 6-6-6-6"/></svg>
            </button>
            <button class="nav-btn">
              Thanh toán &amp; Giao hàng
              <svg width="13" height="13" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="m9 18 6-6-6-6"/></svg>
            </button>
          </nav>
        </aside>

        <!-- Content -->
        <div class="lg:col-span-9 flex flex-col gap-6">

          <!-- Store Info -->
          <section class="section-card">
            <h2 class="text-base font-semibold mb-6">Thông tin cửa hàng</h2>
            <div class="flex flex-col gap-5">
              <div class="grid grid-cols-1 md:grid-cols-2 gap-5">
                <div>
                  <label>Tên cửa hàng</label>
                  <input type="text" class="adm-input" value="MQN EShop Premium" />
                </div>
                <div>
                  <label>Email liên hệ</label>
                  <input type="email" class="adm-input" value="contact@mqn.com" />
                </div>
              </div>
              <div>
                <label>Mô tả ngắn gọn</label>
                <textarea class="adm-input" rows="3" style="resize:none;">Cửa hàng cung cấp các sản phẩm thiết kế và phát triển công nghệ cao cấp.</textarea>
              </div>
            </div>
          </section>

          <!-- Feature Config -->
          <section class="section-card">
            <h2 class="text-base font-semibold mb-6">Cấu hình chức năng</h2>
            <div class="flex flex-col gap-4">
              <!-- Toggle row 1 -->
              <div class="flex items-center justify-between py-1">
                <div>
                  <p class="text-[14px] font-medium" style="color:var(--admin-text-primary);">Chế độ bảo trì</p>
                  <p class="text-[12px] mt-0.5" style="color:var(--admin-text-secondary);">Tạm dừng truy cập từ khách hàng để cập nhật hệ thống.</p>
                </div>
                <button class="toggle-track off"><div class="toggle-dot"></div></button>
              </div>
              <div class="divider"></div>

              <!-- Toggle row 2 -->
              <div class="flex items-center justify-between py-1">
                <div>
                  <p class="text-[14px] font-medium" style="color:var(--admin-text-primary);">Xác thực 2 yếu tố (2FA)</p>
                  <p class="text-[12px] mt-0.5" style="color:var(--admin-text-secondary);">Bắt buộc tất cả Admin phải xác thực 2 bước.</p>
                </div>
                <button class="toggle-track on"><div class="toggle-dot"></div></button>
              </div>
              <div class="divider"></div>

              <!-- Toggle row 3 -->
              <div class="flex items-center justify-between py-1">
                <div>
                  <p class="text-[14px] font-medium" style="color:var(--admin-text-primary);">Đăng ký thành viên mới</p>
                  <p class="text-[12px] mt-0.5" style="color:var(--admin-text-secondary);">Cho phép khách hàng tạo tài khoản trên website.</p>
                </div>
                <button class="toggle-track on"><div class="toggle-dot"></div></button>
              </div>
            </div>
          </section>

          <!-- Save Action -->
          <div class="flex justify-end pt-2">
            <button class="px-6 py-2.5 rounded-[12px] text-[13px] font-semibold transition-all active:scale-95"
                    style="background:var(--admin-text-primary); color:var(--admin-canvas);">
              Lưu cấu hình
            </button>
          </div>

        </div>
      </div>
    </div>
  `
})
export class AdminSettingsComponent {}
