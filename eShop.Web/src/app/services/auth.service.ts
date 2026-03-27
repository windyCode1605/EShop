import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { environment } from '../../environments/environment';
import { AuthResponse, LoginRequest, RegisterRequest, UserDto } from '../models';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = `${environment.apiUrl}/auth`;
  private currentUserSubject: BehaviorSubject<UserDto | null>;
  public currentUser$: Observable<UserDto | null>;
  private tokenKey = 'auth_token';

  constructor(private http: HttpClient) {
    this.currentUserSubject = new BehaviorSubject<UserDto | null>(this.getUserFromStorage());
    this.currentUser$ = this.currentUserSubject.asObservable();
  }

  public get currentUserValue(): UserDto | null {
    return this.currentUserSubject.value;
  }

  public get token(): string | null {
    return localStorage.getItem(this.tokenKey);
  }

  public get isLoggedIn(): boolean {
    return !!this.token;
  }

  login(request: LoginRequest): Observable<AuthResponse> {
    return new Observable(observer => {
      this.http.post<AuthResponse>(`${this.apiUrl}/login`, request).subscribe({
        next: (response) => {
          if (response.success && response.token && response.user) {
            localStorage.setItem(this.tokenKey, response.token);
            this.currentUserSubject.next(response.user);
            observer.next(response);
          } else {
            observer.error('Login failed');
          }
          observer.complete();
        },
        error: (error) => observer.error(error)
      });
    });
  }

  register(request: RegisterRequest): Observable<AuthResponse> {
    return new Observable(observer => {
      this.http.post<AuthResponse>(`${this.apiUrl}/register`, request).subscribe({
        next: (response) => {
          if (response.success && response.token && response.user) {
            localStorage.setItem(this.tokenKey, response.token);
            this.currentUserSubject.next(response.user);
            observer.next(response);
          } else {
            observer.error('Registration failed');
          }
          observer.complete();
        },
        error: (error) => observer.error(error)
      });
    });
  }

  logout(): void {
    localStorage.removeItem(this.tokenKey);
    this.currentUserSubject.next(null);
  }

  private getUserFromStorage(): UserDto | null {
    // In a real application, you might decode the JWT and extract user info
    // For now, we'll return null and re-fetch from the server if needed
    return null;
  }
}
