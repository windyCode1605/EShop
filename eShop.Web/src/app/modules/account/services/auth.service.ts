import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map, Observable } from 'rxjs';
import { UserDto } from '../../../core/models';
import { environment } from '../../../my-lib/shared/enviroments/enviroment';
import {
  RegisterApiResponse,
  RegisterRequest,
  VerifyRegisterOtpApiResponse,
  VerifyRegisterOtpRequest
} from '../models/auth.model';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly apiUrl = environment.api;

  constructor(private readonly http: HttpClient) {}

  register(payload: RegisterRequest): Observable<UserDto> {
    return this.http
      .post<RegisterApiResponse>(`${this.apiUrl}/register`, payload)
      .pipe(map((response) => this.unwrap(response)));
  }

  verifyRegisterOtp(payload: VerifyRegisterOtpRequest): Observable<string> {
    return this.http
      .post<VerifyRegisterOtpApiResponse>(`${this.apiUrl}/register/verify-otp`, payload)
      .pipe(map((response) => this.unwrap(response)));
  }

  private unwrap<T>(response: { data?: T; message?: string }): T {
    if (response.data === undefined || response.data === null) {
      throw new Error(response.message || 'API response is empty.');
    }
    return response.data;
  }
}
