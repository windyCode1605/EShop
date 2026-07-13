import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet, RouterModule } from '@angular/router';

@Component({
  selector: 'app-admin-layout',
  standalone: true,
  imports: [CommonModule, RouterOutlet, RouterModule],
  styles: [`
    /* ─────────────────────────────────────────────────────────
       ADMIN LAYOUT — Industrial Minimalist
    ───────────────────────────────────────────────────────── */
    .admin-layout {
      min-height: 100dvh;
      background: #F9FAFB;
      color: #18181B;
      font-family: 'Geist', 'Inter', sans-serif;
      display: flex;
    }

    /* Sidebar - Minimalist */
    .sidebar {
      width: 240px;
      background: #FFFFFF;
      border-right: 1px solid #E4E4E7;
      padding: 28px 20px;
      display: flex;
      flex-direction: column;
      position: sticky;
      top: 0;
      height: 100vh;
      overflow-y: auto;
    }

    .brand {
      font-size: 16px;
      font-weight: 700;
      letter-spacing: -0.5px;
      margin-bottom: 32px;
      display: flex;
      align-items: center;
      gap: 10px;
    }

    .brand__mark {
      width: 22px;
      height: 22px;
      background: #18181B;
      border-radius: 6px;
    }

    .nav-section-label {
      font-size: 11px;
      font-weight: 600;
      color: #A1A1AA;
      text-transform: uppercase;
      letter-spacing: 0.8px;
      padding: 0 10px;
      margin: 20px 0 6px;
    }
    .nav-section-label:first-of-type { margin-top: 0; }

    .nav-list {
      list-style: none;
      padding: 0;
      margin: 0;
      display: flex;
      flex-direction: column;
      gap: 2px;
    }

    .nav-item {
      display: flex;
      align-items: center;
      gap: 10px;
      padding: 9px 12px;
      border-radius: 10px;
      font-size: 13px;
      font-weight: 500;
      color: #71717A;
      text-decoration: none;
      transition: all 150ms ease;
      cursor: pointer;
    }

    .nav-item:hover {
      background: #F4F4F5;
      color: #18181B;
    }

    .nav-item.active {
      background: #18181B;
      color: #FFFFFF;
    }

    .nav-divider {
      height: 1px;
      background: #F4F4F5;
      margin: 12px 0;
    }

    /* Main Content Wrapper */
    .main-wrapper {
      flex: 1;
      display: flex;
      flex-direction: column;
      overflow-y: auto;
      height: 100vh;
    }
  `],
  template: `
    <div class="admin-layout">
      <!-- Sidebar -->
      <aside class="sidebar">
        <div class="brand">
          <div class="brand__mark"></div>
          ATELIER
        </div>

        <nav>
          <div class="nav-section-label">Tổng quan</div>
          <ul class="nav-list">
            <a routerLink="/admin/dashboard" routerLinkActive="active" class="nav-item" id="nav-dashboard">
              <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24"
                   fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                <rect width="7" height="9" x="3" y="3" rx="1"/>
                <rect width="7" height="5" x="14" y="3" rx="1"/>
                <rect width="7" height="9" x="14" y="12" rx="1"/>
                <rect width="7" height="5" x="3" y="16" rx="1"/>
              </svg>
              Overview
            </a>
          </ul>

          <div class="nav-divider"></div>
          <div class="nav-section-label">Danh mục</div>
          <ul class="nav-list">
            <a routerLink="/admin/categories" routerLinkActive="active" class="nav-item" id="nav-categories">
              <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24"
                   fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                <path d="M2 3h6a4 4 0 0 1 4 4v14a3 3 0 0 0-3-3H2z"/>
                <path d="M22 3h-6a4 4 0 0 0-4 4v14a3 3 0 0 1 3-3h7z"/>
              </svg>
              Danh mục
            </a>
          </ul>

          <div class="nav-divider"></div>
          <div class="nav-section-label">Sản phẩm</div>
          <ul class="nav-list">
            <a routerLink="/admin/products" routerLinkActive="active" class="nav-item" id="nav-products">
              <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24"
                   fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                <path d="m7.5 4.27 9 5.15"/>
                <path d="M21 8a2 2 0 0 0-1-1.73l-7-4a2 2 0 0 0-2 0l-7 4A2 2 0 0 0 3 8v8a2 2 0 0 0 1 1.73l7 4a2 2 0 0 0 2 0l7-4A2 2 0 0 0 21 16Z"/>
                <path d="m3.3 7 8.7 5 8.7-5"/>
                <path d="M12 22V12"/>
              </svg>
              Sản phẩm
            </a>
          </ul>

          <div class="nav-divider"></div>
          <div class="nav-section-label">Vận hành</div>
          <ul class="nav-list">
            <a routerLink="/admin/orders" routerLinkActive="active" class="nav-item" id="nav-orders">
              <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24"
                   fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                <path d="M6 2 3 6v14a2 2 0 0 0 2 2h14a2 2 0 0 0 2-2V6l-3-4Z"/>
                <path d="M3 6h18"/>
                <path d="M16 10a4 4 0 0 1-8 0"/>
              </svg>
              Đơn hàng
            </a>
            <a routerLink="/admin/customers" routerLinkActive="active" class="nav-item" id="nav-customers">
              <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24"
                   fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                <path d="M16 21v-2a4 4 0 0 0-4-4H6a4 4 0 0 0-4 4v2"/>
                <circle cx="9" cy="7" r="4"/>
                <path d="M22 21v-2a4 4 0 0 0-3-3.87"/>
                <path d="M16 3.13a4 4 0 0 1 0 7.75"/>
              </svg>
              Khách hàng
            </a>
          </ul>
        </nav>
      </aside>

      <!-- Main Content Outlet -->
      <main class="main-wrapper">
        <router-outlet></router-outlet>
      </main>
    </div>
  `
})
export class AdminLayoutComponent { }
