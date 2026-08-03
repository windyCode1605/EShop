import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router, NavigationEnd } from '@angular/router';
import { TokenService } from '../../core/service-proxies/token.service';
import { CartService } from '../../modules/cart/services/cart.service';
import { filter } from 'rxjs';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <nav class="shell-nav">
      <div class="page-shell shell-nav__inner">
        <!-- Logo -->
        <a class="shell-nav__brand" routerLink="/">
          <!-- MQN SVG Mark: Navy + Gold brand identity -->
          <svg width="38" height="38" viewBox="0 0 44 44" fill="none" xmlns="http://www.w3.org/2000/svg" class="shell-nav__mark-svg">
            <!-- Outer gold ring -->
            <circle cx="22" cy="22" r="20" stroke="#C9A961" stroke-width="1.5" fill="none" opacity="0.55"/>
            <!-- M strokes — Navy -->
            <path d="M7 31 L7 13 L16 23 L25 13 L25 31" stroke="#0F3460" stroke-width="2.2" stroke-linecap="round" stroke-linejoin="round" fill="none"/>
            <!-- Q arc — Gold -->
            <path d="M29 17 Q38 17 38 24 Q38 31 29 31 L29 17" stroke="#C9A961" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" fill="none"/>
            <!-- Q tail — Gold -->
            <path d="M34 28 L39 34" stroke="#C9A961" stroke-width="2" stroke-linecap="round"/>
          </svg>
          <div class="shell-nav__brand-text">
            <span class="shell-nav__name">QUANG NGUYÊN</span>
            <span class="shell-nav__sub">MAI QUANG NGUYÊN</span>
          </div>
        </a>


        <!-- Main Links -->
        <div class="shell-nav__links">
          <a class="shell-nav__link" routerLink="/dashboard" routerLinkActive="is-active" [routerLinkActiveOptions]="{exact: true}">Trang chủ</a>
          <a class="shell-nav__link" routerLink="/product" routerLinkActive="is-active">Sản phẩm</a>
        </div>

        <!-- Search Bar -->
        <div class="shell-nav__search">
          <svg class="search-icon" xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
            <circle cx="11" cy="11" r="8"></circle>
            <line x1="21" y1="21" x2="16.65" y2="16.65"></line>
          </svg>
          <input type="text" placeholder="Tìm kiếm sản phẩm..." class="search-input" (keyup.enter)="onSearch($event)">
          <div class="search-shortcut">⌘K</div>
        </div>

        <!-- Actions: Icons -->
        <div class="shell-nav__actions">
          
          <!-- Hỗ trợ khách hàng -->
          <button class="shell-nav__icon-btn" title="Hỗ trợ khách hàng">
            <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
              <path d="M15.05 5A5 5 0 0 1 19 8.95M15.05 1A9 9 0 0 1 23 8.94m-1 7.98v3a2 2 0 0 1-2.18 2 19.79 19.79 0 0 1-8.63-3.07 19.5 19.5 0 0 1-6-6 19.79 19.79 0 0 1-3.07-8.67A2 2 0 0 1 4.11 2h3a2 2 0 0 1 2 1.72 12.84 12.84 0 0 0 .7 2.81 2 2 0 0 1-.45 2.11L8.09 9.91a16 16 0 0 0 6 6l1.27-1.27a2 2 0 0 1 2.11-.45 12.84 12.84 0 0 0 2.81.7A2 2 0 0 1 22 16.92z"></path>
            </svg>
          </button>

          <!-- Giỏ hàng -->
          <a routerLink="/cart" class="shell-nav__icon-btn relative" title="Giỏ hàng">
            <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
              <circle cx="9" cy="21" r="1"></circle><circle cx="20" cy="21" r="1"></circle>
              <path d="M1 1h4l2.68 13.39a2 2 0 0 0 2 1.61h9.72a2 2 0 0 0 2-1.61L23 6H6"></path>
            </svg>
            <span *ngIf="totalItems() > 0" class="absolute top-1 right-0 bg-zinc-900 text-white text-[10px] font-bold h-4 min-w-[16px] px-1 rounded-full flex items-center justify-center translate-x-1 -translate-y-1 border-2 border-[#F9FAFB] shadow-sm">
              {{ totalItems() > 99 ? '99+' : totalItems() }}
            </span>
          </a>
          
          <div class="shell-nav__divider"></div>

          <ng-container *ngIf="isLoggedIn(); else loginBtn">
            <!-- Hồ sơ (bao gồm Đơn hàng bên trong) -->
            <a routerLink="/profile" class="shell-nav__icon-btn" routerLinkActive="is-active" title="Hồ sơ của tôi">
              <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                <path d="M20 21v-2a4 4 0 0 0-4-4H8a4 4 0 0 0-4 4v2"></path>
                <circle cx="12" cy="7" r="4"></circle>
              </svg>
            </a>
            <!-- Đăng xuất -->
            <button (click)="logout()" class="shell-nav__icon-btn logout-btn" title="Đăng xuất">
              <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                <path d="M9 21H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h4"></path>
                <polyline points="16 17 21 12 16 7"></polyline>
                <line x1="21" y1="12" x2="9" y2="12"></line>
              </svg>
            </button>
          </ng-container>
          
          <ng-template #loginBtn>
            <a routerLink="/account/login" class="shell-nav__cta">
              Đăng nhập
            </a>
          </ng-template>
        </div>
      </div>
    </nav>
  `,
  styles: [
    `
      .shell-nav {
        position: sticky;
        top: 0;
        z-index: 50;
        backdrop-filter: blur(12px);
        background: rgba(248, 247, 245, 0.85);
        border-bottom: 1px solid var(--color-border);
      }

      .shell-nav__inner {
        display: flex;
        align-items: center;
        justify-content: space-between;
        min-height: 72px;
        padding-top: 0 !important;
        padding-bottom: 0 !important;
        gap: 24px;
      }

      /* LOGO */
      .shell-nav__brand {
        display: flex;
        align-items: center;
        gap: 10px;
        text-decoration: none;
        flex-shrink: 0;
        transition: opacity 200ms ease;
      }
      .shell-nav__brand:hover {
        opacity: 0.85;
      }
      .shell-nav__mark-svg {
        flex-shrink: 0;
        filter: drop-shadow(0 1px 4px rgba(201, 169, 97, 0.18));
        transition: filter 250ms ease;
      }
      .shell-nav__brand:hover .shell-nav__mark-svg {
        filter: drop-shadow(0 2px 8px rgba(201, 169, 97, 0.32));
      }
      .shell-nav__brand-text {
        display: flex;
        flex-direction: column;
        gap: 1px;
      }
      .shell-nav__name {
        font-size: 13px;
        font-weight: 700;
        letter-spacing: 0.18em;
        color: #0F3460;
        text-transform: uppercase;
        line-height: 1;
      }
      .shell-nav__sub {
        font-size: 9px;
        font-weight: 500;
        letter-spacing: 0.1em;
        color: #A88A3E;
        text-transform: uppercase;
        line-height: 1;
      }

      /* LINKS */
      .shell-nav__links {
        display: flex;
        align-items: center;
        gap: 2rem;
        margin-right: auto;
      }
      .shell-nav__link {
        position: relative;
        color: var(--color-text-secondary);
        font-family: 'Outfit', sans-serif;
        font-size: 14px;
        font-weight: 500;
        text-decoration: none;
        padding: 0.5rem 0;
        transition: color 0.2s ease;
      }
      .shell-nav__link:hover,
      .shell-nav__link.is-active {
        color: var(--color-text-primary);
      }
      .shell-nav__link::after {
        content: '';
        position: absolute;
        inset-inline: 0;
        bottom: 0;
        height: 1.5px;
        background: var(--color-text-primary);
        transform: scaleX(0);
        transform-origin: right;
        transition: transform 0.3s ease;
      }
      .shell-nav__link:hover::after,
      .shell-nav__link.is-active::after {
        transform: scaleX(1);
        transform-origin: left;
      }

      /* SEARCH BAR */
      .shell-nav__search {
        position: relative;
        display: flex;
        align-items: center;
        width: 260px;
        height: 40px;
        background: #FFFFFF;
        border: 1px solid var(--color-border);
        border-radius: 999px;
        padding: 0 16px;
        transition: all 0.3s cubic-bezier(0.16, 1, 0.3, 1);
      }
      .shell-nav__search:focus-within {
        width: 320px;
        border-color: var(--color-text-primary);
        box-shadow: 0 4px 12px rgba(0, 0, 0, 0.05);
      }
      .search-icon {
        color: var(--color-text-secondary);
        flex-shrink: 0;
        transition: color 0.3s ease;
      }
      .shell-nav__search:focus-within .search-icon {
        color: var(--color-text-primary);
      }
      .search-input {
        flex: 1;
        border: none;
        background: transparent;
        padding: 0 12px;
        font-size: 14px;
        color: var(--color-text-primary);
        outline: none;
        font-family: inherit;
      }
      .search-input::placeholder {
        color: var(--color-text-muted);
      }
      .search-shortcut {
        font-size: 11px;
        font-weight: 600;
        color: var(--color-text-muted);
        background: #F4F4F5;
        padding: 2px 6px;
        border-radius: 4px;
        border: 1px solid var(--color-border);
        pointer-events: none;
      }

      /* ACTIONS */
      .shell-nav__actions {
        display: flex;
        align-items: center;
        gap: 24px;
        flex-shrink: 0;
      }
      .shell-nav__icon-btn {
        display: flex;
        align-items: center;
        justify-content: center;
        width: 40px;
        height: 40px;
        border-radius: 50%;
        color: var(--color-text-secondary);
        background: transparent;
        border: none;
        cursor: pointer;
        transition: all 0.25s cubic-bezier(0.16, 1, 0.3, 1);
      }
      .shell-nav__icon-btn:hover,
      .shell-nav__icon-btn.is-active {
        color: var(--color-text-primary);
        background: var(--color-surface-secondary, #F4F4F5);
        transform: scale(1.05);
      }
      .shell-nav__icon-btn:active {
        transform: scale(0.95);
      }
      .logout-btn:hover {
        color: #ef4444;
        background: #fef2f2;
      }
      .shell-nav__divider {
        width: 1px;
        height: 24px;
        background-color: var(--color-border, #E4E4E7);
      }

      .shell-nav__cta {
        background: var(--color-text-primary);
        color: white;
        border: none;
        border-radius: 12px;
        padding: 10px 20px;
        font-size: 13.5px;
        font-weight: 600;
        font-family: 'Outfit', sans-serif;
        cursor: pointer;
        text-decoration: none;
        transition: transform 0.25s cubic-bezier(0.22, 1, 0.36, 1), box-shadow 0.25s ease;
      }
      .shell-nav__cta:hover {
        transform: translateY(-1px);
        box-shadow: 0 6px 20px rgba(26, 25, 23, 0.25);
      }
    `
  ]
})
export class NavbarComponent implements OnInit {
  private tokenService = inject(TokenService);
  private router = inject(Router);
  private cartService = inject(CartService);

  isLoggedIn = signal<boolean>(false);
  totalItems = this.cartService.totalItems;

  ngOnInit() {
    this.checkAuthStatus();

    // Listen to router changes to re-check auth status (in case of login/logout redirects)
    this.router.events.pipe(
      filter(event => event instanceof NavigationEnd)
    ).subscribe(() => {
      this.checkAuthStatus();
    });
  }

  checkAuthStatus() {
    const token = this.tokenService.getToken();
    const wasLoggedIn = this.isLoggedIn();
    this.isLoggedIn.set(!!token);

    // Nếu vừa đăng nhập hoặc load lần đầu, gọi API lấy số lượng giỏ hàng
    if (!!token && !wasLoggedIn) {
      this.cartService.getMyCart().subscribe();
    }
  }

  logout() {
    this.tokenService.clearAllCookie();
    this.isLoggedIn.set(false);
    this.router.navigate(['/dashboard']);
  }

  onSearch(event: Event) {
    const input = event.target as HTMLInputElement;
    const keyword = input.value.trim();
    if (keyword) {
      // Navigate to product page and pass the search query in queryParams
      this.router.navigate(['/product'], { queryParams: { q: keyword } });
    }
  }
}
