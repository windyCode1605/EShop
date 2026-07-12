import { Injectable } from "@angular/core";
import { CookieService } from 'ngx-cookie-service';

/** Claim keys do OpenIddict phát hành trong Access Token */
const CLAIM_ROLE = 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role';
const CLAIM_USER_TYPE = 'UserType';
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
    private decodePayload(): Record<string, any> | null {
        const token = this.getToken();
        if (!token) return null;
        try {
            return JSON.parse(atob(token.split('.')[1]));
        } catch {
            return null;
        }
    }

    /**
     * Lấy role đầu tiên từ JWT (dùng cho backward compatibility).
     * OpenIddict phát hành role theo claim dài (chuẩn .NET) hoặc ngắn.
     */
    getUserRole(): string {
        const roles = this.getUserRoles();
        return roles[0] ?? '';
    }

    /**
     * Lấy toàn bộ roles từ JWT dưới dạng mảng.
     * OpenIddict có thể phát nhiều claim 'role' → Array hoặc string đơn.
     */
    getUserRoles(): string[] {
        const payload = this.decodePayload();
        if (!payload) return [];

        // OpenIddict có thể dùng cả 2 tên claim
        const raw = payload[CLAIM_ROLE] ?? payload['role'];
        if (!raw) return [];

        // Nếu là array thì trả về thẳng, nếu là string thì wrap
        return Array.isArray(raw) ? raw : [raw];
    }

    /** Lấy userId từ JWT (claim 'sub') */
    getUserId(): string | null {
        return this.decodePayload()?.[CLAIM_USER_ID] ?? null;
    }

    /**
     * Lấy UserType từ JWT (0=CUSTOMER, 1=ADMIN, 2=SUPER_ADMIN).
     * UserType là số nguyên, khác với dynamic Role name.
     */
    getUserType(): number | null {
        const raw = this.decodePayload()?.[CLAIM_USER_TYPE];
        return raw !== undefined ? Number(raw) : null;
    }

    /** Trả về true nếu UserType là ADMIN (1) hoặc SUPER_ADMIN (2) */
    isAdmin(): boolean {
        const userType = this.getUserType();
        return userType === 1 || userType === 2;
    }
}
