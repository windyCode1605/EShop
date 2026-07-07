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
          <span class="shell-nav__brand-text">EShop</span>
          <span class="shell-nav__brand-suffix">Premium</span>
        </a>

        <!-- Main Links -->
        <div class="shell-nav__links">
          <a class="shell-nav__link" routerLink="/dashboard" routerLinkActive="is-active">Discover</a>
          <a class="shell-nav__link" routerLink="/product" routerLinkActive="is-active">Collections</a>
        </div>

        <!-- Actions: Cart & Auth -->
        <div class="flex items-center gap-6">
          <a routerLink="/cart" class="text-sm font-medium text-[#18181B] hover:text-gray-500 transition-colors flex items-center gap-2">
            <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
              <circle cx="9" cy="21" r="1"></circle><circle cx="20" cy="21" r="1"></circle>
              <path d="M1 1h4l2.68 13.39a2 2 0 0 0 2 1.61h9.72a2 2 0 0 0 2-1.61L23 6H6"></path>
            </svg>
            Cart
          </a>
          
          <div class="w-[1px] h-4 bg-[#E4E4E7]"></div>

          <ng-container *ngIf="isLoggedIn(); else loginBtn">
            <div class="flex items-center gap-4">
              <span class="text-sm font-medium text-[#18181B]">Account</span>
              <button (click)="logout()" class="text-sm font-medium text-[#71717A] hover:text-[#18181B] transition-colors">
                Logout
              </button>
            </div>
          </ng-container>
          <ng-template #loginBtn>
            <a routerLink="/account/login" class="text-sm font-medium text-[#18181B] hover:text-gray-500 transition-colors">
              Sign In
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
        inset-block-start: 0;
        z-index: 50;
        backdrop-filter: blur(12px);
        background: rgba(255, 255, 255, 0.85);
        border-block-end: 1px solid var(--color-border);
      }

      .shell-nav__inner {
        display: flex;
        align-items: center;
        justify-content: space-between;
        min-block-size: 72px;
        padding-top: 0 !important;
        padding-bottom: 0 !important;
      }

      .shell-nav__brand {
        display: flex;
        align-items: baseline;
        gap: 0.4rem;
        text-decoration: none;
      }

      .shell-nav__brand-text {
        font-family: 'Outfit', sans-serif;
        font-size: 1.5rem;
        font-weight: 600;
        letter-spacing: -0.02em;
        color: var(--color-text-primary);
      }

      .shell-nav__brand-suffix {
        font-family: 'Inter', system-ui, sans-serif;
        font-size: 0.65rem;
        font-weight: 500;
        letter-spacing: 0.28em;
        text-transform: uppercase;
        color: var(--color-text-secondary);
        transform: translateY(1px);
      }

      .shell-nav__links {
        display: flex;
        align-items: center;
        gap: 2rem;
      }

      .shell-nav__link {
        position: relative;
        color: var(--color-text-secondary);
        font-family: 'Outfit', sans-serif;
        font-size: 0.95rem;
        font-weight: 500;
        text-decoration: none;
        padding: 0.5rem 0;
        transition: color 0.25s ease;
      }

      .shell-nav__link::after {
        content: '';
        position: absolute;
        inset-inline: 0;
        inset-block-end: 0;
        block-size: 1px;
        background: var(--color-accent);
        transform: scaleX(0);
        transform-origin: right;
        transition: transform 0.3s ease;
      }

      .shell-nav__link:hover {
        color: var(--color-text-primary);
      }

      .shell-nav__link:hover::after {
        transform: scaleX(1);
        transform-origin: left;
      }

      .shell-nav__link.is-active {
        color: var(--color-accent);
      }

      .shell-nav__link.is-active::after {
        transform: scaleX(1);
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
}