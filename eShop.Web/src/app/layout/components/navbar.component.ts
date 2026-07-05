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
        <a class="shell-nav__brand" routerLink="/">
          <span class="shell-nav__brand-text">Quang Nguyên</span>
          <span class="shell-nav__brand-suffix">Luxury</span>
        </a>
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
        backdrop-filter: blur(16px) saturate(140%);
        background: linear-gradient(180deg, rgba(8, 8, 10, 0.94) 0%, rgba(8, 8, 10, 0.86) 100%);
        border-block-end: 1px solid rgba(201, 162, 39, 0.18);
      }

      .shell-nav__inner {
        display: flex;
        align-items: center;
        justify-content: space-between;
        gap: 1.5rem;
        min-block-size: 76px;
      }

      .shell-nav__brand {
        display: flex;
        align-items: baseline;
        gap: 0.4rem;
        color: #f5f0e6;
        text-decoration: none;
      }

      .shell-nav__brand-text {
        font-family: 'Playfair Display', Georgia, 'Times New Roman', serif;
        font-size: 1.35rem;
        font-weight: 600;
        letter-spacing: 0.03em;
        color: #f5f0e6;
      }

      .shell-nav__brand-suffix {
        font-family: 'Inter', system-ui, sans-serif;
        font-size: 0.65rem;
        font-weight: 500;
        letter-spacing: 0.28em;
        text-transform: uppercase;
        color: #c9a227;
        transform: translateY(1px);
      }

      .shell-nav__links {
        display: flex;
        align-items: center;
        gap: 0.5rem;
      }

      .shell-nav__link {
        position: relative;
        color: rgba(245, 240, 230, 0.68);
        font-family: 'Inter', system-ui, sans-serif;
        font-size: 0.8rem;
        font-weight: 500;
        letter-spacing: 0.08em;
        text-transform: uppercase;
        text-decoration: none;
        padding: 0.6rem 1.1rem;
        border-radius: 2px;
        transition: color 0.25s ease;
      }

      .shell-nav__link::after {
        content: '';
        position: absolute;
        inset-inline: 1.1rem;
        inset-block-end: 0.35rem;
        block-size: 1px;
        background: #c9a227;
        transform: scaleX(0);
        transform-origin: left;
        transition: transform 0.3s ease;
      }

      .shell-nav__link:hover {
        color: #f5f0e6;
      }

      .shell-nav__link:hover::after {
        transform: scaleX(1);
      }

      .shell-nav__link.is-active {
        color: #c9a227;
      }

      .shell-nav__link.is-active::after {
        transform: scaleX(1);
      }
    `
  ]
})
export class NavbarComponent {
}