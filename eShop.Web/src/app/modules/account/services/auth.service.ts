import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map, Observable } from 'rxjs';
import { UserDto } from '../../../core/models';
import { API_ENDPOINTS } from '../../../core/constants/api-endpoints.const';
import {
  RegisterApiResponse,
  RegisterRequest,
  VerifyRegisterOtpApiResponse,
  VerifyRegisterOtpRequest,
  SetPasswordRequest,
  SetPasswordApiResponse
} from '../models/auth.model';

@Injectable({ providedIn: 'root' })
export class AuthService {
  constructor(private readonly http: HttpClient) { }

  register(payload: RegisterRequest): Observable<UserDto> {
    return this.http
      .post<RegisterApiResponse>(API_ENDPOINTS.AUTH.REGISTER, payload)
      .pipe(map((response) => this.unwrap(response)));
  }

  verifyRegisterOtp(payload: VerifyRegisterOtpRequest): Observable<string> {
    return this.http
      .post<VerifyRegisterOtpApiResponse>(API_ENDPOINTS.AUTH.VERIFY_OTP, payload)
      .pipe(map((response) => this.unwrap(response)));
  }

  setPassword(payload: SetPasswordRequest): Observable<string> {
    return this.http
      .post<SetPasswordApiResponse>(API_ENDPOINTS.AUTH.SET_PASSWORD, payload)
      .pipe(map((response) => this.unwrap(response)));
  }

  private unwrap<T>(response: { data?: T; message?: string }): T {
    if (response.data === undefined || response.data === null) {
      throw new Error(response.message || 'API response is empty.');
    }
    return response.data;
  }
}
