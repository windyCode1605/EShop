import { Injectable } from "@angular/core";
import { HttpErrorResponse } from "@angular/common/http";
import { AuthenticateModel, AuthenticateResultModel } from "../service-proxies/service-proxies";
import { AuthService } from "../services/auth.service";
import { MessageService } from 'primeng/api';
import { catchError, finalize, throwError } from "rxjs";
import { jwtDecode } from 'jwt-decode';
import { TokenService } from "../service-proxies/token.service";
import { Router } from "@angular/router";
import { AppSessionService } from "../session/app-session.service";
import { PermissionStore } from "../stores/permission.store";

@Injectable({
    providedIn: 'any'
})
export class AppAuthService {
    authenticateModel!: AuthenticateModel;
    authenticateResult: AuthenticateResultModel | null = null;
    rememberMe: boolean = false;

    constructor(
        private _tokenAuthService: AuthService,
        private _tokenService: TokenService,
        private messageService: MessageService,
        private _router: Router,
        private _appSessionService: AppSessionService,
        private _permissionStore: PermissionStore
    ) {
        this.clear();
    }

    authenticate(finallyCallback?: () => void, returnUrl?: string): void {
        finallyCallback = finallyCallback || (() => { });
        this._tokenAuthService.authenticateV2(this.authenticateModel).pipe(
            catchError((err: HttpErrorResponse) => {
                const errorMessage = this.getAuthErrorMessage(err);
                this.messageService.add({
                    severity: 'error',
                    summary: 'Đăng nhập thất bại',
                    life: 5000,
                    detail: errorMessage
                });
                console.error('Authentication error:', err);
                if (finallyCallback) finallyCallback();
                return throwError(() => err);
            }),
            finalize(() => {
                finallyCallback();
            })
        ).subscribe((result: AuthenticateResultModel) => {
            this.processAuthenticateResult(result, returnUrl);
        });
    }

    clear() {
        this.authenticateModel = new AuthenticateModel();
        this.authenticateModel.rememberClient = false;
        this.authenticateResult = null;
        this.rememberMe = false;
    }

    /** Đăng xuất — xóa token, clear store, về trang login */
    logout(): void {
        this._tokenService.clearAllCookie();
        this._permissionStore.clear();
        this._appSessionService.setUser(null);
        this._router.navigate(['/account/login']);
    }

    private getAuthErrorMessage(err: HttpErrorResponse): string {
        if (err.status === 400 && err.error?.error === 'invalid_grant') return 'Tên đăng nhập hoặc mật khẩu không đúng.';
        if (err.status === 401) return 'Tài khoản hoặc mật khẩu không đúng.';
        if (err.status === 429) return 'Quá nhiều lần thử đăng nhập. Vui lòng thử lại sau.';
        if (err.status === 0) return 'Không thể kết nối đến máy chủ. Vui lòng kiểm tra kết nối mạng.';
        return 'Đã xảy ra lỗi khi đăng nhập. Vui lòng thử lại.';
    }

    private processAuthenticateResult(authenticateResult: AuthenticateResultModel, returnUrl?: string): void {
        this.authenticateResult = authenticateResult;
        console.log('[DEBUG 1] API Trả về kết quả login:', authenticateResult)
        if (authenticateResult.access_token) {
            this.login(
                authenticateResult.access_token,
                authenticateResult.refresh_token ?? '',
                authenticateResult.encryptedAccessToken ?? '',
                authenticateResult.expires_in,
                this.authenticateModel.rememberClient,
                returnUrl
            );
        } else {
            this._router.navigate(['account/login']);
        }
    }

    private login(
        accessToken: string,
        refreshToken: string,
        encryptedAccessToken: string,
        expiresIn: number,
        rememberMe?: boolean,
        returnUrl?: string
    ): void {
        const decoded = jwtDecode<{ exp?: number }>(accessToken);
        const tokenExpireDate = this.unixToDate(decoded.exp ?? 0);


        this._tokenService.setToken(accessToken, tokenExpireDate);
        this._tokenService.setRefreshToken(refreshToken);

        Promise.all([
            this._appSessionService.init(),
            this._permissionStore.load()
        ]).then(() => {
            this.navigateAfterLogin(returnUrl, accessToken);
        }).catch(err => {
            console.error('[AppAuthService] Lỗi khi tải thông tin sau đăng nhập:', err);
            this.navigateAfterLogin(returnUrl, accessToken);
        });
    }

    private navigateAfterLogin(returnUrl?: string, accessToken?: string): void {
        const isAdmin = this._tokenService.isAdmin(accessToken);

        // Nếu là Admin và returnUrl là trang chủ/trang khách hàng mặc định do authGuard đẩy về,
        // thì ưu tiên chuyển hướng thẳng vào trang quản trị thay vì trang khách hàng.
        if (isAdmin && returnUrl && (returnUrl === '/' || returnUrl.startsWith('/dashboard'))) {
            returnUrl = undefined;
        }

        if (returnUrl) {
            this._router.navigateByUrl(returnUrl).catch(err => console.error('Navigation error:', err));
            return;
        }

        if (isAdmin) {
            this._router.navigate(['/admin/dashboard']).catch(err => console.error('Navigation error:', err));
        } else {
            this._router.navigate(['/']).catch(err => console.error('Navigation error:', err));
        }
    }

    /// <summary>
    /// Hàm này chuyển đổi thời gian từ định dạng unix timestamp (số giây kể từ 1/1/1970) sang đối tượng Date của JavaScript.
    /// </summary>
    private unixToDate(unixTimestamp: number): Date {
        return new Date(unixTimestamp * 1000);
    }
}