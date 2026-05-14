import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-footer',
  standalone: true,
  imports: [CommonModule],
  template: `
    <footer class="footer">
      <div class="footer__content page-shell">
        <div class="footer__section">
          <h3 class="footer__title">Về chúng tôi</h3>
          <ul class="footer__links">
            <li><a href="#about">Giới thiệu</a></li>
            <li><a href="#story">Câu chuyện thương hiệu</a></li>
            <li><a href="#team">Đội ngũ</a></li>
            <li><a href="#careers">Tuyển dụng</a></li>
          </ul>
        </div>

        <div class="footer__section">
          <h3 class="footer__title">Hỗ trợ khách hàng</h3>
          <ul class="footer__links">
            <li><a href="#contact">Liên hệ</a></li>
            <li><a href="#faq">Câu hỏi thường gặp</a></li>
            <li><a href="#shipping">Vận chuyển</a></li>
            <li><a href="#returns">Chính sách trả hàng</a></li>
          </ul>
        </div>

        <div class="footer__section">
          <h3 class="footer__title">Chính sách</h3>
          <ul class="footer__links">
            <li><a href="#privacy">Chính sách bảo mật</a></li>
            <li><a href="#terms">Điều khoản sử dụng</a></li>
            <li><a href="#cookies">Chính sách Cookie</a></li>
            <li><a href="#terms">Điều kiện bán hàng</a></li>
          </ul>
        </div>

        <div class="footer__section">
          <h3 class="footer__title">Kết nối</h3>
          <ul class="footer__links">
            <li><a href="#facebook" target="_blank">Facebook</a></li>
            <li><a href="#instagram" target="_blank">Instagram</a></li>
            <li><a href="#twitter" target="_blank">Twitter</a></li>
            <li><a href="#linkedin" target="_blank">LinkedIn</a></li>
          </ul>
        </div>
      </div>

      <div class="footer__divider"></div>

      <div class="footer__bottom page-shell">
        <div class="footer__copyright">
          <p>&copy; 2024 eShop Fashion. Tất cả các quyền được bảo lưu.</p>
        </div>
        <div class="footer__payment">
          <span class="footer__label">Thanh toán:</span>
          <span class="footer__methods">💳 Thẻ ngân hàng • 🏦 Chuyển khoản • 📱 E-wallet</span>
        </div>
      </div>
    </footer>
  `,
  styles: [`
    .footer {
      background: linear-gradient(180deg, rgba(255, 255, 255, 0.98), rgba(250, 250, 249, 0.96));
      border-top: 1px solid var(--color-border);
      color: var(--color-text);
      padding: var(--spacing-2xl) 0 var(--spacing-xl);
    }

    .footer__content {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
      gap: var(--spacing-xl);
      margin-bottom: var(--spacing-2xl);
    }

    .footer__section {
      display: flex;
      flex-direction: column;
      gap: var(--spacing-md);
    }

    .footer__title {
      font-size: 1rem;
      font-weight: 700;
      color: var(--color-accent);
      margin: 0;
      text-transform: uppercase;
      letter-spacing: 1px;
      border-bottom: 2px solid var(--color-primary);
      padding-bottom: var(--spacing-sm);
    }

    .footer__links {
      list-style: none;
      margin: 0;
      padding: 0;
      display: flex;
      flex-direction: column;
      gap: var(--spacing-sm);
    }

    .footer__links a {
      color: var(--color-text-secondary);
      text-decoration: none;
      font-size: 0.9rem;
      transition: color 0.3s ease;
      padding-bottom: 2px;
      border-bottom: 1px solid transparent;
    }

    .footer__links a:hover {
      color: var(--color-primary);
      border-bottom-color: var(--color-primary);
    }

    .footer__divider {
      height: 1px;
      background: linear-gradient(90deg, transparent, var(--color-border), transparent);
      margin: var(--spacing-xl) 0;
    }

    .footer__bottom {
      display: flex;
      justify-content: space-between;
      align-items: center;
      flex-wrap: wrap;
      gap: var(--spacing-lg);
      font-size: 0.875rem;
      color: var(--color-text-secondary);
    }

    .footer__copyright p {
      margin: 0;
      color: var(--color-text-secondary);
    }

    .footer__payment {
      display: flex;
      align-items: center;
      gap: var(--spacing-md);
    }

    .footer__label {
      font-weight: 600;
      color: var(--color-text);
    }

    .footer__methods {
      color: var(--color-text-secondary);
    }

    @media (max-width: 768px) {
      .footer {
        padding: var(--spacing-xl) 0 var(--spacing-md);
      }

      .footer__content {
        grid-template-columns: repeat(2, 1fr);
        gap: var(--spacing-lg);
        margin-bottom: var(--spacing-lg);
      }

      .footer__bottom {
        flex-direction: column;
        align-items: flex-start;
        font-size: 0.8rem;
      }
    }

    @media (max-width: 576px) {
      .footer__content {
        grid-template-columns: 1fr;
        gap: var(--spacing-md);
      }

      .footer__title {
        font-size: 0.875rem;
      }
    }
  `]
})
export class FooterComponent { }
