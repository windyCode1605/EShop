import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-footer',
  standalone: true,
  imports: [CommonModule, RouterModule],
  styles: [`
    .footer {
      background-color: var(--color-canvas);
      border-top: 1px solid var(--color-border);
      padding: var(--spacing-64) 0 var(--spacing-32);
      color: var(--color-text-primary);
    }

    .footer__top {
      display: flex;
      justify-content: space-between;
      align-items: flex-start;
      margin-bottom: var(--spacing-64);
      gap: var(--spacing-32);
      flex-wrap: wrap;
    }

    .footer__brand h2 {
      font-size: 48px;
      font-weight: 700;
      letter-spacing: -1.5px;
      margin-bottom: var(--spacing-16);
      color: var(--color-text-primary);
    }

    .footer__brand p {
      color: var(--color-text-secondary);
      font-size: 16px;
      max-width: 300px;
      line-height: 1.6;
      margin: 0;
    }

    .footer__newsletter {
      display: flex;
      flex-direction: column;
      gap: var(--spacing-16);
      width: 100%;
      max-width: 400px;
    }

    .footer__newsletter h3 {
      font-size: 18px;
      font-weight: 500;
      margin: 0;
      color: var(--color-text-primary);
    }

    .newsletter-input {
      display: flex;
      gap: var(--spacing-8);
    }

    .newsletter-input .app-input {
      flex: 1;
      padding: 12px 16px;
    }

    .footer__content {
      display: grid;
      grid-template-columns: repeat(4, 1fr);
      gap: var(--spacing-32);
      margin-bottom: var(--spacing-64);
    }

    .footer__title {
      font-size: 12px;
      font-weight: 600;
      color: var(--color-text-primary);
      margin-bottom: var(--spacing-24);
      text-transform: uppercase;
      letter-spacing: 1px;
    }

    .footer__links {
      list-style: none;
      padding: 0;
      margin: 0;
      display: flex;
      flex-direction: column;
      gap: var(--spacing-12);
    }

    .footer__links a {
      color: var(--color-text-secondary);
      font-size: 15px;
      text-decoration: none;
      transition: color 0.2s ease;
    }

    .footer__links a:hover {
      color: var(--color-accent);
    }

    .footer__bottom {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding-top: var(--spacing-32);
      border-top: 1px solid var(--color-border);
      flex-wrap: wrap;
      gap: var(--spacing-16);
    }

    .footer__copyright p {
      margin: 0;
      color: var(--color-text-secondary);
      font-size: 14px;
    }

    .footer__socials {
      display: flex;
      gap: var(--spacing-24);
    }

    .footer__socials a {
      color: var(--color-text-secondary);
      font-size: 12px;
      font-weight: 500;
      text-transform: uppercase;
      letter-spacing: 0.5px;
      text-decoration: none;
      transition: color 0.2s ease;
    }

    .footer__socials a:hover {
      color: var(--color-accent);
    }

    @media (max-width: 1024px) {
      .footer__content {
        grid-template-columns: repeat(2, 1fr);
      }
    }

    @media (max-width: 768px) {
      .footer__top {
        flex-direction: column;
        gap: var(--spacing-32);
      }
      .footer__newsletter {
        max-width: 100%;
      }
    }

    @media (max-width: 480px) {
      .footer__content {
        grid-template-columns: 1fr;
      }
      .footer__bottom {
        flex-direction: column;
        align-items: flex-start;
        gap: var(--spacing-12);
      }
      .newsletter-input {
        flex-direction: column;
      }
    }
  `],
  template: `
    <footer class="footer">
      <div class="page-shell">

        <div class="footer__top">
          <div class="footer__brand">
            <h2>ESHOP.</h2>
            <p>Trang phục thường ngày được tuyển chọn kỹ lưỡng, mang lại sự tinh tế và thoải mái trong từng khoảnh khắc.</p>
          </div>

        </div>

        <div class="footer__content">
          <div class="footer__section">
            <h3 class="footer__title">Cửa hàng</h3>
            <ul class="footer__links">
              <li><a href="#new">Hàng mới về</a></li>
              <li><a href="#bestsellers">Bán chạy nhất</a></li>
              <li><a href="#sale">Khuyến mãi</a></li>
              <li><a href="#collections">Bộ sưu tập</a></li>
            </ul>
          </div>

          <div class="footer__section">
            <h3 class="footer__title">Về chúng tôi</h3>
            <ul class="footer__links">
              <li><a href="#story">Câu chuyện thương hiệu</a></li>
              <li><a href="#stores">Hệ thống cửa hàng</a></li>
              <li><a href="#careers">Tuyển dụng</a></li>
              <li><a href="#press">Truyền thông</a></li>
            </ul>
          </div>

          <div class="footer__section">
            <h3 class="footer__title">Hỗ trợ</h3>
            <ul class="footer__links">
              <li><a href="#contact">Liên hệ</a></li>
              <li><a href="#faq">Câu hỏi thường gặp</a></li>
              <li><a href="#shipping">Vận chuyển & Đổi trả</a></li>
              <li><a href="#size">Hướng dẫn chọn size</a></li>
            </ul>
          </div>

          <div class="footer__section">
            <h3 class="footer__title">Pháp lý</h3>
            <ul class="footer__links">
              <li><a href="#privacy">Bảo mật thông tin</a></li>
              <li><a href="#terms">Điều khoản dịch vụ</a></li>
              <li><a href="#cookies">Chính sách Cookie</a></li>
            </ul>
          </div>
        </div>

        <div class="footer__bottom">
          <div class="footer__copyright">
            <p>&copy; 2024 ESHOP. Tất cả các quyền được bảo lưu.</p>
          </div>
          <div class="footer__socials">
            <a href="#instagram" target="_blank">Instagram</a>
            <a href="#twitter" target="_blank">Twitter</a>
            <a href="#pinterest" target="_blank">Pinterest</a>
          </div>
        </div>

      </div>
    </footer>
  `
})
export class FooterComponent { }
