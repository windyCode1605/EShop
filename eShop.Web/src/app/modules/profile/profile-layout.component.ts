import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-profile-layout',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <div class="min-h-[100dvh] bg-[#F9FAFB] text-zinc-900 pb-24 pt-12 md:pt-20">
      <div class="max-w-6xl mx-auto px-6 md:px-8">
        
        <!-- Premium Editorial Header -->
        <header class="mb-12 md:mb-16 max-w-2xl">
          <h1 class="text-4xl md:text-5xl font-medium tracking-tight mb-4">Hồ sơ cá nhân</h1>
          <p class="text-zinc-500 text-base md:text-lg leading-relaxed">
            Quản lý thông tin tài khoản, địa chỉ giao hàng và tuỳ chọn liên lạc của bạn tại đây.
          </p>
        </header>

        <!-- Asymmetric Layout -->
        <div class="grid grid-cols-1 lg:grid-cols-12 gap-12 lg:gap-20 items-start">
          
          <!-- Sidebar (Left - 3 Cols) -->
          <aside class="lg:col-span-3 sticky top-24">
            <nav class="flex flex-col gap-2">
              <a routerLink="/profile/info" routerLinkActive="bg-white text-zinc-900 shadow-[0_2px_12px_rgba(0,0,0,0.03)] border-zinc-200" [routerLinkActiveOptions]="{exact: true}"
                 class="px-5 py-3.5 rounded-[16px] text-sm font-medium text-zinc-500 hover:text-zinc-900 hover:bg-white/50 border border-transparent transition-all duration-250 flex items-center gap-3">
                <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M20 21v-2a4 4 0 0 0-4-4H8a4 4 0 0 0-4 4v2"></path><circle cx="12" cy="7" r="4"></circle></svg>
                Thông tin chung
              </a>
              
              <a routerLink="/profile/address" routerLinkActive="bg-white text-zinc-900 shadow-[0_2px_12px_rgba(0,0,0,0.03)] border-zinc-200"
                 class="px-5 py-3.5 rounded-[16px] text-sm font-medium text-zinc-500 hover:text-zinc-900 hover:bg-white/50 border border-transparent transition-all duration-250 flex items-center gap-3">
                <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M21 10c0 7-9 13-9 13s-9-6-9-13a9 9 0 0 1 18 0z"></path><circle cx="12" cy="10" r="3"></circle></svg>
                Sổ địa chỉ
              </a>

              <div class="h-px w-full bg-zinc-200/50 my-2"></div>

              <a routerLink="/profile/orders" routerLinkActive="bg-white text-zinc-900 shadow-[0_2px_12px_rgba(0,0,0,0.03)] border-zinc-200"
                 class="px-5 py-3.5 rounded-[16px] text-sm font-medium text-zinc-500 hover:text-zinc-900 hover:bg-white/50 border border-transparent transition-all duration-250 flex items-center gap-3">
                <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><circle cx="9" cy="21" r="1"></circle><circle cx="20" cy="21" r="1"></circle><path d="M1 1h4l2.68 13.39a2 2 0 0 0 2 1.61h9.72a2 2 0 0 0 2-1.61L23 6H6"></path></svg>
                Đơn hàng của tôi
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
export class ProfileLayoutComponent {}
