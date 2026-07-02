export class AuthenticateModel implements IAuthenticateModel {
    username!: string;
    password!: string;
    rememberClient!: boolean;
    clientId!: string;
    secret!: string;
    
    // Hàm khởi tạo để gán giá trị cho các thuộc tính
    constructor(data?: IAuthenticateModel) {
        if (data) {
            for (var property in data) {
                if (data.hasOwnProperty(property))
                    (this as any)[property] = (data as any)[property];
            }
        }
    }
}
export class AuthenticateResultModel implements IAuthenticateResultModel {
    access_token: string | undefined;
    refresh_token: string | undefined;
    encryptedAccessToken: string | undefined;
    expires_in!: number;
    userId: number | undefined;

    // Hàm khởi tạo để gán giá trị cho các thuộc tính
    constructor(data?: IAuthenticateResultModel) {
        if (data) {
            for (var property in data) {
                if (data.hasOwnProperty(property))
                    (this as any)[property] = (data as any)[property];
            }
        }
    }
}
export interface IAuthenticateModel {
    username: string; 
    password: string; 
    rememberClient: boolean;
    clientId: string;
    secret: string;
    
}
export interface IAuthenticateResultModel {
    access_token: string | undefined;
    refresh_token: string | undefined;
    encryptedAccessToken: string | undefined;           // Chuỗi mã hóa của access token, có thể dùng để lưu trữ an toàn hơn
    expires_in: number;
    userId: number | undefined;
}