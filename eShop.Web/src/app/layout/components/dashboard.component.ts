import { Component } from '@angular/core';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  template: `
    <section class="page-shell hero">
      <div class="hero__panel surface">
        <p class="hero__eyebrow">eShop</p>
        <h1 class="section-title">A cleaner storefront shell, built with your own UI.</h1>
        <p class="section-copy">
          This starter now uses only the project styles and components you own. No Bootstrap chrome, no generic layout noise.
        </p>
      </div>
    </section>
  `,
  styles: [
    `
      .hero {
        padding-block: 3rem;
      }

      .hero__panel {
        padding: clamp(1.5rem, 4vw, 3rem);
        background:
          radial-gradient(circle at top right, rgba(138, 162, 180, 0.16), transparent 32%),
          linear-gradient(180deg, rgba(19, 28, 39, 0.96), rgba(10, 17, 24, 0.98));
      }

      .hero__eyebrow {
        margin-block-end: 0.75rem;
        color: var(--color-primary);
        text-transform: uppercase;
        letter-spacing: 0.18em;
        font-size: 0.75rem;
      }
    `
  ]
})
export class DashboardComponent { }
