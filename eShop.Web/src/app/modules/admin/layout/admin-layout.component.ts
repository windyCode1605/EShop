import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet, RouterModule } from '@angular/router';
import { ThemeService } from '../../../core/services/theme.service';
import { AppAuthService } from '../../../core/auth/app-auth.service';

@Component({
  selector: 'app-admin-layout',
  standalone: true,
  imports: [CommonModule, RouterOutlet, RouterModule],
  template: `
    <div [class]="isDark() ? 'dark' : 'light'"
         class="min-h-[100dvh] flex font-sans selection:bg-indigo-500/30 transition-colors duration-300"
         [style.background]="isDark() ? '#0F172A' : '#F4F4F5'"
         [style.color]="isDark() ? '#d4d4d8' : '#18181B'">

      <!-- Sidebar -->
      <aside class="w-60 flex-shrink-0 flex flex-col sticky top-0 h-screen overflow-y-auto px-5 py-7 transition-colors duration-300"
             [style.background]="isDark() ? '#0B1121' : '#FFFFFF'"
             [style.borderRight]="isDark() ? '1px solid rgba(255,255,255,0.06)' : '1px solid #E4E4E7'">

        <!-- MQN Logo Mark: Navy + Gold brand colors -->
        <div class="flex items-center gap-3 mb-9 px-1">
          <svg width="36" height="36" viewBox="0 0 44 44" fill="none" xmlns="http://www.w3.org/2000/svg" style="filter: drop-shadow(0 2px 6px rgba(201,169,97,0.2))">
            <circle cx="22" cy="22" r="20" stroke="#C9A961" stroke-width="1.5" fill="none" opacity="0.6"/>
            <path d="M7 31 L7 13 L16 23 L25 13 L25 31" [attr.stroke]="isDark() ? '#FFFFFF' : '#0F3460'" stroke-width="2.2" stroke-linecap="round" stroke-linejoin="round" fill="none"/>
            <path d="M29 17 Q38 17 38 24 Q38 31 29 31 L29 17" stroke="#C9A961" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" fill="none"/>
            <path d="M34 29 L40 36" stroke="#C9A961" stroke-width="2" stroke-linecap="round"/>
          </svg>
          <div class="flex flex-col leading-none">
            <span class="text-[14px] font-bold tracking-[0.15em] uppercase"
                  [style.color]="isDark() ? '#FFFFFF' : '#0F3460'">QUANG NGUYÊN</span>
            <span class="text-[10px] tracking-[0.12em] font-medium mt-0.5"
                  [style.color]="isDark() ? '#C9A961' : '#A88A3E'">ADMIN</span>
          </div>
        </div>

        <!-- Navigation -->
        <nav class="flex-1 flex flex-col gap-5">

          <!-- Overview -->
          <div>
            <div class="text-[10px] font-semibold uppercase tracking-widest mb-2 px-1"
                 [style.color]="isDark() ? '#3f3f46' : '#A1A1AA'">Tổng quan</div>
            <ul class="flex flex-col gap-0.5">
              <li>
                <a routerLink="/admin/dashboard"
                   [routerLinkActive]="isDark() ? 'bg-white/10 text-white ring-1 ring-white/10' : 'bg-zinc-100 text-zinc-900 ring-1 ring-zinc-200'"
                   class="flex items-center gap-2.5 px-3 py-2.5 rounded-xl text-[13px] font-medium transition-all duration-200 outline-none"
                   [style.color]="isDark() ? '#A1A1AA' : '#52525B'">
                  <svg width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><rect width="7" height="9" x="3" y="3" rx="1"/><rect width="7" height="5" x="14" y="3" rx="1"/><rect width="7" height="9" x="14" y="12" rx="1"/><rect width="7" height="5" x="3" y="16" rx="1"/></svg>
                  Overview
                </a>
              </li>
            </ul>
          </div>

          <!-- Catalog -->
          <div>
            <div class="text-[10px] font-semibold uppercase tracking-widest mb-2 px-1"
                 [style.color]="isDark() ? '#3f3f46' : '#A1A1AA'">Danh mục</div>
            <ul class="flex flex-col gap-0.5">
              <li>
                <a routerLink="/admin/categories"
                   [routerLinkActive]="isDark() ? 'bg-white/10 text-white ring-1 ring-white/10' : 'bg-zinc-100 text-zinc-900 ring-1 ring-zinc-200'"
                   class="flex items-center gap-2.5 px-3 py-2.5 rounded-xl text-[13px] font-medium transition-all duration-200 outline-none"
                   [style.color]="isDark() ? '#A1A1AA' : '#52525B'">
                  <svg width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M2 3h6a4 4 0 0 1 4 4v14a3 3 0 0 0-3-3H2z"/><path d="M22 3h-6a4 4 0 0 0-4 4v14a3 3 0 0 1 3-3h7z"/></svg>
                  Danh mục
                </a>
              </li>
              <li>
                <a routerLink="/admin/products"
                   [routerLinkActive]="isDark() ? 'bg-white/10 text-white ring-1 ring-white/10' : 'bg-zinc-100 text-zinc-900 ring-1 ring-zinc-200'"
                   class="flex items-center gap-2.5 px-3 py-2.5 rounded-xl text-[13px] font-medium transition-all duration-200 outline-none"
                   [style.color]="isDark() ? '#A1A1AA' : '#52525B'">
                  <svg width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="m7.5 4.27 9 5.15"/><path d="M21 8a2 2 0 0 0-1-1.73l-7-4a2 2 0 0 0-2 0l-7 4A2 2 0 0 0 3 8v8a2 2 0 0 0 1 1.73l7 4a2 2 0 0 0 2 0l7-4A2 2 0 0 0 21 16Z"/><path d="m3.3 7 8.7 5 8.7-5"/><path d="M12 22V12"/></svg>
                  Sản phẩm
                </a>
              </li>
            </ul>
          </div>

          <!-- Operation -->
          <div>
            <div class="text-[10px] font-semibold uppercase tracking-widest mb-2 px-1"
                 [style.color]="isDark() ? '#3f3f46' : '#A1A1AA'">Vận hành</div>
            <ul class="flex flex-col gap-0.5">
              <li>
                <a routerLink="/admin/orders"
                   [routerLinkActive]="isDark() ? 'bg-white/10 text-white ring-1 ring-white/10' : 'bg-zinc-100 text-zinc-900 ring-1 ring-zinc-200'"
                   class="flex items-center gap-2.5 px-3 py-2.5 rounded-xl text-[13px] font-medium transition-all duration-200 outline-none"
                   [style.color]="isDark() ? '#A1A1AA' : '#52525B'">
                  <svg width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M6 2 3 6v14a2 2 0 0 0 2 2h14a2 2 0 0 0 2-2V6l-3-4Z"/><path d="M3 6h18"/><path d="M16 10a4 4 0 0 1-8 0"/></svg>
                  Đơn hàng
                </a>
              </li>
              <li>
                <a routerLink="/admin/customers"
                   [routerLinkActive]="isDark() ? 'bg-white/10 text-white ring-1 ring-white/10' : 'bg-zinc-100 text-zinc-900 ring-1 ring-zinc-200'"
                   class="flex items-center gap-2.5 px-3 py-2.5 rounded-xl text-[13px] font-medium transition-all duration-200 outline-none"
                   [style.color]="isDark() ? '#A1A1AA' : '#52525B'">
                  <svg width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M16 21v-2a4 4 0 0 0-4-4H6a4 4 0 0 0-4 4v2"/><circle cx="9" cy="7" r="4"/><path d="M22 21v-2a4 4 0 0 0-3-3.87"/><path d="M16 3.13a4 4 0 0 1 0 7.75"/></svg>
                  Khách hàng
                </a>
              </li>
            </ul>
          </div>

          <!-- System -->
          <div>
            <div class="text-[10px] font-semibold uppercase tracking-widest mb-2 px-1"
                 [style.color]="isDark() ? '#3f3f46' : '#A1A1AA'">Hệ thống</div>
            <ul class="flex flex-col gap-0.5">
              <li>
                <a routerLink="/admin/settings"
                   [routerLinkActive]="isDark() ? 'bg-white/10 text-white ring-1 ring-white/10' : 'bg-zinc-100 text-zinc-900 ring-1 ring-zinc-200'"
                   class="flex items-center gap-2.5 px-3 py-2.5 rounded-xl text-[13px] font-medium transition-all duration-200 outline-none"
                   [style.color]="isDark() ? '#A1A1AA' : '#52525B'">
                  <svg width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M12.22 2h-.44a2 2 0 0 0-2 2v.18a2 2 0 0 1-1 1.73l-.43.25a2 2 0 0 1-2 0l-.15-.08a2 2 0 0 0-2.73.73l-.22.38a2 2 0 0 0 .73 2.73l.15.1a2 2 0 0 1 1 1.72v.51a2 2 0 0 1-1 1.74l-.15.09a2 2 0 0 0-.73 2.73l.22.38a2 2 0 0 0 2.73.73l.15-.08a2 2 0 0 1 2 0l.43.25a2 2 0 0 1 1 1.73V20a2 2 0 0 0 2 2h.44a2 2 0 0 0 2-2v-.18a2 2 0 0 1 1-1.73l.43-.25a2 2 0 0 1 2 0l.15.08a2 2 0 0 0 2.73-.73l.22-.39a2 2 0 0 0-.73-2.73l-.15-.08a2 2 0 0 1-1-1.74v-.5a2 2 0 0 1 1-1.74l.15-.09a2 2 0 0 0 .73-2.73l-.22-.38a2 2 0 0 0-2.73-.73l-.15.08a2 2 0 0 1-2 0l-.43-.25a2 2 0 0 1-1-1.73V4a2 2 0 0 0-2-2z"/><circle cx="12" cy="12" r="3"/></svg>
                  Cài đặt
                </a>
              </li>
              <li>
                <a routerLink="/admin/roles"
                   [routerLinkActive]="isDark() ? 'bg-white/10 text-white ring-1 ring-white/10' : 'bg-zinc-100 text-zinc-900 ring-1 ring-zinc-200'"
                   class="flex items-center gap-2.5 px-3 py-2.5 rounded-xl text-[13px] font-medium transition-all duration-200 outline-none"
                   [style.color]="isDark() ? '#A1A1AA' : '#52525B'">
                  <svg width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M12 22s8-4 8-10V5l-8-3-8 3v7c0 6 8 10 8 10z"/></svg>
                  Phân quyền
                </a>
              </li>
            </ul>
          </div>

        </nav>

        <!-- Bottom: Theme Toggle + User + Logout -->
        <div class="mt-auto pt-5 flex flex-col gap-3"
             [style.borderTop]="isDark() ? '1px solid rgba(255,255,255,0.06)' : '1px solid #E4E4E7'">

          <!-- Theme Toggle Button -->
          <button (click)="toggleTheme()"
                  class="flex items-center gap-2.5 px-3 py-2.5 rounded-xl text-[13px] font-medium transition-all duration-200 w-full outline-none cursor-pointer text-left"
                  [style.color]="isDark() ? '#A1A1AA' : '#52525B'"
                  title="Chuyển chế độ sáng / tối">
            <!-- Sun (shown in light mode) -->
            <svg *ngIf="!isDark()" width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
              <circle cx="12" cy="12" r="4"/><path d="M12 2v2M12 20v2M4.93 4.93l1.41 1.41M17.66 17.66l1.41 1.41M2 12h2M20 12h2M6.34 17.66l-1.41 1.41M19.07 4.93l-1.41 1.41"/>
            </svg>
            <!-- Moon (shown in dark mode) -->
            <svg *ngIf="isDark()" width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
              <path d="M12 3a6 6 0 0 0 9 9 9 9 0 1 1-9-9Z"/>
            </svg>
            <span>{{ isDark() ? 'Chế độ tối' : 'Chế độ sáng' }}</span>
          </button>

          <!-- User + Logout -->
          <div class="flex items-center gap-2.5">
            <!-- Avatar initials -->
            <div class="w-8 h-8 rounded-full flex-shrink-0 flex items-center justify-center text-[11px] font-bold select-none"
                 [style.background]="isDark() ? '#1E293B' : '#E4E4E7'"
                 [style.color]="isDark() ? '#94A3B8' : '#52525B'">MQ</div>
            <!-- Name -->
            <div class="flex-1 flex flex-col leading-tight min-w-0">
              <span class="text-[13px] font-semibold truncate"
                    [style.color]="isDark() ? '#E2E8F0' : '#18181B'">Mai Quang Nguyên</span>
              <span class="text-[11px]" [style.color]="isDark() ? '#475569' : '#A1A1AA'">Administrator</span>
            </div>
            <!-- Logout Icon Button -->
            <button (click)="onLogout()"
                    class="w-8 h-8 rounded-lg flex items-center justify-center transition-all duration-200 flex-shrink-0 outline-none cursor-pointer hover:bg-red-500/10 group"
                    [style.color]="isDark() ? '#4B5563' : '#A1A1AA'"
                    title="Đăng xuất">
              <svg width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"
                   class="group-hover:stroke-red-400 transition-colors duration-200">
                <path d="M9 21H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h4"/>
                <polyline points="16 17 21 12 16 7"/>
                <line x1="21" y1="12" x2="9" y2="12"/>
              </svg>
            </button>
          </div>
        </div>
      </aside>

      <!-- Main Content -->
      <main class="flex-1 flex flex-col h-screen overflow-y-auto">
        <router-outlet></router-outlet>
      </main>
    </div>
  `
})
export class AdminLayoutComponent {
  private themeService = inject(ThemeService);
  private authService = inject(AppAuthService);

  isDark = this.themeService.isDark;

  toggleTheme(): void {
    this.themeService.toggle();
  }

  onLogout(): void {
    this.authService.logout();
  }
}
