import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-account-register-page',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})
export class RegisterPageComponent {
  submitting = false;
  step: 'register' | 'verify' | 'set-password' = 'register';
  registeredUserId: number | null = null;
  errorMessage = '';
  successMessage = '';

  readonly registerForm = this.fb.group({
    displayName: ['', [Validators.required]],
    userName: ['', [Validators.required, Validators.minLength(3)]],
    email: ['', [Validators.required, Validators.email]],
    userCode: ['']
  });

  readonly verifyForm = this.fb.group({
    otpCode: ['', [Validators.required, Validators.pattern(/^[a-zA-Z0-9]{6}$/)]]
  });

  readonly setPasswordForm = this.fb.group({
    password: ['', [Validators.required, Validators.minLength(6)]],
    confirmPassword: ['', [Validators.required]]
  });

  constructor(
    private readonly fb: FormBuilder,
    private readonly authService: AuthService,
    private readonly router: Router
  ) { }

  submitRegister(): void {
    if (this.registerForm.invalid || this.submitting) {
      return;
    }

    this.submitting = true;
    this.errorMessage = '';
    this.successMessage = '';

    this.authService
      .register({
        fullName: this.registerForm.controls.displayName.value?.trim() ?? '',
        userName: this.registerForm.controls.userName.value?.trim() ?? '',
        email: this.registerForm.controls.email.value?.trim() ?? '',
        userCode: this.registerForm.controls.userCode.value?.trim() ?? ''
      })
      .subscribe({
        next: (response) => {
          this.submitting = false;
          this.registeredUserId = response.id;
          this.step = 'verify';
          this.successMessage = 'Đăng ký thành công. Vui lòng nhập OTP đã gửi về email.';
        },
        error: (error: HttpErrorResponse) => {
          this.submitting = false;
          this.errorMessage = this.extractApiError(error, 'Không thể đăng ký tài khoản.');
        }
      });
  }

  submitVerifyOtp(): void {
    if (this.verifyForm.invalid || this.submitting) {
      return;
    }

    this.submitting = true;
    this.errorMessage = '';
    this.successMessage = '';

    this.authService
      .verifyRegisterOtp({
        email: this.registerForm.controls.email.value?.trim() ?? '',
        otpCode: this.verifyForm.controls.otpCode.value?.trim().toUpperCase() ?? ''
      })
      .subscribe({
        next: () => {
          this.submitting = false;
          this.successMessage = 'Xác thực OTP thành công. Vui lòng cài đặt mật khẩu.';
          this.step = 'set-password';
        },
        error: (error: HttpErrorResponse) => {
          this.submitting = false;
          this.errorMessage = this.extractApiError(error, 'OTP không hợp lệ hoặc đã hết hạn.');
        }
      });
  }

  submitSetPassword(): void {
    if (this.setPasswordForm.invalid || this.submitting || !this.registeredUserId) {
      return;
    }

    const password = this.setPasswordForm.controls.password.value ?? '';
    const confirmPassword = this.setPasswordForm.controls.confirmPassword.value ?? '';

    if (password !== confirmPassword) {
      this.errorMessage = 'Mật khẩu xác nhận không khớp.';
      return;
    }

    this.submitting = true;
    this.errorMessage = '';
    this.successMessage = '';

    this.authService
      .setPassword({
        id: this.registeredUserId,
        password: password,
        isPasswordTemp: false
      })
      .subscribe({
        next: () => {
          this.submitting = false;
          this.successMessage = 'Cài đặt mật khẩu thành công. Mời bạn đăng nhập.';
          setTimeout(() => this.router.navigate(['/account/login']), 1500);
        },
        error: (error: HttpErrorResponse) => {
          this.submitting = false;
          this.errorMessage = this.extractApiError(error, 'Không thể cài đặt mật khẩu.');
        }
      });
  }

  backToRegister(): void {
    this.step = 'register';
    this.errorMessage = '';
    this.successMessage = '';
  }

  private extractApiError(error: any, fallback: string): string {
    if (error instanceof Error) {
      return error.message;
    }
    const payload = error?.error as { errors?: string[]; message?: string } | undefined;
    if (payload?.errors?.length) {
      return payload.errors[0];
    }
    if (payload?.message) {
      return payload.message;
    }
    return fallback;
  }
}
