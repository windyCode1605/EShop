import { Injectable } from "@angular/core";
import { HttpErrorResponse } from "@angular/common/http";
import { AuthenticateModel, AuthenticateResultModel } from "../service-proxies/service-proxies";
import { AuthService } from "../services/auth.service";
import { environment } from "../../my-lib/shared/enviroments/enviroment";
import { MessageService } from 'primeng/api';
import { catchError, finalize, throwError } from "rxjs";
import { jwtDecode } from 'jwt-decode';
import { TokenService } from "../service-proxies/token.service";
import { Router } from "@angular/router";
import { AppSessionService } from "../session/app-session.service";




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
        private _appSessionService: AppSessionService
    ) {
        
        this.clear()
    }
    authenticate(finallyCallback?: () => void, returnUrl?: string): void {
        finallyCallback = finallyCallback || (() => { });
        this._tokenAuthService.authenticateV2(this.authenticateModel).pipe(
            catchError((err: HttpErrorResponse) => {
                const errorMessage = this.getAuthErrorMessage(err);
                this.messageService.add({ 
                    severity: 'error', 
                    summary: 'Authentication Failed',
                    life: 5000, 
                    detail: errorMessage });
                    console.error('Authentication error:', err);
                if (finallyCallback) finallyCallback();
                return throwError(() => err);
            }),
            finalize(() => {
                finallyCallback();
            })
        ).subscribe((result: AuthenticateResultModel) => {(this.processAuthenticateResult(result, returnUrl))});
    }
    

    clear() {
        this.authenticateModel = new AuthenticateModel();
        this.authenticateModel.rememberClient = false;
        this.authenticateResult = null;
        this.rememberMe = false;
    }

    private getAuthErrorMessage(err: HttpErrorResponse): string {
        if (err.status === 400 && err.error && err.error.error === 'invalid_grant') return 'Tên đăng nhập hoặc mật khẩu không đúng.';
        if (err.status === 401) return 'Tài khoản hoặc mật khẩu không đúng.';
        if (err.status === 429) return 'Quá nhiều lần thử đăng nhập. Vui lòng thử lại sau.';
        if (err.status === 0) return 'Không thể kết nối đến máy chủ. Vui lòng kiểm tra kết nối mạng.';
        return 'Đã xảy ra lỗi khi đăng nhập. Vui lòng thử lại.';
    }


    private processAuthenticateResult( authenticateResult: AuthenticateResultModel, returnUrl?: string) : void {
        this.authenticateResult = authenticateResult;
        if(authenticateResult.access_token) {
            this.login(
                authenticateResult.access_token,
                authenticateResult.refresh_token ?? '',
                authenticateResult.encryptedAccessToken ?? '',
                authenticateResult.expires_in,
                this.authenticateModel.rememberClient,
                returnUrl
            );
        }
        else {
            this._router.navigate(['account/login']);
        }
    }

    private login( 
        acessToken: string, 
        refreshToken: string,
        excryptedAccesToken: string, 
        expiresIn: number,
        rememberMe?: boolean,
        returnUrl?: string
    ) : void { 
        const decoded = jwtDecode<{ exp?: number }>(acessToken);
        const tokenExpireDate = this.unixToDate(decoded.exp ?? 0);
        this._tokenService.setToken(acessToken, tokenExpireDate);
        this._tokenService.setRefreshToken(refreshToken);
        this._appSessionService.init().then(
            () => {

            },
            (err: any) => {
                console.error(err);
            }
        );
        const token = this._tokenService.getToken();
        let userIfo : any;

        if(token) {
            userIfo = jwtDecode(token);
        }

        // Ưu tiên returnUrl nếu có
        if (returnUrl) {
            this._router.navigateByUrl(returnUrl).catch(err => console.error('Navigation error:', err));
            return;
        }

        if(userIfo?.user_type === 2 || userIfo?.user_type === 1 || userIfo?.userType === 2 || userIfo?.userType === 1) {
            // Admin/Staff: redirect to product manager
            this._router.navigate(['/product-manager']).catch(err => console.error('Navigation error:', err));
        } else {
            // Customer: redirect to customer product page
            this._router.navigate(['/product']).catch(err => console.error('Navigation error:', err));
        }
    }
    /// <summary>
    /// Hàm này chuyển đổi thời gian từ định dạng unix timestamp (số giây kể từ 1/1/1970) sang đối tượng Date của JavaScript.
    /// </summary>
    private unixToDate(unixTimestamp: number): Date {
        return new Date(unixTimestamp * 1000);
    }
}