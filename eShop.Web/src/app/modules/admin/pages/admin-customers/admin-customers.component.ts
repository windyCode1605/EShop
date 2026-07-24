import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-admin-customers',
  standalone: true,
  imports: [CommonModule],
  styles: [`
    :host { display: block; }

    .adm-page {
      padding: 40px 64px;
      min-height: 100%;
      background: var(--admin-canvas);
      color: var(--admin-text-primary);
    }

    h1 { color: var(--admin-text-primary); }
    .subtitle { color: var(--admin-text-secondary); }

    /* Search input */
    .search-field {
      background: var(--admin-input-bg);
      border: 1px solid var(--admin-input-border);
      color: var(--admin-text-primary);
      border-radius: 10px;
      padding: 9px 14px 9px 36px;
      font-size: 13px;
      outline: none;
      transition: border-color 150ms;
      width: 240px;
      &::placeholder { color: var(--admin-text-muted); }
      &:focus { border-color: rgba(99,102,241,0.5); }
    }

    /* Table wrapper */
    .table-card {
      background: var(--admin-surface);
      border: 1px solid var(--admin-border);
      border-radius: 16px;
      overflow: hidden;
    }

    table { width: 100%; border-collapse: collapse; text-align: left; }

    thead tr { background: var(--admin-table-header); }
    th {
      padding: 14px 24px;
      font-size: 11px;
      font-weight: 600;
      text-transform: uppercase;
      letter-spacing: 0.06em;
      color: var(--admin-text-secondary);
      border-bottom: 1px solid var(--admin-border);
    }

    td {
      padding: 18px 24px;
      font-size: 14px;
      border-bottom: 1px solid var(--admin-border-sub);
      color: var(--admin-text-primary);
    }

    tbody tr:last-child td { border-bottom: none; }
    tbody tr:hover td { background: var(--admin-table-row-hover); }

    /* Avatar */
    .avatar {
      width: 38px; height: 38px; border-radius: 50%;
      display: flex; align-items: center; justify-content: center;
      font-size: 12px; font-weight: 600;
      background: var(--admin-surface-alt);
      color: var(--admin-text-secondary);
      border: 1px solid var(--admin-border);
      flex-shrink: 0;
    }

    .name-cell { font-weight: 500; }
    .email-cell { font-size: 12px; color: var(--admin-text-secondary); margin-top: 2px; }

    /* Action btn */
    .action-btn {
      padding: 6px 8px; border-radius: 8px; border: none;
      background: transparent; cursor: pointer;
      color: var(--admin-text-muted);
      transition: background 150ms, color 150ms;
      &:hover { background: var(--admin-btn-hover-bg); color: var(--admin-text-primary); }
    }

    /* Pagination */
    .pagination { border-top: 1px solid var(--admin-border); }
    .page-btn {
      padding: 6px 14px; border-radius: 8px;
      border: 1px solid var(--admin-btn-border);
      background: var(--admin-btn-bg);
      color: var(--admin-btn-text);
      font-size: 13px; cursor: pointer;
      transition: background 150ms;
      &:hover { background: var(--admin-btn-hover-bg); }
      &:disabled { opacity: 0.4; cursor: not-allowed; }
    }
  `],
  template: `
    <div class="adm-page">

      <!-- Header -->
      <header class="flex items-center justify-between mb-8">
        <div>
          <h1 class="text-[28px] font-semibold tracking-tight mb-1">Khách hàng</h1>
          <p class="subtitle text-sm">Quản lý và theo dõi thông tin người dùng trên hệ thống.</p>
        </div>
        <div class="flex items-center gap-3">
          <!-- Search -->
          <div style="position:relative;">
            <svg style="position:absolute;left:10px;top:50%;transform:translateY(-50%);pointer-events:none;" width="14" height="14" viewBox="0 0 24 24" fill="none" [attr.stroke]="'var(--admin-text-muted)'" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><circle cx="11" cy="11" r="8"/><path d="m21 21-4.3-4.3"/></svg>
            <input type="text" class="search-field" placeholder="Tìm kiếm khách hàng..." />
          </div>
          <!-- Add button -->
          <button class="flex items-center gap-2 px-4 py-2 rounded-[10px] text-[13px] font-semibold"
                  style="background:var(--admin-text-primary); color:var(--admin-canvas);">
            <svg width="13" height="13" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M5 12h14"/><path d="M12 5v14"/></svg>
            Thêm mới
          </button>
        </div>
      </header>

      <!-- Table -->
      <div class="table-card">
        <table>
          <thead>
            <tr>
              <th>Khách hàng</th>
              <th>Trạng thái</th>
              <th>Số đơn hàng</th>
              <th>Tổng chi tiêu</th>
              <th style="text-align:right;">Thao tác</th>
            </tr>
          </thead>
          <tbody>
            <tr *ngFor="let c of customers">
              <td>
                <div style="display:flex;align-items:center;gap:12px;">
                  <div class="avatar">{{ c.initials }}</div>
                  <div>
                    <div class="name-cell">{{ c.name }}</div>
                    <div class="email-cell">{{ c.email }}</div>
                  </div>
                </div>
              </td>
              <td>
                <span class="inline-flex items-center gap-1.5 px-2.5 py-1 rounded-full text-xs font-medium"
                      [style.background]="c.active ? 'rgba(16,185,129,0.1)' : 'rgba(239,68,68,0.1)'"
                      [style.color]="c.active ? '#10B981' : '#F87171'">
                  <span class="w-1.5 h-1.5 rounded-full inline-block"
                        [style.background]="c.active ? '#10B981' : '#F87171'"></span>
                  {{ c.active ? 'Hoạt động' : 'Vô hiệu hóa' }}
                </span>
              </td>
              <td>{{ c.orders }} đơn</td>
              <td style="font-weight:500;">{{ c.spent }}</td>
              <td style="text-align:right;">
                <button class="action-btn">
                  <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><circle cx="12" cy="12" r="1"/><circle cx="19" cy="12" r="1"/><circle cx="5" cy="12" r="1"/></svg>
                </button>
              </td>
            </tr>
          </tbody>
        </table>

        <!-- Pagination -->
        <div class="pagination px-6 py-4 flex items-center justify-between">
          <span style="font-size:13px;color:var(--admin-text-secondary);">Hiển thị 1 đến {{ customers.length }} của 24 khách hàng</span>
          <div class="flex items-center gap-2">
            <button class="page-btn" disabled>Trước</button>
            <button class="page-btn">Tiếp</button>
          </div>
        </div>
      </div>

    </div>
  `
})
export class AdminCustomersComponent {
  customers = [
    { initials: 'MQ', name: 'Mai Quang Nguyễn', email: 'mqnguyen@example.com', active: true,  orders: 24, spent: '$1,250.00' },
    { initials: 'AD', name: 'Alex Doe',           email: 'alex.doe@example.com',  active: true,  orders: 5,  spent: '$320.00'   },
    { initials: 'SL', name: 'Sarah Lee',          email: 'sarah.lee@example.com', active: false, orders: 0,  spent: '$0.00'     },
  ];
}
