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
  step: 'register' | 'verify' = 'register';
  errorMessage = '';
  successMessage = '';

  readonly registerForm = this.fb.group({
    displayName: ['', [Validators.required]],
    email: ['', [Validators.required, Validators.email]],
    userCode: ['']
  });

  readonly verifyForm = this.fb.group({
    otpCode: ['', [Validators.required, Validators.pattern(/^\d{6}$/)]]
  });

  constructor(
    private readonly fb: FormBuilder,
    private readonly authService: AuthService,
    private readonly router: Router
  ) {}

  submitRegister(): void {
    if (this.registerForm.invalid || this.submitting) {
      return;
    }

    this.submitting = true;
    this.errorMessage = '';
    this.successMessage = '';

    this.authService
      .register({
        userName: this.registerForm.controls.displayName.value?.trim() ?? '',
        email: this.registerForm.controls.email.value?.trim() ?? '',
        userCode: this.registerForm.controls.userCode.value?.trim() ?? ''
      })
      .subscribe({
        next: () => {
          this.submitting = false;
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
        otpCode: this.verifyForm.controls.otpCode.value?.trim() ?? ''
      })
      .subscribe({
        next: () => {
          this.submitting = false;
          this.successMessage = 'Xác thực OTP thành công. Mời bạn đăng nhập.';
          setTimeout(() => this.router.navigate(['/account/login']), 1000);
        },
        error: (error: HttpErrorResponse) => {
          this.submitting = false;
          this.errorMessage = this.extractApiError(error, 'OTP không hợp lệ hoặc đã hết hạn.');
        }
      });
  }

  backToRegister(): void {
    this.step = 'register';
    this.errorMessage = '';
    this.successMessage = '';
  }

  private extractApiError(error: HttpErrorResponse, fallback: string): string {
    const payload = error.error as { errors?: string[]; message?: string } | undefined;
    if (payload?.errors?.length) {
      return payload.errors[0];
    }
    if (payload?.message) {
      return payload.message;
    }
    return fallback;
  }
}
