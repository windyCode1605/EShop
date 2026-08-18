import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ThemeToggleComponent } from '../../../shared/components/theme-toggle/theme-toggle.component';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, ThemeToggleComponent],
  template: `
    <div class="min-h-screen w-full flex bg-canvas text-primary transition-colors duration-300">
      
      <!-- Theme Toggle Absolute Position -->
      <div class="absolute top-8 right-8 z-50">
        <app-theme-toggle></app-theme-toggle>
      </div>

      <!-- Left Side: Visual / Branding (Optional for Split Layout) -->
      <div class="hidden lg:flex w-1/2 flex-col justify-between bg-surface border-r border-border p-12 transition-colors duration-300">
        <div>
          <h1 class="text-h2 font-serif text-accent">EShop Premium</h1>
        </div>
        
        <div class="flex-1 flex flex-col justify-center max-w-md mx-auto relative">
          <div class="relative rounded-2xl overflow-hidden aspect-square border border-border shadow-soft bg-canvas flex items-center justify-center p-8 transition-colors duration-300">
            <img src="Cu_Do_38.jpg" alt="Logo Cu Đơ 38" class="w-full h-full object-cover rounded-xl opacity-90 mix-blend-multiply dark:mix-blend-screen" />
          </div>
          <div class="mt-12 space-y-4">
            <h2 class="text-display-sans">Mai Quang Nguyên</h2>
            <p class="text-body text-secondary border-l-2 border-accent pl-4">
              Bình tĩnh - Bản lĩnh - Hà Tĩnh | Cu Đơ 38
            </p>
          </div>
        </div>
        
        <div class="text-caption text-secondary">
          © 2026 M.Q.Nguyen IT Portfolio. All rights reserved.
        </div>
      </div>

      <!-- Right Side: Login Form -->
      <div class="w-full lg:w-1/2 flex items-center justify-center p-8 lg:p-24 relative">
        <div class="w-full max-w-md space-y-12">
          
          <div class="space-y-3">
            <h1 class="text-h1">Chào mừng quay lại.</h1>
            <p class="text-body text-secondary">Vui lòng đăng nhập vào hệ thống để tiếp tục.</p>
          </div>

          <form [formGroup]="loginForm" (ngSubmit)="onSubmit()" class="space-y-6">
            
            <div class="space-y-1">
              <label class="text-meta">Tài khoản</label>
              <input 
                type="text" 
                formControlName="username"
                class="app-input-boxed bg-surface text-primary border-border focus:border-accent"
                placeholder="Nhập tên đăng nhập"
              />
            </div>

            <div class="space-y-1">
              <label class="text-meta">Mật khẩu</label>
              <input 
                type="password" 
                formControlName="password"
                class="app-input-boxed bg-surface text-primary border-border focus:border-accent"
                placeholder="Nhập mật khẩu"
              />
            </div>

            <div class="flex items-center justify-between">
              <label class="flex items-center gap-2 cursor-pointer">
                <input type="checkbox" class="w-4 h-4 rounded border-border text-accent focus:ring-accent" />
                <span class="text-caption">Ghi nhớ đăng nhập</span>
              </label>
              <a href="#" class="text-caption text-accent hover:underline">Quên mật khẩu?</a>
            </div>

            <button 
              type="submit" 
              [disabled]="loginForm.invalid || isLoading"
              class="w-full app-button-accent relative overflow-hidden"
            >
              <span [class.opacity-0]="isLoading">Đăng nhập</span>
              <div *ngIf="isLoading" class="absolute inset-0 flex items-center justify-center">
                <div class="spinner-ring"></div>
              </div>
            </button>
          </form>

        </div>
      </div>

    </div>
  `,
  styles: []
})
export class LoginComponent {
  private fb = inject(FormBuilder);
  
  isLoading = false;

  loginForm = this.fb.nonNullable.group({
    username: ['', Validators.required],
    password: ['', Validators.required]
  });

  onSubmit() {
    if (this.loginForm.valid) {
      this.isLoading = true;
      // Mock API call
      setTimeout(() => {
        this.isLoading = false;
        alert('Đăng nhập thành công với giao diện Premium!');
      }, 1500);
    }
  }
}
