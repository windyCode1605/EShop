import { Component } from '@angular/core';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  template: `
    <div class="landing-page">
      <!-- Asymmetric Hero Section -->
      <section class="hero-section">
        <div class="hero-content">
          <span class="hero-meta">EShop Premium</span>
          <h1 class="hero-title">
            The quiet luxury of <br/> seamless shopping.
          </h1>
          <p class="hero-desc">
            Discover a curated collection of essentials, designed with intention and crafted without compromise. 
            Experience the new standard of digital commerce.
          </p>
          <div class="hero-actions">
            <button class="app-button" (click)="navigateToProducts()">
              Explore Collection
            </button>
          </div>
        </div>
        <div class="hero-visual">
          <!-- Inline Image Typography / Editorial Image -->
          <div class="editorial-image-wrapper">
            <img 
              src="https://images.unsplash.com/photo-1491553895911-0055eca6402d?auto=format&fit=crop&q=80&w=1000" 
              alt="Premium lifestyle"
              class="editorial-image"
            />
          </div>
        </div>
      </section>

      <!-- Featured Categories (Zigzag / Asymmetric) -->
      <section class="featured-section">
        <div class="section-header">
          <h2 class="text-h2">Curated Selections</h2>
          <p class="text-body">Our finest categories, handpicked for quality and design.</p>
        </div>

        <div class="zigzag-layout">
          <div class="zigzag-item">
            <div class="zigzag-image">
              <img src="https://images.unsplash.com/photo-1505740420928-5e560c06d30e?auto=format&fit=crop&q=80&w=800" alt="Audio"/>
            </div>
            <div class="zigzag-text">
              <span class="text-meta">01 // Acoustic</span>
              <h3 class="text-h1">High-Fidelity Audio</h3>
              <p class="text-body">Experience sound in its purest form. Our acoustic collection is engineered for audiophiles who demand nothing but perfection.</p>
              <a href="#" class="inline-link">Discover Audio →</a>
            </div>
          </div>

          <div class="zigzag-item reverse">
            <div class="zigzag-image">
              <img src="https://images.unsplash.com/photo-1523275335684-37898b6baf30?auto=format&fit=crop&q=80&w=800" alt="Watches"/>
            </div>
            <div class="zigzag-text">
              <span class="text-meta">02 // Timepieces</span>
              <h3 class="text-h1">Precision Wearables</h3>
              <p class="text-body">Master time with our exclusive range of smart and analog timepieces. Crafted for those who value every second.</p>
              <a href="#" class="inline-link">Discover Wearables →</a>
            </div>
          </div>
        </div>
      </section>
    </div>
  `,
  styles: [
    `
      .landing-page {
        max-width: 1400px;
        margin: 0 auto;
        padding: 0 var(--spacing-24);
      }

      /* Hero Section */
      .hero-section {
        display: grid;
        grid-template-columns: 1fr 1fr;
        gap: var(--spacing-64);
        min-height: 80vh;
        align-items: center;
        padding-top: var(--spacing-48);
        padding-bottom: var(--spacing-96);
      }

      .hero-meta {
        font-size: 12px;
        text-transform: uppercase;
        letter-spacing: 2px;
        color: var(--color-text-secondary);
        margin-bottom: var(--spacing-24);
        display: block;
      }

      .hero-title {
        font-size: clamp(3rem, 5vw, 4.5rem);
        line-height: 1.05;
        letter-spacing: -2px;
        font-weight: 600;
        margin-bottom: var(--spacing-32);
        color: var(--color-text-primary);
      }

      .hero-desc {
        font-size: 1.125rem;
        line-height: 1.6;
        color: var(--color-text-secondary);
        max-width: 480px;
        margin-bottom: var(--spacing-48);
      }

      .editorial-image-wrapper {
        border-radius: var(--radius-card);
        overflow: hidden;
        height: 70vh;
        position: relative;
        box-shadow: var(--shadow-medium);
      }

      .editorial-image {
        width: 100%;
        height: 100%;
        object-fit: cover;
      }

      /* Featured Categories */
      .featured-section {
        padding: var(--spacing-96) 0;
      }

      .section-header {
        max-width: 600px;
        margin-bottom: var(--spacing-64);
      }

      .zigzag-layout {
        display: grid;
        gap: var(--spacing-96);
      }

      .zigzag-item {
        display: grid;
        grid-template-columns: 5fr 4fr;
        gap: var(--spacing-64);
        align-items: center;
      }

      .zigzag-item.reverse {
        grid-template-columns: 4fr 5fr;
      }

      .zigzag-item.reverse .zigzag-image {
        order: 2;
      }

      .zigzag-item.reverse .zigzag-text {
        order: 1;
      }

      .zigzag-image {
        border-radius: var(--radius-card);
        overflow: hidden;
        aspect-ratio: 4/3;
      }

      .zigzag-image img {
        width: 100%;
        height: 100%;
        object-fit: cover;
        transition: transform 0.6s cubic-bezier(0.175, 0.885, 0.32, 1.275);
      }

      .zigzag-item:hover .zigzag-image img {
        transform: scale(1.03);
      }

      .zigzag-text {
        padding: var(--spacing-32);
      }

      .inline-link {
        display: inline-block;
        margin-top: var(--spacing-24);
        font-weight: 500;
        border-bottom: 1px solid var(--color-accent);
        padding-bottom: 2px;
      }

      .inline-link:hover {
        color: var(--color-text-secondary);
        border-color: var(--color-text-secondary);
      }

      /* Responsive */
      @media (max-width: 992px) {
        .hero-section {
          grid-template-columns: 1fr;
          min-height: auto;
          text-align: center;
        }

        .hero-desc {
          margin: 0 auto var(--spacing-48) auto;
        }

        .editorial-image-wrapper {
          height: 50vh;
        }

        .zigzag-item, .zigzag-item.reverse {
          grid-template-columns: 1fr;
        }

        .zigzag-item.reverse .zigzag-image {
          order: 1;
        }
        .zigzag-item.reverse .zigzag-text {
          order: 2;
        }
      }
    `
  ]
})
export class DashboardComponent {
  navigateToProducts() {
    window.location.href = '/product';
  }
}
