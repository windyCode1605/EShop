import { Component, OnDestroy, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormArray, FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-forgot-password',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './forgot-password.component.html'
})
export class ForgotPasswordComponent implements OnInit, OnDestroy {
  currentStep: 1 | 2 | 3 = 1;
  loading = false;
  errorMessage = '';

  userEmail = '';
  resetToken = '';

  step1Form!: FormGroup;
  otpForm!: FormGroup;
  step3Form!: FormGroup;

  resendCountdown = 0;
  private timerInterval: any;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.step1Form = this.fb.group({
      email: ['', [Validators.required, Validators.email]]
    });

    this.otpForm = this.fb.group({
      digits: this.fb.array([
        ['', Validators.required],
        ['', Validators.required],
        ['', Validators.required],
        ['', Validators.required],
        ['', Validators.required],
        ['', Validators.required]
      ])
    });

    this.step3Form = this.fb.group({
      newPassword: ['', [Validators.required, Validators.minLength(6)]],
      confirmPassword: ['', Validators.required]
    }, { validators: this.passwordMatchValidator });
  }

  ngOnDestroy(): void {
    if (this.timerInterval) {
      clearInterval(this.timerInterval);
    }
    if (this.redirectInterval) {
      clearInterval(this.redirectInterval);
    }
  }

  get otpArray(): FormArray {
    return this.otpForm.get('digits') as FormArray;
  }

  get otpControls() {
    return this.otpArray.controls;
  }

  get isOtpComplete(): boolean {
    const values = this.otpArray.value;
    return Array.isArray(values) && values.length === 6 && values.every(v => typeof v === 'string' && v.trim().length === 1);
  }

  isFieldInvalid(form: FormGroup, field: string): boolean {
    const control = form.get(field);
    return !!(control && control.invalid && (control.dirty || control.touched));
  }

  private passwordMatchValidator(group: FormGroup) {
    const newPass = group.get('newPassword')?.value;
    const confirmPass = group.get('confirmPassword')?.value;
    return newPass === confirmPass ? null : { mismatch: true };
  }

  // STEP 1: Request OTP
  onRequestOtp(): void {
    if (this.step1Form.invalid) {
      this.step1Form.markAllAsTouched();
      return;
    }

    this.loading = true;
    this.errorMessage = '';
    this.userEmail = this.step1Form.value.email;

    this.authService.forgotPassword({ email: this.userEmail }).subscribe({
      next: () => {
        this.loading = false;
        this.currentStep = 2;
        this.startResendTimer();
        setTimeout(() => {
          document.getElementById('otp-0')?.focus();
        }, 100);
      },
      error: (err) => {
        this.loading = false;
        this.errorMessage = err?.message || err?.error?.message || err?.error?.Message || 'Không thể gửi yêu cầu. Vui lòng thử lại.';
      }
    });
  }

  // STEP 2: Verify OTP
  onVerifyOtp(): void {
    if (!this.isOtpComplete) return;

    this.loading = true;
    this.errorMessage = '';
    const otpCode = this.otpArray.value.join('');

    this.authService.verifyResetOtp({ email: this.userEmail, otpCode }).subscribe({
      next: (res: any) => {
        this.loading = false;
        this.resetToken = res?.resetToken || res?.ResetToken || '';
        this.currentStep = 3;
      },
      error: (err) => {
        this.loading = false;
        this.errorMessage = err?.message || err?.error?.message || err?.error?.Message || 'Mã OTP không đúng hoặc đã hết hạn.';
      }
    });
  }

  onResendOtp(): void {
    if (this.resendCountdown > 0 || this.loading) return;

    this.loading = true;
    this.errorMessage = '';

    this.authService.forgotPassword({ email: this.userEmail }).subscribe({
      next: () => {
        this.loading = false;
        this.startResendTimer();
      },
      error: (err) => {
        this.loading = false;
        this.errorMessage = err?.message || err?.error?.message || err?.error?.Message || 'Không thể gửi lại mã OTP.';
      }
    });
  }

  onOtpInput(event: any, index: number): void {
    const rawVal = (event.target.value || '').trim();

    if (!rawVal) {
      this.otpArray.at(index).setValue('', { emitEvent: false });
      return;
    }

    // Luôn chỉ lấy 1 ký tự mới nhất vừa nhập vào ô này
    const char = rawVal.slice(-1).toUpperCase();
    this.otpArray.at(index).setValue(char, { emitEvent: false });
    event.target.value = char;

    // Tự động chuyển tiêu điểm sang ô tiếp theo
    if (index < 5) {
      const nextInput = document.getElementById(`otp-${index + 1}`) as HTMLInputElement;
      nextInput?.focus();
      nextInput?.select();
    }

    // Tự động xác thực khi đã điền đủ 6 ô
    if (this.isOtpComplete && !this.loading) {
      this.onVerifyOtp();
    }
  }

  onOtpKeyDown(event: KeyboardEvent, index: number): void {
    if (event.key === 'Backspace') {
      const currentVal = this.otpArray.at(index).value;
      if (!currentVal && index > 0) {
        // Nếu ô hiện tại trống, lùi về ô trước và xóa ô trước
        this.otpArray.at(index - 1).setValue('', { emitEvent: false });
        const prevInput = document.getElementById(`otp-${index - 1}`) as HTMLInputElement;
        if (prevInput) {
          prevInput.value = '';
          prevInput.focus();
        }
      } else {
        // Xóa ô hiện tại
        this.otpArray.at(index).setValue('', { emitEvent: false });
        (event.target as HTMLInputElement).value = '';
      }
    } else if (event.key === 'ArrowLeft' && index > 0) {
      const prevInput = document.getElementById(`otp-${index - 1}`) as HTMLInputElement;
      prevInput?.focus();
    } else if (event.key === 'ArrowRight' && index < 5) {
      const nextInput = document.getElementById(`otp-${index + 1}`) as HTMLInputElement;
      nextInput?.focus();
    }
  }

  onOtpPaste(event: ClipboardEvent): void {
    event.preventDefault();
    const clipboardData = event.clipboardData?.getData('text') || '';
    const cleanedData = clipboardData.trim().toUpperCase().replace(/\s+/g, '').slice(0, 6);

    if (cleanedData) {
      for (let i = 0; i < 6; i++) {
        const char = cleanedData[i] || '';
        this.otpArray.at(i).setValue(char, { emitEvent: false });
        const inputElem = document.getElementById(`otp-${i}`) as HTMLInputElement;
        if (inputElem) {
          inputElem.value = char;
        }
      }
      
      const lastIndex = Math.min(cleanedData.length, 5);
      const targetInput = document.getElementById(`otp-${lastIndex}`) as HTMLInputElement;
      targetInput?.focus();

      if (this.isOtpComplete && !this.loading) {
        this.onVerifyOtp();
      }
    }
  }

  private startResendTimer(): void {
    this.resendCountdown = 60;
    if (this.timerInterval) clearInterval(this.timerInterval);
    this.timerInterval = setInterval(() => {
      if (this.resendCountdown > 0) {
        this.resendCountdown--;
      } else {
        clearInterval(this.timerInterval);
      }
    }, 1000);
  }

  showNewPassword = false;
  showConfirmPassword = false;
  showSuccessModal = false;
  redirectCountdown = 3;
  private redirectInterval: any;

  toggleShowNewPassword(): void {
    this.showNewPassword = !this.showNewPassword;
  }

  toggleShowConfirmPassword(): void {
    this.showConfirmPassword = !this.showConfirmPassword;
  }

  goToLoginImmediately(): void {
    if (this.redirectInterval) clearInterval(this.redirectInterval);
    this.router.navigate(['/account/login']);
  }

  // STEP 3: Reset Password
  onResetPassword(): void {
    if (this.step3Form.invalid) {
      this.step3Form.markAllAsTouched();
      return;
    }

    this.loading = true;
    this.errorMessage = '';
    const newPassword = this.step3Form.value.newPassword;

    this.authService.resetPassword({
      email: this.userEmail,
      resetToken: this.resetToken,
      newPassword
    }).subscribe({
      next: () => {
        this.loading = false;
        this.showSuccessModal = true;
        this.redirectCountdown = 3;
        if (this.redirectInterval) clearInterval(this.redirectInterval);
        
        this.redirectInterval = setInterval(() => {
          this.redirectCountdown--;
          if (this.redirectCountdown <= 0) {
            clearInterval(this.redirectInterval);
            this.router.navigate(['/account/login']);
          }
        }, 1000);
      },
      error: (err) => {
        this.loading = false;
        this.errorMessage = err?.message || err?.error?.message || err?.error?.Message || 'Đặt lại mật khẩu thất bại.';
      }
    });
  }
}
