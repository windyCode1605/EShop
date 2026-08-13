import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet, RouterModule } from '@angular/router';
import { ThemeService } from '../../../core/services/theme.service';
import { AppAuthService } from '../../../core/auth/app-auth.service';
import { HasPermissionDirective } from '../../../shared/directives/has-permission.directive';
import { PERMISSIONS } from '../../../core/constants/permissions.const';

@Component({
  selector: 'app-admin-layout',
  standalone: true,
  imports: [CommonModule, RouterOutlet, RouterModule, HasPermissionDirective],
  styles: [`
    .nav-item { color: #64748B; font-weight: 500; transition: all 0.2s cubic-bezier(0.4, 0, 0.2, 1); }
    .nav-item:hover { background-color: rgba(100, 116, 139, 0.1); color: #0F172A; }
    .nav-item:active { transform: scale(0.96); background-color: rgba(100, 116, 139, 0.15); }
    .dark .nav-item { color: #94A3B8; }
    .dark .nav-item:hover { background-color: rgba(148, 163, 184, 0.1); color: #F8FAFC; }
    .dark .nav-item:active { transform: scale(0.96); background-color: rgba(148, 163, 184, 0.15); }
    
    .nav-item.active {
      background-color: #EEF2FF !important;
      color: #4F46E5 !important;
      box-shadow: inset 3px 0 0 0 #4F46E5, 0 0 0 1px rgba(199, 210, 254, 0.6);
      font-weight: 600;
    }
    .dark .nav-item.active {
      background-color: rgba(79, 70, 229, 0.15) !important;
      color: #818CF8 !important;
      box-shadow: inset 3px 0 0 0 #818CF8, 0 0 0 1px rgba(99, 102, 241, 0.2);
    }
    
    .nav-item-mobile { color: #94A3B8; transition: all 0.2s cubic-bezier(0.4, 0, 0.2, 1); }
    .nav-item-mobile:active { transform: scale(0.92); color: #6366F1; }
    .nav-item-mobile.active { color: #6366F1 !important; font-weight: 600; }
  `],
  template: `
    <div [class]="isDark() ? 'dark' : 'light'"
         class="min-h-[100dvh] flex flex-col font-sans selection:bg-indigo-500/30 transition-colors duration-300 antialiased"
         [style.background]="isDark() ? '#0B0F19' : '#F8FAFC'"
         [style.color]="isDark() ? '#E2E8F0' : '#0F172A'">

      <div class="flex flex-1 min-h-[100dvh] relative overflow-x-hidden">
        
        <!-- Backdrop Overlay for Mobile Drawer -->
        <div *ngIf="isMobileDrawerOpen()"
             (click)="closeMobileDrawer()"
             class="fixed inset-0 bg-black/60 backdrop-blur-xs z-40 md:hidden transition-opacity duration-300">
        </div>

        <!-- Sidebar (Desktop: Sticky Sidebar | Mobile: Slide-over Drawer) -->
        <aside [class]="'flex-shrink-0 flex flex-col fixed md:sticky top-0 h-[100dvh] overflow-y-auto px-4 py-6 transition-all duration-300 ease-in-out z-50 md:z-20 ' +
                        (isMobileDrawerOpen() ? 'translate-x-0 w-64 shadow-2xl' : '-translate-x-full md:translate-x-0 ') +
                        (isCollapsed() ? 'md:w-20' : 'md:w-64')"
               [style.background]="isDark() ? '#111827' : '#FFFFFF'"
               [style.borderRight]="isDark() ? '1px solid rgba(255,255,255,0.07)' : '1px solid #E2E8F0'">

          <!-- Header / Brand Logo & Close/Toggle -->
          <div class="flex items-center justify-between mb-8 px-1">
            <div class="flex items-center gap-3 overflow-hidden">
              <svg width="34" height="34" viewBox="0 0 44 44" fill="none" xmlns="http://www.w3.org/2000/svg" class="flex-shrink-0 drop-shadow-md">
                <circle cx="22" cy="22" r="20" stroke="#C9A961" stroke-width="1.5" fill="none" opacity="0.7"/>
                <path d="M7 31 L7 13 L16 23 L25 13 L25 31" [attr.stroke]="isDark() ? '#FFFFFF' : '#0F3460'" stroke-width="2.2" stroke-linecap="round" stroke-linejoin="round" fill="none"/>
                <path d="M29 17 Q38 17 38 24 Q38 31 29 31 L29 17" stroke="#C9A961" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" fill="none"/>
                <path d="M34 29 L40 36" stroke="#C9A961" stroke-width="2" stroke-linecap="round"/>
              </svg>
              <div *ngIf="!isCollapsed() || isMobileDrawerOpen()" class="flex flex-col leading-none whitespace-nowrap transition-opacity duration-200">
                <span class="text-[13px] font-extrabold tracking-[0.15em] uppercase"
                      [style.color]="isDark() ? '#FFFFFF' : '#0F3460'">QUANG NGUYÊN</span>
                <span class="text-[10px] tracking-[0.12em] font-semibold mt-1"
                      [style.color]="isDark() ? '#C9A961' : '#A88A3E'">ADMIN PANEL</span>
              </div>
            </div>

            <!-- Mobile Close Button -->
            <button (click)="closeMobileDrawer()"
                    class="md:hidden p-1.5 rounded-lg transition-colors outline-none cursor-pointer hover:bg-slate-500/10 text-slate-400">
              <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                <path d="M18 6 6 18"/><path d="m6 6 12 12"/>
              </svg>
            </button>

          </div>

          <!-- Navigation Links -->
          <nav class="flex-1 flex flex-col gap-6">

            <!-- Group: Overview -->
            <div>
              <div *ngIf="!isCollapsed() || isMobileDrawerOpen()" class="text-[10px] font-bold uppercase tracking-widest mb-2.5 px-2 text-slate-400">
                Tổng quan
              </div>
              <div *ngIf="isCollapsed() && !isMobileDrawerOpen()" class="w-full h-px my-2" [style.background]="isDark() ? 'rgba(255,255,255,0.06)' : '#F1F5F9'"></div>
              <ul class="flex flex-col gap-1">
                <li *hasPermission="PERMISSIONS.DASHBOARD.VIEW">
                  <a routerLink="/admin/dashboard"
                     (click)="closeMobileDrawer()"
                     [routerLinkActiveOptions]="{exact: true}"
                     routerLinkActive="active"
                     [class]="'flex items-center gap-3 py-2.5 rounded-xl text-[13px] font-medium transition-all duration-200 outline-none ' + ((isCollapsed() && !isMobileDrawerOpen()) ? 'justify-center px-0' : 'px-3')"
                     >
                    <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="flex-shrink-0"><rect width="7" height="9" x="3" y="3" rx="1"/><rect width="7" height="5" x="14" y="3" rx="1"/><rect width="7" height="9" x="14" y="12" rx="1"/><rect width="7" height="5" x="3" y="16" rx="1"/></svg>
                    <span *ngIf="!isCollapsed() || isMobileDrawerOpen()" class="truncate">Overview</span>
                  </a>
                </li>
              </ul>
            </div>

            <!-- Group: Catalog -->
            <div>
              <div *ngIf="!isCollapsed() || isMobileDrawerOpen()" class="text-[10px] font-bold uppercase tracking-widest mb-2.5 px-2 text-slate-400">
                Danh mục
              </div>
              <div *ngIf="isCollapsed() && !isMobileDrawerOpen()" class="w-full h-px my-2" [style.background]="isDark() ? 'rgba(255,255,255,0.06)' : '#F1F5F9'"></div>
              <ul class="flex flex-col gap-1">
                <li *hasPermission="PERMISSIONS.CATALOG.CATEGORIES_VIEW">
                  <a routerLink="/admin/categories"
                     (click)="closeMobileDrawer()"
                     [routerLinkActiveOptions]="{exact: true}"
                     routerLinkActive="active"
                     [class]="'flex items-center gap-3 py-2.5 rounded-xl text-[13px] font-medium transition-all duration-200 outline-none ' + ((isCollapsed() && !isMobileDrawerOpen()) ? 'justify-center px-0' : 'px-3')"
                     >
                    <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="flex-shrink-0"><path d="M2 3h6a4 4 0 0 1 4 4v14a3 3 0 0 0-3-3H2z"/><path d="M22 3h-6a4 4 0 0 0-4 4v14a3 3 0 0 1 3-3h7z"/></svg>
                    <span *ngIf="!isCollapsed() || isMobileDrawerOpen()" class="truncate">Danh mục</span>
                  </a>
                </li>
                <li *hasPermission="PERMISSIONS.CATALOG.PRODUCTS_VIEW">
                  <a routerLink="/admin/products"
                     (click)="closeMobileDrawer()"
                     [routerLinkActiveOptions]="{exact: true}"
                     routerLinkActive="active"
                     [class]="'flex items-center gap-3 py-2.5 rounded-xl text-[13px] font-medium transition-all duration-200 outline-none ' + ((isCollapsed() && !isMobileDrawerOpen()) ? 'justify-center px-0' : 'px-3')"
                     >
                    <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="flex-shrink-0"><path d="m7.5 4.27 9 5.15"/><path d="M21 8a2 2 0 0 0-1-1.73l-7-4a2 2 0 0 0-2 0l-7 4A2 2 0 0 0 3 8v8a2 2 0 0 0 1 1.73l7 4a2 2 0 0 0 2 0l7-4A2 2 0 0 0 21 16Z"/><path d="m3.3 7 8.7 5 8.7-5"/><path d="M12 22V12"/></svg>
                    <span *ngIf="!isCollapsed() || isMobileDrawerOpen()" class="truncate">Sản phẩm</span>
                  </a>
                </li>
              </ul>
            </div>

            <!-- Group: Operation -->
            <div>
              <div *ngIf="!isCollapsed() || isMobileDrawerOpen()" class="text-[10px] font-bold uppercase tracking-widest mb-2.5 px-2 text-slate-400">
                Vận hành
              </div>
              <div *ngIf="isCollapsed() && !isMobileDrawerOpen()" class="w-full h-px my-2" [style.background]="isDark() ? 'rgba(255,255,255,0.06)' : '#F1F5F9'"></div>
              <ul class="flex flex-col gap-1">
                <li *hasPermission="PERMISSIONS.SALES.ORDERS_VIEW">
                  <a routerLink="/admin/orders"
                     (click)="closeMobileDrawer()"
                     [routerLinkActiveOptions]="{exact: true}"
                     routerLinkActive="active"
                     [class]="'flex items-center gap-3 py-2.5 rounded-xl text-[13px] font-medium transition-all duration-200 outline-none ' + ((isCollapsed() && !isMobileDrawerOpen()) ? 'justify-center px-0' : 'px-3')"
                     >
                    <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="flex-shrink-0"><path d="M6 2 3 6v14a2 2 0 0 0 2 2h14a2 2 0 0 0 2-2V6l-3-4Z"/><path d="M3 6h18"/><path d="M16 10a4 4 0 0 1-8 0"/></svg>
                    <span *ngIf="!isCollapsed() || isMobileDrawerOpen()" class="truncate">Đơn hàng</span>
                  </a>
                </li>
                <li *hasPermission="PERMISSIONS.CUSTOMERS.VIEW">
                  <a routerLink="/admin/customers"
                     (click)="closeMobileDrawer()"
                     [routerLinkActiveOptions]="{exact: true}"
                     routerLinkActive="active"
                     [class]="'flex items-center gap-3 py-2.5 rounded-xl text-[13px] font-medium transition-all duration-200 outline-none ' + ((isCollapsed() && !isMobileDrawerOpen()) ? 'justify-center px-0' : 'px-3')"
                     >
                    <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="flex-shrink-0"><path d="M16 21v-2a4 4 0 0 0-4-4H6a4 4 0 0 0-4 4v2"/><circle cx="9" cy="7" r="4"/><path d="M22 21v-2a4 4 0 0 0-3-3.87"/><path d="M16 3.13a4 4 0 0 1 0 7.75"/></svg>
                    <span *ngIf="!isCollapsed() || isMobileDrawerOpen()" class="truncate">Khách hàng</span>
                  </a>
                </li>
              </ul>
            </div>

            <!-- Group: System -->
            <div>
              <div *ngIf="!isCollapsed() || isMobileDrawerOpen()" class="text-[10px] font-bold uppercase tracking-widest mb-2.5 px-2 text-slate-400">
                Hệ thống
              </div>
              <div *ngIf="isCollapsed() && !isMobileDrawerOpen()" class="w-full h-px my-2" [style.background]="isDark() ? 'rgba(255,255,255,0.06)' : '#F1F5F9'"></div>
              <ul class="flex flex-col gap-1">
                <li *hasPermission="PERMISSIONS.SYSTEM.SETTINGS_VIEW">
                  <a routerLink="/admin/settings"
                     (click)="closeMobileDrawer()"
                     [routerLinkActiveOptions]="{exact: true}"
                     routerLinkActive="active"
                     [class]="'flex items-center gap-3 py-2.5 rounded-xl text-[13px] font-medium transition-all duration-200 outline-none ' + ((isCollapsed() && !isMobileDrawerOpen()) ? 'justify-center px-0' : 'px-3')"
                     >
                    <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="flex-shrink-0"><path d="M12.22 2h-.44a2 2 0 0 0-2 2v.18a2 2 0 0 1-1 1.73l-.43.25a2 2 0 0 1-2 0l-.15-.08a2 2 0 0 0-2.73.73l-.22.38a2 2 0 0 0 .73 2.73l.15.1a2 2 0 0 1 1 1.72v.51a2 2 0 0 1-1 1.74l-.15.09a2 2 0 0 0-.73 2.73l.22.38a2 2 0 0 0 2.73.73l.15-.08a2 2 0 0 1 2 0l.43.25a2 2 0 0 1 1 1.73V20a2 2 0 0 0 2 2h.44a2 2 0 0 0 2-2v-.18a2 2 0 0 1 1-1.73l.43-.25a2 2 0 0 1 2 0l.15.08a2 2 0 0 0 2.73-.73l.22-.39a2 2 0 0 0-.73-2.73l-.15-.08a2 2 0 0 1-1-1.74v-.5a2 2 0 0 1 1-1.74l.15-.09a2 2 0 0 0 .73-2.73l-.22-.38a2 2 0 0 0-2.73-.73l-.15.08a2 2 0 0 1-2 0l-.43-.25a2 2 0 0 1-1-1.73V4a2 2 0 0 0-2-2z"/><circle cx="12" cy="12" r="3"/></svg>
                    <span *ngIf="!isCollapsed() || isMobileDrawerOpen()" class="truncate">Cài đặt</span>
                  </a>
                </li>
                <li *hasPermission="[PERMISSIONS.IDENTITY.ROLES_VIEW, PERMISSIONS.IDENTITY.ROLES_MANAGE]">
                  <a routerLink="/admin/roles"
                     (click)="closeMobileDrawer()"
                     [routerLinkActiveOptions]="{exact: true}"
                     routerLinkActive="active"
                     [class]="'flex items-center gap-3 py-2.5 rounded-xl text-[13px] font-medium transition-all duration-200 outline-none ' + ((isCollapsed() && !isMobileDrawerOpen()) ? 'justify-center px-0' : 'px-3')"
                     >
                    <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="flex-shrink-0"><path d="M12 22s8-4 8-10V5l-8-3-8 3v7c0 6 8 10 8 10z"/></svg>
                    <span *ngIf="!isCollapsed() || isMobileDrawerOpen()" class="truncate">Phân quyền</span>
                  </a>
                </li>
                <li *hasPermission="[PERMISSIONS.IDENTITY.USERS_VIEW, PERMISSIONS.IDENTITY.USERS_MANAGE]">
                  <a routerLink="/admin/employees"
                     (click)="closeMobileDrawer()"
                     [routerLinkActiveOptions]="{exact: true}"
                     routerLinkActive="active"
                     [class]="'flex items-center gap-3 py-2.5 rounded-xl text-[13px] font-medium transition-all duration-200 outline-none ' + ((isCollapsed() && !isMobileDrawerOpen()) ? 'justify-center px-0' : 'px-3')"
                     >
                    <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="flex-shrink-0"><path d="M16 21v-2a4 4 0 0 0-4-4H6a4 4 0 0 0-4 4v2"/><circle cx="9" cy="7" r="4"/><line x1="19" y1="8" x2="19" y2="14"/><line x1="22" y1="11" x2="16" y2="11"/></svg>
                    <span *ngIf="!isCollapsed() || isMobileDrawerOpen()" class="truncate">Nhân viên</span>
                  </a>
                </li>
              </ul>
            </div>

          </nav>

          <!-- Sidebar Footer: Theme Toggle & User Info -->
          <div class="mt-auto pt-4 flex flex-col gap-3"
               [style.borderTop]="isDark() ? '1px solid rgba(255,255,255,0.07)' : '1px solid #E2E8F0'">

            <!-- Theme Toggle Button -->
            <button (click)="toggleTheme()"
                    [class]="'flex items-center gap-3 py-2.5 rounded-xl text-[13px] font-medium transition-all duration-200 outline-none cursor-pointer hover:bg-slate-500/10 text-left ' + ((isCollapsed() && !isMobileDrawerOpen()) ? 'justify-center px-0' : 'px-3')"
                    >
              <svg *ngIf="!isDark()" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="flex-shrink-0">
                <circle cx="12" cy="12" r="4"/><path d="M12 2v2M12 20v2M4.93 4.93l1.41 1.41M17.66 17.66l1.41 1.41M2 12h2M20 12h2M6.34 17.66l-1.41 1.41M19.07 4.93l-1.41 1.41"/>
              </svg>
              <svg *ngIf="isDark()" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="flex-shrink-0">
                <path d="M12 3a6 6 0 0 0 9 9 9 9 0 1 1-9-9Z"/>
              </svg>
              <span *ngIf="!isCollapsed() || isMobileDrawerOpen()" class="truncate">{{ isDark() ? 'Chế độ tối' : 'Chế độ sáng' }}</span>
            </button>

            <!-- User Info & Logout -->
            <div [class]="'flex items-center gap-3 ' + ((isCollapsed() && !isMobileDrawerOpen()) ? 'flex-col justify-center' : '')">
              <div class="w-8 h-8 rounded-full flex-shrink-0 flex items-center justify-center text-[11px] font-bold select-none shadow-sm"
                   [style.background]="isDark() ? '#1E293B' : '#E2E8F0'"
                   [style.color]="isDark() ? '#38BDF8' : '#0284C7'">MQ</div>
              
              <div *ngIf="!isCollapsed() || isMobileDrawerOpen()" class="flex-1 flex flex-col leading-tight min-w-0">
                <span class="text-[13px] font-semibold truncate"
                      [style.color]="isDark() ? '#F1F5F9' : '#0F172A'">Mai Quang Nguyên</span>
                <span class="text-[11px] font-medium" [style.color]="isDark() ? '#64748B' : '#94A3B8'">Administrator</span>
              </div>

              <button (click)="onLogout()"
                      class="w-8 h-8 rounded-lg flex items-center justify-center transition-all duration-200 flex-shrink-0 outline-none cursor-pointer hover:bg-rose-500/10 text-slate-400 hover:text-rose-500"
                      title="Đăng xuất">
                <svg width="17" height="17" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                  <path d="M9 21H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h4"/>
                  <polyline points="16 17 21 12 16 7"/>
                  <line x1="21" y1="12" x2="9" y2="12"/>
                </svg>
              </button>
            </div>

          </div>

        </aside>

        <!-- Main Workspace Section -->
        <div class="flex-1 flex flex-col min-w-0 h-[100dvh] overflow-y-auto">
          
          <!-- Top Header Navigation Bar -->
          <header class="h-16 px-4 md:px-6 flex items-center justify-between sticky top-0 z-10 backdrop-blur-md transition-colors duration-300"
                  [style.background]="isDark() ? 'rgba(17, 24, 39, 0.85)' : 'rgba(255, 255, 255, 0.85)'"
                  [style.borderBottom]="isDark() ? '1px solid rgba(255,255,255,0.07)' : '1px solid #E2E8F0'">
            
            <div class="flex items-center gap-3">
              <!-- Mobile Drawer Toggle Button (Hamburger) -->
              <button (click)="toggleMobileDrawer()"
                      class="md:hidden p-2 rounded-xl transition-all duration-200 outline-none cursor-pointer border hover:bg-slate-500/10"
                      [style.borderColor]="isDark() ? 'rgba(255,255,255,0.1)' : '#E2E8F0'"
                      >
                <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                  <line x1="4" x2="20" y1="12" y2="12"/>
                  <line x1="4" x2="20" y1="6" y2="6"/>
                  <line x1="4" x2="20" y1="18" y2="18"/>
                </svg>
              </button>

              <!-- Desktop Sidebar Collapse Toggle Button -->
              <button (click)="toggleSidebar()"
                      class="hidden md:flex p-2 rounded-xl transition-all duration-200 outline-none cursor-pointer border hover:bg-slate-500/10"
                      [style.borderColor]="isDark() ? 'rgba(255,255,255,0.1)' : '#E2E8F0'"
                      
                      [attr.title]="isCollapsed() ? 'Mở rộng menu' : 'Thu gọn menu'">
                <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                  <line x1="4" x2="20" y1="12" y2="12"/>
                  <line x1="4" x2="20" y1="6" y2="6"/>
                  <line x1="4" x2="20" y1="18" y2="18"/>
                </svg>
              </button>

            </div>

            <!-- Top Header Actions -->
            <div class="flex items-center gap-2 sm:gap-3">
              <!-- Quick Stats / System Badge -->
              <span class="hidden md:inline-flex items-center gap-1.5 px-3 py-1 rounded-full text-[11px] font-semibold tracking-wide border"
                    [style.background]="isDark() ? 'rgba(16, 185, 129, 0.1)' : '#ECFDF5'"
                    [style.borderColor]="isDark() ? 'rgba(16, 185, 129, 0.2)' : '#A7F3D0'"
                    [style.color]="isDark() ? '#34D399' : '#059669'">
                <span class="w-1.5 h-1.5 rounded-full bg-emerald-500 animate-pulse"></span>
                Hệ thống hoạt động
              </span>

              <!-- Theme Toggle Button (Mobile Top Bar) -->
              <button (click)="toggleTheme()"
                      class="md:hidden p-2 rounded-xl transition-all duration-200 outline-none cursor-pointer border hover:bg-slate-500/10"
                      [style.borderColor]="isDark() ? 'rgba(255,255,255,0.1)' : '#E2E8F0'"
                      >
                <svg *ngIf="!isDark()" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><circle cx="12" cy="12" r="4"/><path d="M12 2v2M12 20v2M4.93 4.93l1.41 1.41M17.66 17.66l1.41 1.41M2 12h2M20 12h2M6.34 17.66l-1.41 1.41M19.07 4.93l-1.41 1.41"/></svg>
                <svg *ngIf="isDark()" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M12 3a6 6 0 0 0 9 9 9 9 0 1 1-9-9Z"/></svg>
              </button>

              <!-- Notification Bell -->
              <button class="relative p-2 rounded-xl transition-all duration-200 outline-none cursor-pointer border hover:bg-slate-500/10 text-slate-400"
                      [style.borderColor]="isDark() ? 'rgba(255,255,255,0.1)' : '#E2E8F0'"
                      title="Thông báo">
                <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                  <path d="M6 8a6 6 0 0 1 12 0c0 7 3 9 3 9H3s3-2 3-9"/><path d="M10.3 21a1.94 1.94 0 0 0 3.4 0"/>
                </svg>
                <span class="absolute top-1.5 right-1.5 w-2 h-2 rounded-full bg-indigo-500"></span>
              </button>
            </div>
          </header>

          <!-- Router Outlet for Child Component Views -->
          <main class="flex-1 p-4 sm:p-6 lg:p-8 pb-20 md:pb-8">
            <router-outlet></router-outlet>
          </main>

          <!-- Mobile Touch Bottom Navigation Bar (App-Native Feel) -->
          <nav class="md:hidden fixed bottom-0 left-0 right-0 h-16 z-30 flex items-center justify-around px-2 backdrop-blur-lg border-t transition-colors duration-300"
               [style.background]="isDark() ? 'rgba(17, 24, 39, 0.92)' : 'rgba(255, 255, 255, 0.92)'"
               [style.borderColor]="isDark() ? 'rgba(255,255,255,0.08)' : '#E2E8F0'">
            
            <a routerLink="/admin/dashboard"
               [routerLinkActiveOptions]="{exact: true}"
               routerLinkActive="active"
               class="nav-item-mobile flex flex-col items-center justify-center w-14 h-full gap-0.5 outline-none transition-colors duration-200">
              <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><rect width="7" height="9" x="3" y="3" rx="1"/><rect width="7" height="5" x="14" y="3" rx="1"/><rect width="7" height="9" x="14" y="12" rx="1"/><rect width="7" height="5" x="3" y="16" rx="1"/></svg>
              <span class="text-[10px] tracking-tight">Overview</span>
            </a>

            <a routerLink="/admin/products"
               [routerLinkActiveOptions]="{exact: true}"
               routerLinkActive="active"
               class="nav-item-mobile flex flex-col items-center justify-center w-14 h-full gap-0.5 outline-none transition-colors duration-200">
              <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="m7.5 4.27 9 5.15"/><path d="M21 8a2 2 0 0 0-1-1.73l-7-4a2 2 0 0 0-2 0l-7 4A2 2 0 0 0 3 8v8a2 2 0 0 0 1 1.73l7 4a2 2 0 0 0 2 0l7-4A2 2 0 0 0 21 16Z"/><path d="m3.3 7 8.7 5 8.7-5"/><path d="M12 22V12"/></svg>
              <span class="text-[10px] tracking-tight">Sản phẩm</span>
            </a>

            <a routerLink="/admin/orders"
               [routerLinkActiveOptions]="{exact: true}"
               routerLinkActive="active"
               class="nav-item-mobile flex flex-col items-center justify-center w-14 h-full gap-0.5 outline-none transition-colors duration-200">
              <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M6 2 3 6v14a2 2 0 0 0 2 2h14a2 2 0 0 0 2-2V6l-3-4Z"/><path d="M3 6h18"/><path d="M16 10a4 4 0 0 1-8 0"/></svg>
              <span class="text-[10px] tracking-tight">Đơn hàng</span>
            </a>

            <a routerLink="/admin/settings"
               routerLinkActive="active"
               class="nav-item-mobile flex flex-col items-center justify-center w-14 h-full gap-0.5 outline-none transition-colors duration-200">
              <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M12.22 2h-.44a2 2 0 0 0-2 2v.18a2 2 0 0 1-1 1.73l-.43.25a2 2 0 0 1-2 0l-.15-.08a2 2 0 0 0-2.73.73l-.22.38a2 2 0 0 0 .73 2.73l.15.1a2 2 0 0 1 1 1.72v.51a2 2 0 0 1-1 1.74l-.15.09a2 2 0 0 0-.73 2.73l.22.38a2 2 0 0 0 2.73.73l.15-.08a2 2 0 0 1 2 0l.43.25a2 2 0 0 1 1 1.73V20a2 2 0 0 0 2 2h.44a2 2 0 0 0 2-2v-.18a2 2 0 0 1 1-1.73l.43-.25a2 2 0 0 1 2 0l.15.08a2 2 0 0 0 2.73-.73l.22-.39a2 2 0 0 0-.73-2.73l-.15-.08a2 2 0 0 1-1-1.74v-.5a2 2 0 0 1 1-1.74l.15-.09a2 2 0 0 0 .73-2.73l-.22-.38a2 2 0 0 0-2.73-.73l-.15.08a2 2 0 0 1-2 0l-.43-.25a2 2 0 0 1-1-1.73V4a2 2 0 0 0-2-2z"/><circle cx="12" cy="12" r="3"/></svg>
              <span class="text-[10px] tracking-tight">Cài đặt</span>
            </a>

            <button (click)="toggleMobileDrawer()"
                    class="flex flex-col items-center justify-center w-14 h-full gap-0.5 text-slate-400 outline-none cursor-pointer">
              <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><line x1="4" x2="20" y1="12" y2="12"/><line x1="4" x2="20" y1="6" y2="6"/><line x1="4" x2="20" y1="18" y2="18"/></svg>
              <span class="text-[10px] tracking-tight">Menu</span>
            </button>
          </nav>

        </div>
      </div>

    </div>
  `
})
export class AdminLayoutComponent {
  private themeService = inject(ThemeService);
  private authService = inject(AppAuthService);

  readonly PERMISSIONS = PERMISSIONS;

  isDark = this.themeService.isDark;
  isCollapsed = signal<boolean>(false);
  isMobileDrawerOpen = signal<boolean>(false);

  toggleSidebar(): void {
    this.isCollapsed.update(state => !state);
  }

  toggleMobileDrawer(): void {
    this.isMobileDrawerOpen.update(state => !state);
  }

  closeMobileDrawer(): void {
    this.isMobileDrawerOpen.set(false);
  }

  toggleTheme(): void {
    this.themeService.toggle();
  }

  onLogout(): void {
    this.authService.logout();
  }
}



