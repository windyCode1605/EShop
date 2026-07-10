import { Injectable } from "@angular/core";
import { CookieService } from 'ngx-cookie-service';

@Injectable({
    providedIn: 'root'
})
export class TokenService {

    constructor(private _cookieService: CookieService) { }

    setToken(accessToken: string, expireDate: Date): void {
        this._cookieService.set('access_token', accessToken, expireDate, '/');
    }
    setRefreshToken(refreshToken: string): void {
        this._cookieService.set('refresh_token', refreshToken, undefined, '/');
    }

    getToken(): string | null {
        return this._cookieService.get('access_token');
    }

    getRefreshToken(): string {
        return this._cookieService.get('refresh_token');
    }

    clearAllCookie(): void {
        this._cookieService.delete('access_token', '/');
        this._cookieService.delete('refresh_token', '/');
    }
    getUserRole(): string {
        const token = this.getToken();
        if (!token) return '';

        try {
            // Decode JWT token payload (phần thứ 2 của token)
            const payload = JSON.parse(atob(token.split('.')[1]));
            return payload.role ||
                payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] || 'Customer';
        } catch (e) {
            return '';
        }
    }
}
