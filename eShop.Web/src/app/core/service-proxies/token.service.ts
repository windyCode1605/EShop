import { Injectable } from "@angular/core";
import { CookieService } from 'ngx-cookie-service';
import { jwtDecode } from 'jwt-decode';

/** Claim keys do OpenIddict phát hành trong Access Token */
const CLAIM_ROLE = 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role';
const CLAIM_USER_TYPE = 'user_type';
const CLAIM_USER_ID = 'sub';

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
        return this._cookieService.get('access_token') || null;
    }

    getRefreshToken(): string {
        return this._cookieService.get('refresh_token');
    }

    clearAllCookie(): void {
        this._cookieService.delete('access_token', '/');
        this._cookieService.delete('refresh_token', '/');
    }

    /** Decode payload của JWT token hiện tại, trả về null nếu không hợp lệ */
    private decodePayload(tokenOverride?: string): Record<string, any> | null {
        const token = tokenOverride || this.getToken();
        if (!token) return null;
        try {
            return jwtDecode(token);
        } catch {
            return null;
        }
    }

    /**
     * Lấy role đầu tiên từ JWT (dùng cho backward compatibility).
     * OpenIddict phát hành role theo claim dài (chuẩn .NET) hoặc ngắn.
     */
    getUserRole(tokenOverride?: string): string {
        const roles = this.getUserRoles(tokenOverride);
        return roles[0] ?? '';
    }

    /**
     * Lấy toàn bộ roles từ JWT dưới dạng mảng.
     * OpenIddict có thể phát nhiều claim 'role' → Array hoặc string đơn.
     */
    getUserRoles(tokenOverride?: string): string[] {
        const payload = this.decodePayload(tokenOverride);
        if (!payload) return [];

        const raw = payload[CLAIM_ROLE] ?? payload['role'];
        if (!raw) return [];

        return Array.isArray(raw) ? raw : [raw];
    }


    getUserId(tokenOverride?: string): string | null {
        return this.decodePayload(tokenOverride)?.[CLAIM_USER_ID] ?? null;
    }

    /**
     * Lấy UserType từ JWT (0=CUSTOMER, 1=ADMIN, 2=SUPER_ADMIN).
     * UserType là số nguyên, khác với dynamic Role name.
     */
    getUserType(tokenOverride?: string): number | null {
        const payload = this.decodePayload(tokenOverride);
        const raw = payload?.[CLAIM_USER_TYPE];
        return raw !== undefined ? Number(raw) : null;
    }

    isAdmin(tokenOverride?: string): boolean {

        const userType = this.getUserType(tokenOverride);
        if (userType === 1 || userType === 2) {
            return true;
        }
        const roles = this.getUserRoles(tokenOverride);
        return roles.includes('SuperAdmin') || roles.includes('Admin');
    }
}
