import { Component, AfterViewInit, ElementRef, ViewChildren, QueryList } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule],
  styles: [`
    /* ─────────────────────────────────────────────────────────
       DASHBOARD — Premium with Wave Animation
    ───────────────────────────────────────────────────────── */
    .dashboard {
      min-height: 100dvh;
      background: #F8F7F5;
      overflow-x: hidden;
    }

    /* Wave Background */
    .wave-bg {
      position: fixed;
      inset: 0;
      pointer-events: none;
      z-index: 0;
      overflow: hidden;
      opacity: 0.45;
    }

    .wave-svg {
      position: absolute;
      bottom: 0;
      left: 0;
      width: 200%;
      animation: wave-move 18s linear infinite;
    }

    .wave-svg--2 {
      animation: wave-move 26s linear infinite reverse;
      opacity: 0.6;
      bottom: -20px;
    }

    .wave-svg--3 {
      animation: wave-move 14s linear infinite;
      opacity: 0.35;
      bottom: -40px;
    }

    @keyframes wave-move {
      0%   { transform: translateX(0); }
      100% { transform: translateX(-50%); }
    }

    /* Orb decorations */
    .orb {
      position: fixed;
      border-radius: 50%;
      pointer-events: none;
      z-index: 0;
    }

    .orb--1 {
      width: 600px; height: 600px;
      top: -200px; right: -100px;
      background: radial-gradient(circle, rgba(197, 168, 130, 0.12) 0%, transparent 65%);
      animation: float-orb 22s ease-in-out infinite;
    }

    .orb--2 {
      width: 400px; height: 400px;
      bottom: 10%; left: -80px;
      background: radial-gradient(circle, rgba(197, 168, 130, 0.08) 0%, transparent 65%);
      animation: float-orb 16s ease-in-out infinite;
      animation-delay: -8s;
    }

    @keyframes float-orb {
      0%, 100% { transform: translate(0, 0) scale(1); }
      33%       { transform: translate(25px, -30px) scale(1.06); }
      66%       { transform: translate(-15px, 20px) scale(0.96); }
    }

    /* Content layer */
    .dashboard__content {
      position: relative;
      z-index: 1;
    }

    /* Container */
    .container {
      max-width: 1280px;
      margin: 0 auto;
      padding: 0 32px;
    }

    /* ─── NAV ─── */
    .dash-nav {
      display: flex;
      align-items: center;
      justify-content: space-between;
      padding: 22px 0;
      border-bottom: 1px solid #E5E3DF;
    }

    .dash-nav__logo {
      display: flex;
      align-items: center;
      gap: 10px;
      cursor: pointer;
    }

    .dash-nav__mark {
      width: 36px; height: 36px;
      background: #1A1917;
      border-radius: 10px;
      display: flex;
      align-items: center;
      justify-content: center;
      font-weight: 800;
      font-size: 17px;
      color: white;
      letter-spacing: -1px;
    }

    .dash-nav__name {
      font-size: 12px;
      font-weight: 600;
      letter-spacing: 0.2em;
      color: #9D9890;
      text-transform: uppercase;
    }

    .dash-nav__links {
      display: flex;
      align-items: center;
      gap: 32px;
    }

    .dash-nav__link {
      font-size: 14px;
      color: #6B6864;
      text-decoration: none;
      font-weight: 500;
      transition: color 150ms ease;
      cursor: pointer;
      background: none;
      border: none;
      font-family: 'Outfit', sans-serif;
    }

    .dash-nav__link:hover { color: #1A1917; }

    .dash-nav__cta {
      background: #1A1917;
      color: white;
      border: none;
      border-radius: 12px;
      padding: 10px 20px;
      font-size: 13.5px;
      font-weight: 600;
      font-family: 'Outfit', sans-serif;
      cursor: pointer;
      transition: transform 250ms cubic-bezier(0.22, 1, 0.36, 1), box-shadow 250ms ease;
    }

    .dash-nav__cta:hover {
      transform: translateY(-1px);
      box-shadow: 0 6px 20px rgba(26, 25, 23, 0.25);
    }

    /* ─── HERO ─── */
    .hero {
      display: grid;
      grid-template-columns: 1fr 1fr;
      gap: 80px;
      align-items: center;
      min-height: 88vh;
      padding: 60px 0 80px;
    }

    .hero__text { max-width: 540px; }

    .hero__label {
      display: inline-flex;
      align-items: center;
      gap: 8px;
      font-size: 11px;
      font-weight: 600;
      text-transform: uppercase;
      letter-spacing: 0.14em;
      color: #A8885E;
      margin-bottom: 24px;
      padding: 6px 12px;
      background: rgba(197, 168, 130, 0.1);
      border: 1px solid rgba(197, 168, 130, 0.25);
      border-radius: 100px;
    }

    .hero__label-dot {
      width: 5px; height: 5px;
      background: #C5A882;
      border-radius: 50%;
    }

    .hero__h1 {
      font-family: 'Cormorant Garamond', Georgia, serif;
      font-size: clamp(52px, 6vw, 80px);
      font-weight: 500;
      font-style: italic;
      color: #111111;
      line-height: 1.05;
      letter-spacing: -1px;
      margin: 0 0 24px;
    }

    .hero__h1 em {
      font-style: normal;
      font-weight: 600;
      color: #C9A961;
    }

    .hero__sub {
      font-size: 17px;
      color: #6B6864;
      line-height: 1.65;
      margin: 0 0 40px;
      max-width: 420px;
    }

    .hero__actions {
      display: flex;
      align-items: center;
      gap: 16px;
    }

    .hero__btn-primary {
      display: inline-flex;
      align-items: center;
      gap: 8px;
      background: #111111;
      color: white;
      border: none;
      border-radius: 8px;
      padding: 16px 28px;
      font-size: 12px;
      font-weight: 700;
      font-family: 'Outfit', sans-serif;
      text-transform: uppercase;
      letter-spacing: 0.1em;
      cursor: pointer;
      transition: transform 280ms cubic-bezier(0.22, 1, 0.36, 1), box-shadow 280ms ease;
    }

    .hero__btn-primary:hover {
      transform: translateY(-2px);
      box-shadow: 0 8px 24px rgba(0, 0, 0, 0.15);
    }

    .hero__btn-primary svg { transition: transform 250ms ease; }
    .hero__btn-primary:hover svg { transform: translateX(3px); }

    .hero__btn-ghost {
      display: inline-flex;
      align-items: center;
      gap: 6px;
      background: transparent;
      color: #6B6864;
      border: 1px solid #E5E3DF;
      border-radius: 14px;
      padding: 15px 24px;
      font-size: 15px;
      font-weight: 500;
      font-family: 'Outfit', sans-serif;
      cursor: pointer;
      transition: all 150ms ease;
    }

    .hero__btn-ghost:hover {
      background: #F3F2F0;
      color: #1A1917;
      border-color: #D4D0CB;
    }

    .hero__image-wrap {
      position: relative;
      height: 560px;
      border-radius: 28px;
      overflow: hidden;
      background: #ECEAE7;
    }

    .hero__img {
      width: 100%;
      height: 100%;
      object-fit: cover;
      transition: transform 600ms ease;
    }

    .hero__image-wrap:hover .hero__img { transform: scale(1.03); }

    .hero__badge {
      position: absolute;
      bottom: 24px;
      left: 24px;
      background: rgba(255, 255, 255, 0.92);
      backdrop-filter: blur(16px);
      border: 1px solid rgba(255, 255, 255, 0.8);
      border-radius: 16px;
      padding: 14px 18px;
      display: flex;
      align-items: center;
      gap: 12px;
    }

    .hero__badge-icon {
      width: 36px; height: 36px;
      background: #F8F7F5;
      border-radius: 10px;
      display: flex;
      align-items: center;
      justify-content: center;
      color: #C5A882;
    }

    .hero__badge-label {
      font-size: 12px;
      font-weight: 500;
      color: #6B6864;
    }

    .hero__badge-val {
      font-size: 18px;
      font-weight: 700;
      color: #1A1917;
      line-height: 1;
    }

    /* ─── STATS ─── */
    .stats-row {
      display: grid;
      grid-template-columns: repeat(3, 1fr);
      gap: 1px;
      background: #E5E3DF;
      border: 1px solid #E5E3DF;
      border-radius: 20px;
      overflow: hidden;
      margin-bottom: 96px;
    }

    .stat-item {
      background: #FFFFFF;
      padding: 28px 32px;
      display: flex;
      flex-direction: column;
      gap: 4px;
      transition: background 200ms ease;
    }

    .stat-item:hover { background: #FAFAF9; }

    .stat-item__value {
      font-size: 32px;
      font-weight: 700;
      color: #1A1917;
      letter-spacing: -1px;
      line-height: 1;
    }

    .stat-item__label {
      font-size: 13px;
      color: #9D9890;
      font-weight: 500;
    }

    /* ─── CATEGORIES ─── */
    .section-header {
      max-width: 560px;
      margin-bottom: 64px;
    }

    .section-eyebrow {
      font-size: 11px;
      font-weight: 600;
      text-transform: uppercase;
      letter-spacing: 0.14em;
      color: #A8885E;
      margin-bottom: 12px;
    }

    .section-title {
      font-size: 38px;
      font-weight: 600;
      color: #1A1917;
      letter-spacing: -1px;
      line-height: 1.15;
      margin: 0 0 16px;
    }

    .section-sub {
      font-size: 16px;
      color: #6B6864;
      margin: 0;
    }

    /* Category editorial items */
    .editorial-item {
      display: grid;
      grid-template-columns: 7fr 5fr;
      gap: 48px;
      align-items: center;
      padding: 64px 0;
      border-top: 1px solid #E5E3DF;
      opacity: 0;
      transform: translateY(30px);
      transition: opacity 0.6s ease, transform 0.6s cubic-bezier(0.22, 1, 0.36, 1);
    }

    .editorial-item.is-visible {
      opacity: 1;
      transform: translateY(0);
    }

    .editorial-item--reverse {
      grid-template-columns: 5fr 7fr;
    }

    .editorial-item--reverse .editorial-item__text { order: -1; }

    .editorial-item__img-wrap {
      border-radius: 24px;
      overflow: hidden;
      aspect-ratio: 4 / 3;
      background: #ECEAE7;
      position: relative;
    }

    .editorial-item__img {
      width: 100%; height: 100%;
      object-fit: cover;
      transition: transform 700ms cubic-bezier(0.22, 1, 0.36, 1);
    }

    .editorial-item__img-wrap:hover .editorial-item__img { transform: scale(1.06); }

    .editorial-item__text { padding: 16px 8px; }

    .editorial-item__num {
      font-size: 11px;
      font-weight: 700;
      letter-spacing: 0.1em;
      color: #C5A882;
      margin-bottom: 16px;
      text-transform: uppercase;
    }

    .editorial-item__title {
      font-size: 28px;
      font-weight: 600;
      color: #1A1917;
      letter-spacing: -0.5px;
      margin: 0 0 16px;
      line-height: 1.2;
    }

    .editorial-item__desc {
      font-size: 15px;
      color: #6B6864;
      line-height: 1.65;
      margin: 0 0 28px;
    }

    .editorial-item__link {
      display: inline-flex;
      align-items: center;
      gap: 8px;
      font-size: 14px;
      font-weight: 600;
      color: #1A1917;
      text-decoration: none;
      border-bottom: 1.5px solid #1A1917;
      padding-bottom: 2px;
      transition: color 150ms ease, border-color 150ms ease, gap 200ms ease;
      cursor: pointer;
      background: none;
      border-top: none;
      border-left: none;
      border-right: none;
      font-family: 'Outfit', sans-serif;
    }

    .editorial-item__link:hover {
      color: #C5A882;
      border-bottom-color: #C5A882;
      gap: 12px;
    }

    /* ─── FOOTER ─── */
    .dash-footer {
      border-top: 1px solid #E5E3DF;
      padding: 40px 0;
      margin-top: 96px;
      display: flex;
      align-items: center;
      justify-content: space-between;
    }

    .dash-footer__copy {
      font-size: 13px;
      color: #9D9890;
    }

    /* ─── ANIMATIONS (initial load) ─── */
    .hero__text { animation: fade-up 0.7s 0.1s cubic-bezier(0.22, 1, 0.36, 1) both; }
    .hero__image-wrap { animation: fade-up 0.7s 0.25s cubic-bezier(0.22, 1, 0.36, 1) both; }
    .stats-row { animation: fade-up 0.6s 0.4s cubic-bezier(0.22, 1, 0.36, 1) both; }
    .section-header { animation: fade-up 0.6s 0.1s cubic-bezier(0.22, 1, 0.36, 1) both; }

    @keyframes fade-up {
      from { opacity: 0; transform: translateY(24px); }
      to   { opacity: 1; transform: translateY(0); }
    }

    /* ─── Responsive ─── */
    @media (max-width: 1024px) {
      .hero { grid-template-columns: 1fr; gap: 48px; min-height: auto; }
      .hero__image-wrap { height: 400px; }
      .editorial-item, .editorial-item--reverse { grid-template-columns: 1fr; }
      .editorial-item--reverse .editorial-item__text { order: 0; }
      .stats-row { grid-template-columns: 1fr; }
      .dash-nav__links { display: none; }
    }
  `],
  template: `
    <div class="dashboard">

      <!-- Fixed wave background -->
      <div class="wave-bg" aria-hidden="true">
        <svg class="wave-svg" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 1440 120" preserveAspectRatio="none">
          <path fill="rgba(197,168,130,0.15)" d="M0,60 C240,100 480,20 720,60 C960,100 1200,20 1440,60 L1440,120 L0,120 Z"/>
          <path fill="rgba(197,168,130,0.08)" d="M0,80 C360,40 720,100 1080,60 C1260,40 1380,80 1440,80 L1440,120 L0,120 Z"/>
        </svg>
        <svg class="wave-svg wave-svg--2" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 1440 120" preserveAspectRatio="none">
          <path fill="rgba(197,168,130,0.10)" d="M0,40 C180,80 360,10 540,50 C720,90 900,20 1080,55 C1260,90 1380,30 1440,50 L1440,120 L0,120 Z"/>
        </svg>
        <svg class="wave-svg wave-svg--3" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 1440 120" preserveAspectRatio="none">
          <path fill="rgba(26,25,23,0.04)" d="M0,70 C300,30 600,100 900,55 C1100,25 1300,75 1440,70 L1440,120 L0,120 Z"/>
        </svg>
      </div>

      <!-- Floating orbs -->
      <div class="orb orb--1" aria-hidden="true"></div>
      <div class="orb orb--2" aria-hidden="true"></div>

      <div class="dashboard__content">
        <div class="container">

          <!-- HERO -->
          <section class="hero">
            <div class="hero__text">
              <span class="hero__label">
                <span class="hero__label-dot"></span>
                Bộ sưu tập mới 2025
              </span>
              <h1 class="hero__h1">Sống chậm hơn,<br>chọn <em>đúng hơn.</em></h1>
              <p class="hero__sub">
                Những sản phẩm được lựa chọn kỹ càng — không phải để có nhiều hơn,
                mà để sống tốt hơn.
              </p>
              <div class="hero__actions">
                <button class="hero__btn-primary" (click)="navigateToProducts()">
                  Khám phá ngay
                  <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round"><path d="M5 12h14"/><path d="m12 5 7 7-7 7"/></svg>
                </button>
                <button class="hero__btn-ghost">
                  <svg xmlns="http://www.w3.org/2000/svg" width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><polygon points="5 3 19 12 5 21 5 3"/></svg>
                  Xem thêm
                </button>
              </div>
            </div>

            <div class="hero__image-wrap">
              <img class="hero__img"
                src="https://images.unsplash.com/photo-1441986300917-64674bd600d8?auto=format&fit=crop&q=80&w=1000"
                alt="Premium lifestyle shopping" loading="eager"/>
              <div class="hero__badge">
                <div class="hero__badge-icon">
                  <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M12 22s8-4 8-10V5l-8-3-8 3v7c0 6 8 10 8 10z"/></svg>
                </div>
                <div>
                  <div class="hero__badge-label">Đánh giá</div>
                  <div class="hero__badge-val">4.9 ★</div>
                </div>
              </div>
            </div>
          </section>

          <!-- STATS -->
          <div class="stats-row">
            <div class="stat-item">
              <span class="stat-item__value">12K+</span>
              <span class="stat-item__label">Sản phẩm cao cấp</span>
            </div>
            <div class="stat-item">
              <span class="stat-item__value">98%</span>
              <span class="stat-item__label">Khách hàng hài lòng</span>
            </div>
            <div class="stat-item">
              <span class="stat-item__value">2-day</span>
              <span class="stat-item__label">Giao hàng nhanh</span>
            </div>
          </div>

          <!-- CATEGORIES EDITORIAL -->
          <section>
            <div class="section-header">
              <p class="section-eyebrow">Danh mục nổi bật</p>
              <h2 class="section-title">Lựa chọn tinh tế,<br>chất lượng vượt trội.</h2>
              <p class="section-sub">Khám phá các dòng sản phẩm được đánh giá cao nhất.</p>
            </div>

            <div class="editorial-items">
              <!-- Item 1 -->
              <div class="editorial-item" #editorialItem>
                <div class="editorial-item__img-wrap">
                  <img class="editorial-item__img"
                    src="https://images.unsplash.com/photo-1505740420928-5e560c06d30e?auto=format&fit=crop&q=80&w=900"
                    alt="Âm thanh cao cấp" loading="lazy"/>
                </div>
                <div class="editorial-item__text">
                  <p class="editorial-item__num">01 — Acoustic</p>
                  <h3 class="editorial-item__title">Âm thanh Hi-Fi tinh tế</h3>
                  <p class="editorial-item__desc">
                    Bộ sưu tập âm thanh được thiết kế cho những người hiểu nhạc.
                    Trải nghiệm âm thanh ở dạng thuần khiết nhất.
                  </p>
                  <button class="editorial-item__link" (click)="navigateToProducts()">
                    Khám phá bộ sưu tập
                    <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round"><path d="M5 12h14"/><path d="m12 5 7 7-7 7"/></svg>
                  </button>
                </div>
              </div>

              <!-- Item 2 -->
              <div class="editorial-item editorial-item--reverse" #editorialItem>
                <div class="editorial-item__img-wrap">
                  <img class="editorial-item__img"
                    src="https://images.unsplash.com/photo-1523275335684-37898b6baf30?auto=format&fit=crop&q=80&w=900"
                    alt="Đồng hồ cao cấp" loading="lazy"/>
                </div>
                <div class="editorial-item__text">
                  <p class="editorial-item__num">02 — Timepieces</p>
                  <h3 class="editorial-item__title">Đồng hồ — Nghệ thuật của thời gian</h3>
                  <p class="editorial-item__desc">
                    Mỗi chiếc đồng hồ là một câu chuyện được kể qua cơ khí.
                    Được chế tác để trường tồn qua thế hệ.
                  </p>
                  <button class="editorial-item__link" (click)="navigateToProducts()">
                    Xem đồng hồ
                    <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round"><path d="M5 12h14"/><path d="m12 5 7 7-7 7"/></svg>
                  </button>
                </div>
              </div>

              <!-- Item 3 -->
              <div class="editorial-item" #editorialItem>
                <div class="editorial-item__img-wrap">
                  <img class="editorial-item__img"
                    src="https://images.unsplash.com/photo-1491553895911-0055eca6402d?auto=format&fit=crop&q=80&w=900"
                    alt="Phong cách sống" loading="lazy"/>
                </div>
                <div class="editorial-item__text">
                  <p class="editorial-item__num">03 — Lifestyle</p>
                  <h3 class="editorial-item__title">Phong cách sống — Định nghĩa lại</h3>
                  <p class="editorial-item__desc">
                    Những sản phẩm không chỉ đẹp mà còn có lý do tồn tại.
                    Thiết kế với mục đích, không phải để gây ấn tượng.
                  </p>
                  <button class="editorial-item__link" (click)="navigateToProducts()">
                    Khám phá lifestyle
                    <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round"><path d="M5 12h14"/><path d="m12 5 7 7-7 7"/></svg>
                  </button>
                </div>
              </div>
            </div>
          </section>

          <!-- FOOTER -->
          <footer class="dash-footer">
            <span class="dash-footer__copy">© 2025 ATELIER eShop — Crafted with intention.</span>
            <a href="/account/login" style="font-size: 13px; color: #9D9890; text-decoration: none;">Đăng nhập</a>
          </footer>

        </div>
      </div>
    </div>
  `
})
export class DashboardComponent implements AfterViewInit {
  @ViewChildren('editorialItem') editorialItems!: QueryList<ElementRef>;

  navigateToProducts() {
    window.location.href = '/product';
  }

  ngAfterViewInit(): void {
    // Scroll-reveal with IntersectionObserver
    const observer = new IntersectionObserver(
      (entries) => {
        entries.forEach(entry => {
          if (entry.isIntersecting) {
            entry.target.classList.add('is-visible');
            observer.unobserve(entry.target);
          }
        });
      },
      { threshold: 0.15, rootMargin: '0px 0px -60px 0px' }
    );

    this.editorialItems.forEach(item => {
      observer.observe(item.nativeElement);
    });
  }
}
