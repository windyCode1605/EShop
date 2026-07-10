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
      width: 280px;
      background: #FFFFFF;
      border-right: 1px solid #E4E4E7;
      padding: 32px 24px;
      display: flex;
      flex-direction: column;
      position: sticky;
      top: 0;
      height: 100vh;
    }

    .brand {
      font-size: 18px;
      font-weight: 700;
      letter-spacing: -0.5px;
      margin-bottom: 48px;
      display: flex;
      align-items: center;
      gap: 12px;
    }

    .brand__mark {
      width: 24px;
      height: 24px;
      background: #18181B;
      border-radius: 4px;
    }

    .nav-list {
      list-style: none;
      padding: 0;
      margin: 0;
      display: flex;
      flex-direction: column;
      gap: 8px;
    }

    .nav-item {
      display: flex;
      align-items: center;
      gap: 12px;
      padding: 10px 14px;
      border-radius: 8px;
      font-size: 14px;
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
          ATELIER Admin
        </div>
        <nav>
          <ul class="nav-list">
            <a routerLink="/admin/dashboard" routerLinkActive="active" class="nav-item">
              <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><rect width="7" height="9" x="3" y="3" rx="1"/><rect width="7" height="5" x="14" y="3" rx="1"/><rect width="7" height="9" x="14" y="12" rx="1"/><rect width="7" height="5" x="3" y="16" rx="1"/></svg>
              Overview
            </a>
            <a routerLink="/admin/products" routerLinkActive="active" class="nav-item">
              <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="m7.5 4.27 9 5.15"/><path d="M21 8a2 2 0 0 0-1-1.73l-7-4a2 2 0 0 0-2 0l-7 4A2 2 0 0 0 3 8v8a2 2 0 0 0 1 1.73l7 4a2 2 0 0 0 2 0l7-4A2 2 0 0 0 21 16Z"/><path d="m3.3 7 8.7 5 8.7-5"/><path d="M12 22V12"/></svg>
              Products
            </a>
            <a routerLink="/admin/orders" routerLinkActive="active" class="nav-item">
              <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M6 2 3 6v14a2 2 0 0 0 2 2h14a2 2 0 0 0 2-2V6l-3-4Z"/><path d="M3 6h18"/><path d="M16 10a4 4 0 0 1-8 0"/></svg>
              Orders
            </a>
            <a routerLink="/admin/customers" routerLinkActive="active" class="nav-item">
              <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M16 21v-2a4 4 0 0 0-4-4H6a4 4 0 0 0-4 4v2"/><circle cx="9" cy="7" r="4"/><path d="M22 21v-2a4 4 0 0 0-3-3.87"/><path d="M16 3.13a4 4 0 0 1 0 7.75"/></svg>
              Customers
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
