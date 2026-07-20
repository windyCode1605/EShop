import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_ENDPOINTS } from '../constants/api-endpoints.const';

export interface UserProfile {
  id: number;
  userId: number;
  fullName: string | null;
  phoneNumber: string;
  dateOfBirth: string | null;
  gender: number | null;
  avatarUrl: string | null;
}

export interface UpdateProfileDto {
  fullName: string | null;
  phoneNumber: string | null;
  dateOfBirth: string | null;
  gender: number | null;
  avatarUrl: string | null;
}

@Injectable({
  providedIn: 'root'
})
export class ProfileService {
  private http = inject(HttpClient);
  private endpoint = API_ENDPOINTS.CUSTOMER.ME;

  getMyProfile(): Observable<any> {
    return this.http.get<any>(this.endpoint);
  }

  updateMyProfile(dto: UpdateProfileDto): Observable<any> {
    return this.http.patch<any>(this.endpoint, dto);
  }
}
