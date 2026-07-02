/// <summary>
/// Đoạn service này dùng để quản lý thông tin phiên đăng nhập hiện tại của user trong frontend.
/// </summary>
import { Injectable } from "@angular/core";
import { environment } from "../../my-lib/shared/enviroments/enviroment";
import { BehaviorSubject, firstValueFrom, map, Observable } from "rxjs";
import { TokenService } from "../service-proxies/token.service";
import { HttpClient } from "@angular/common/http";

interface UserProfile {
    userId: number;
    username: string;
    email: string;
    fullname: string;
    displayname: string;
    avartarImageUrl: string;
    phoneNumber: string;
    userType: string;
}
@Injectable({
    providedIn: 'root'
})
export class AppSessionService {
    private readonly apiUrl = `${environment.api}/api`;
    private readonly _user$ = new BehaviorSubject<UserProfile | null>(null);

    constructor(
        private readonly tokenService: TokenService,
        private readonly http: HttpClient
    ) {
        
    }
    get User() : UserProfile | null {
        return this._user$.value;   
    }
    get UserId() : number | null {
        return this._user$.value ? this._user$.value.userId : null; // Trả về userId nếu có, ngược lại trả về null
    }
    setUser(value: UserProfile | null) : void {
        this._user$.next(value);
    }
    getUserByToken(): Observable<any> {
        return this.http
            .get<{data : any}>(`${this.apiUrl}/core/user/find-by-user`)
    }
    private fetchUserProfile(): Observable<UserProfile> {
        return this.http
            .get<{data : any}>(`${this.apiUrl}/core/user/find-by-user`)
            .pipe(
                map(({ data : u }) => ({
                    userId: u.id,
                    username: u.username,
                    email: u.email,
                    fullname: u.fullname,
                    displayname: u.displayname,
                    avartarImageUrl: u.avartarImageUrl,
                    phoneNumber: u.phoneNumber,
                    userType: u.userType
                }))
            );
    }
    // Hàm này được gọi khi ứng dụng khởi động để kiểm tra xem có token hợp lệ nào không 
    // và nếu có thì lấy thông tin người dùng tương ứng.
    async init(): Promise<boolean> {
        if ( !this.tokenService.getToken()) 
        {
            this.setUser(null);
            return false;
        }
        try 
        {
            const profile = await firstValueFrom(this.fetchUserProfile());
            this.setUser(profile);
            return true;
        }
        catch 
        {
            this.setUser(null);
            return false;
        }
    }
}