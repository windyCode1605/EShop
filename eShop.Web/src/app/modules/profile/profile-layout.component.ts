import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ThemeService } from '../../core/services/theme.service';

@Component({
  selector: 'app-profile-layout',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <div class="min-h-[100dvh] pb-24 pt-12 md:pt-20 font-sans transition-colors duration-300"
         [style.background]="isDark() ? '#111111' : '#F9FAFB'"
         [style.color]="isDark() ? '#E4E4E7' : '#18181B'">
      <div class="max-w-[1100px] mx-auto px-6 md:px-8">

        <!-- Top-right: Theme Toggle -->
        <div class="flex justify-end mb-8">
          <button (click)="toggleTheme()"
                  class="flex items-center gap-2 px-4 py-2 rounded-[12px] text-[13px] font-medium transition-all duration-250 border outline-none cursor-pointer"
                  [style.background]="isDark() ? '#1C1C1E' : '#FFFFFF'"
                  [style.border]="isDark() ? '1px solid rgba(255,255,255,0.1)' : '1px solid #E4E4E7'"
                  [style.color]="isDark() ? '#A1A1AA' : '#52525B'">
            <!-- Sun -->
            <svg *ngIf="!isDark()" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
              <circle cx="12" cy="12" r="4"/><path d="M12 2v2M12 20v2M4.93 4.93l1.41 1.41M17.66 17.66l1.41 1.41M2 12h2M20 12h2M6.34 17.66l-1.41 1.41M19.07 4.93l-1.41 1.41"/>
            </svg>
            <!-- Moon -->
            <svg *ngIf="isDark()" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
              <path d="M12 3a6 6 0 0 0 9 9 9 9 0 1 1-9-9Z"/>
            </svg>
            <span>{{ isDark() ? 'Tối' : 'Sáng' }}</span>
          </button>
        </div>

        <!-- Premium Editorial Header -->
        <header class="mb-16 max-w-2xl">
          <!-- MQN Logo Mark: Navy + Gold -->
          <div class="flex items-center gap-2.5 mb-6">
            <svg width="30" height="30" viewBox="0 0 44 44" fill="none" xmlns="http://www.w3.org/2000/svg" style="filter: drop-shadow(0 1px 5px rgba(201,169,97,0.2))">
              <circle cx="22" cy="22" r="20" stroke="#C9A961" stroke-width="1.5" fill="none" opacity="0.6"/>
              <path d="M7 31 L7 13 L16 23 L25 13 L25 31" [attr.stroke]="isDark() ? '#FFFFFF' : '#0F3460'" stroke-width="2.2" stroke-linecap="round" stroke-linejoin="round" fill="none"/>
              <path d="M29 17 Q38 17 38 24 Q38 31 29 31 L29 17" stroke="#C9A961" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" fill="none"/>
              <path d="M34 29 L40 36" stroke="#C9A961" stroke-width="2" stroke-linecap="round"/>
            </svg>
            <div class="flex flex-col leading-none">
              <span class="text-[13px] font-bold tracking-[0.18em] uppercase"
                    [style.color]="isDark() ? '#FFFFFF' : '#0F3460'">QUANG NGUYỆN</span>
              <span class="text-[10px] tracking-[0.1em] font-medium mt-0.5"
                    [style.color]="isDark() ? '#C9A961' : '#A88A3E'">MAI QUANG NGUYỆN</span>
            </div>
          </div>
          <h1 class="text-4xl md:text-[44px] font-medium tracking-tight mb-4">Hồ sơ cá nhân</h1>
          <p class="text-base md:text-lg leading-relaxed"
             [style.color]="isDark() ? '#71717A' : '#71717A'">
            Quản lý thông tin tài khoản, địa chỉ giao hàng và theo dõi đơn hàng của bạn.
          </p>
        </header>

        <!-- Asymmetric Layout -->
        <div class="grid grid-cols-1 lg:grid-cols-12 gap-12 lg:gap-24 items-start">

          <!-- Sidebar (Left - 3 Cols) -->
          <aside class="lg:col-span-3 sticky top-28">
            <nav class="flex flex-col gap-1.5">

              <a routerLink="/profile/info" [routerLinkActiveOptions]="{exact: true}"
                 [routerLinkActive]="isDark() ? 'bg-white/10 text-white border-white/10' : 'bg-white text-[#18181B] border-zinc-200 shadow-[0_4px_24px_rgba(0,0,0,0.04)]'"
                 class="px-5 py-3.5 rounded-[14px] text-[13px] font-medium border border-transparent transition-all duration-300 flex items-center justify-between group"
                 [style.color]="isDark() ? '#71717A' : '#71717A'">
                <div class="flex items-center gap-3">
                  <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M20 21v-2a4 4 0 0 0-4-4H8a4 4 0 0 0-4 4v2"></path><circle cx="12" cy="7" r="4"></circle></svg>
                  Thông tin chung
                </div>
                <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="opacity-0 -translate-x-2 group-hover:opacity-100 group-hover:translate-x-0 transition-all duration-300"><path d="m9 18 6-6-6-6"/></svg>
              </a>

              <a routerLink="/profile/address"
                 [routerLinkActive]="isDark() ? 'bg-white/10 text-white border-white/10' : 'bg-white text-[#18181B] border-zinc-200 shadow-[0_4px_24px_rgba(0,0,0,0.04)]'"
                 class="px-5 py-3.5 rounded-[14px] text-[13px] font-medium border border-transparent transition-all duration-300 flex items-center justify-between group"
                 [style.color]="isDark() ? '#71717A' : '#71717A'">
                <div class="flex items-center gap-3">
                  <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M21 10c0 7-9 13-9 13s-9-6-9-13a9 9 0 0 1 18 0z"></path><circle cx="12" cy="10" r="3"></circle></svg>
                  Sổ địa chỉ
                </div>
                <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="opacity-0 -translate-x-2 group-hover:opacity-100 group-hover:translate-x-0 transition-all duration-300"><path d="m9 18 6-6-6-6"/></svg>
              </a>

              <div class="h-px w-full my-3"
                   [style.background]="isDark() ? 'rgba(255,255,255,0.06)' : '#E4E4E7'"></div>

              <a routerLink="/profile/orders"
                 [routerLinkActive]="isDark() ? 'bg-white/10 text-white border-white/10' : 'bg-white text-[#18181B] border-zinc-200 shadow-[0_4px_24px_rgba(0,0,0,0.04)]'"
                 class="px-5 py-3.5 rounded-[14px] text-[13px] font-medium border border-transparent transition-all duration-300 flex items-center justify-between group"
                 [style.color]="isDark() ? '#71717A' : '#71717A'">
                <div class="flex items-center gap-3">
                  <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><circle cx="9" cy="21" r="1"></circle><circle cx="20" cy="21" r="1"></circle><path d="M1 1h4l2.68 13.39a2 2 0 0 0 2 1.61h9.72a2 2 0 0 0 2-1.61L23 6H6"></path></svg>
                  Đơn hàng của tôi
                </div>
                <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="opacity-0 -translate-x-2 group-hover:opacity-100 group-hover:translate-x-0 transition-all duration-300"><path d="m9 18 6-6-6-6"/></svg>
              </a>

            </nav>
          </aside>

          <!-- Main Content (Right - 9 Cols) -->
          <main class="lg:col-span-9">
            <router-outlet></router-outlet>
          </main>

        </div>
      </div>
    </div>
  `
})
export class ProfileLayoutComponent {
  private themeService = inject(ThemeService);
  isDark = this.themeService.isDark;

  toggleTheme(): void {
    this.themeService.toggle();
  }
}
