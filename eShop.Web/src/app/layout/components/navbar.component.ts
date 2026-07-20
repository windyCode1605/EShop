import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router, NavigationEnd } from '@angular/router';
import { TokenService } from '../../core/service-proxies/token.service';
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
          <span class="shell-nav__mark">A</span>
          <span class="shell-nav__name">Atelier</span>
        </a>

        <!-- Main Links -->
        <div class="shell-nav__links">
          <a class="shell-nav__link" routerLink="/dashboard" routerLinkActive="is-active">Sản phẩm</a>
          <a class="shell-nav__link" routerLink="/product" routerLinkActive="is-active">Bộ sưu tập</a>
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

        <!-- Actions: Cart & Auth -->
        <div class="shell-nav__actions">
          <a routerLink="/cart" class="shell-nav__cart">
            <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
              <circle cx="9" cy="21" r="1"></circle><circle cx="20" cy="21" r="1"></circle>
              <path d="M1 1h4l2.68 13.39a2 2 0 0 0 2 1.61h9.72a2 2 0 0 0 2-1.61L23 6H6"></path>
            </svg>
            Giỏ hàng
          </a>
          
          <div class="shell-nav__divider"></div>

          <ng-container *ngIf="isLoggedIn(); else loginBtn">
            <div class="shell-nav__user">
              <a routerLink="/profile" class="shell-nav__user-name" routerLinkActive="is-active">Hồ sơ của tôi</a>
              <a routerLink="/order" class="shell-nav__user-name" routerLinkActive="is-active">Đơn hàng của tôi</a>
              <button (click)="logout()" class="shell-nav__logout">
                Đăng xuất
              </button>
            </div>
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
      }
      .shell-nav__mark {
        width: 36px; height: 36px;
        background: var(--color-text-primary);
        border-radius: 10px;
        display: flex;
        align-items: center;
        justify-content: center;
        font-weight: 800;
        font-size: 17px;
        color: white;
        letter-spacing: -1px;
      }
      .shell-nav__name {
        font-size: 12px;
        font-weight: 600;
        letter-spacing: 0.2em;
        color: var(--color-text-muted);
        text-transform: uppercase;
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
      .shell-nav__cart {
        font-size: 14px;
        font-weight: 500;
        color: var(--color-text-primary);
        text-decoration: none;
        display: flex;
        align-items: center;
        gap: 8px;
        transition: color 0.2s ease;
      }
      .shell-nav__cart:hover {
        color: var(--color-accent);
      }
      .shell-nav__divider {
        width: 1px;
        height: 16px;
        background: var(--color-border);
      }
      .shell-nav__user {
        display: flex;
        align-items: center;
        gap: 16px;
      }
      .shell-nav__user-name {
        font-size: 14px;
        font-weight: 500;
        color: var(--color-text-primary);
        text-decoration: none;
        transition: color 0.2s ease;
      }
      .shell-nav__user-name:hover,
      .shell-nav__user-name.is-active {
        color: var(--color-accent);
      }
      .shell-nav__logout {
        font-size: 14px;
        font-weight: 500;
        color: var(--color-text-secondary);
        background: none;
        border: none;
        cursor: pointer;
        transition: color 0.2s ease;
      }
      .shell-nav__logout:hover {
        color: var(--color-text-primary);
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
  
  isLoggedIn = signal<boolean>(false);

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
    this.isLoggedIn.set(!!token);
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