import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { catchError, map, Observable, throwError } from 'rxjs';
import { UserDto } from '../../../core/models';
import { API_ENDPOINTS } from '../../../core/constants/api-endpoints.const';
import {
  RegisterApiResponse,
  RegisterRequest,
  VerifyRegisterOtpApiResponse,
  VerifyRegisterOtpRequest,
  SetPasswordRequest,
  SetPasswordApiResponse,
  ForgotPasswordRequest,
  ForgotPasswordApiResponse,
  VerifyResetOtpRequest,
  VerifyResetOtpApiResponse,
  VerifyResetOtpResponseData,
  ResetPasswordRequest,
  ResetPasswordApiResponse
} from '../models/auth.model';

@Injectable({ providedIn: 'root' })
export class AuthService {
  constructor(private readonly http: HttpClient) { }

  register(payload: RegisterRequest): Observable<UserDto> {
    return this.http
      .post<RegisterApiResponse>(API_ENDPOINTS.AUTH.REGISTER, payload)
      .pipe(
        map((response) => this.unwrap<UserDto>(response)),
        catchError((err) => this.handleError(err))
      );
  }

  verifyRegisterOtp(payload: VerifyRegisterOtpRequest): Observable<string> {
    return this.http
      .post<VerifyRegisterOtpApiResponse>(API_ENDPOINTS.AUTH.VERIFY_OTP, payload)
      .pipe(
        map((response) => this.unwrap<string>(response)),
        catchError((err) => this.handleError(err))
      );
  }

  setPassword(payload: SetPasswordRequest): Observable<string> {
    return this.http
      .post<SetPasswordApiResponse>(API_ENDPOINTS.AUTH.SET_PASSWORD, payload)
      .pipe(
        map((response) => this.unwrap<string>(response)),
        catchError((err) => this.handleError(err))
      );
  }

  forgotPassword(payload: ForgotPasswordRequest): Observable<any> {
    return this.http
      .post<ForgotPasswordApiResponse>(API_ENDPOINTS.AUTH.FORGOT_PASSWORD, payload)
      .pipe(
        map((response) => this.unwrap<any>(response)),
        catchError((err) => this.handleError(err))
      );
  }

  verifyResetOtp(payload: VerifyResetOtpRequest): Observable<VerifyResetOtpResponseData> {
    return this.http
      .post<VerifyResetOtpApiResponse>(API_ENDPOINTS.AUTH.VERIFY_RESET_OTP, payload)
      .pipe(
        map((response) => this.unwrap<VerifyResetOtpResponseData>(response)),
        catchError((err) => this.handleError(err))
      );
  }

  resetPassword(payload: ResetPasswordRequest): Observable<any> {
    return this.http
      .post<ResetPasswordApiResponse>(API_ENDPOINTS.AUTH.RESET_PASSWORD, payload)
      .pipe(
        map((response) => this.unwrap<any>(response)),
        catchError((err) => this.handleError(err))
      );
  }

  private unwrap<T>(response: any): T {
    if (!response) {
      throw new Error('Hệ thống không phản hồi. Vui lòng thử lại sau.');
    }
    const isSuccess = response.isSuccess !== undefined ? response.isSuccess : (response.success !== undefined ? response.success : true);
    if (isSuccess === false || response.isFailure === true) {
      const errorMessage = response.message || response.Message || response.errors?.[0] || 'Đã xảy ra lỗi. Vui lòng thử lại.';

      const error = new Error(errorMessage) as any;
      error.errorCode = response.errorCode ?? response.code;
      throw error;
    }
    // Result<T> trả về dữ liệu nằm trong trường 'value' hoặc 'Value', hoặc 'otherData', hoặc 'data'
    const payload = response.value ?? response.Value ?? response.otherData ?? response.data;
    return payload as T;
  }

  private handleError(err: any): Observable<never> {
    if (err instanceof HttpErrorResponse && err.error) {
      const backendMsg = err.error.message || err.error.Message || err.error.errors?.[0];
      if (backendMsg) {
        const error = new Error(backendMsg) as any;
        error.errorCode = err.error.errorCode ?? err.error.code;
        return throwError(() => error);
      }
    }
    return throwError(() => err);
  }
}
