import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <nav class="shell-nav">
      <div class="page-shell shell-nav__inner">
        <a class="shell-nav__brand" routerLink="/">Quang Nguyên Luxury</a>
        <div class="shell-nav__links">
          <a class="shell-nav__link" routerLink="/product-manager" routerLinkActive="is-active">Products</a>
          <a class="shell-nav__link" routerLink="/dashboard" routerLinkActive="is-active">Dashboard</a>
        </div>
      </div>
    </nav>
  `,
  styles: [
    `
      .shell-nav {
        position: sticky;
        inset-block-start: 0;
        z-index: 20;
        backdrop-filter: blur(14px);
        background: rgba(10, 17, 24, 0.82);
        border-block-end: 1px solid var(--color-border);
      }

      .shell-nav__inner {
        display: flex;
        align-items: center;
        justify-content: space-between;
        gap: 1rem;
        min-block-size: 72px;
      }

      .shell-nav__brand {
        color: var(--color-text);
        font-weight: 700;
        letter-spacing: 0.04em;
        text-transform: uppercase;
      }

      .shell-nav__links {
        display: flex;
        align-items: center;
        gap: 0.75rem;
      }

      .shell-nav__link {
        color: var(--color-text-secondary);
        padding: 0.5rem 0.75rem;
        border-radius: 999px;
      }

      .shell-nav__link.is-active {
        color: var(--color-background);
        background: var(--color-primary);
      }
    `
  ]
})
export class NavbarComponent {
}
