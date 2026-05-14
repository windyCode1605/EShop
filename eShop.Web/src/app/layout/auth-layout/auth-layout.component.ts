import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FooterComponent } from '../components/footer.component';

@Component({
  selector: 'app-auth-layout',
  standalone: true,
  imports: [CommonModule, RouterModule, FooterComponent],
  template: `
    <div class="auth-layout">
      <main class="auth-layout__content">
        <router-outlet></router-outlet>
      </main>
      <footer class="auth-layout__footer">
        <app-footer></app-footer>
      </footer>
    </div>
  `,
  styles: [`
    .auth-layout {
      display: flex;
      flex-direction: column;
      min-height: 100vh;
      background: linear-gradient(135deg, #FAFAF9 0%, #F5F5F4 100%);
    }

    .auth-layout__content {
      flex: 1;
      display: flex;
      align-items: center;
      justify-content: center;
      padding: var(--spacing-xl);
    }

    .auth-layout__footer {
      padding: var(--spacing-xl) var(--spacing-lg);
      border-top: 1px solid var(--color-border);
      background-color: var(--color-surface);
    }
  `]
})
export class AuthLayoutComponent { }
