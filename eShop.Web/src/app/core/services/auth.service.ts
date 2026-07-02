import { HttpClient, HttpHeaders, HttpParams } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { environment } from "../../my-lib/shared/enviroments/enviroment";
import { catchError, Observable, throwError } from "rxjs";

@Injectable({
    providedIn: "root",
})
export class AuthService {
    constructor(private http: HttpClient) { }
    private apiUrl = `${environment.api}`;

    authenticateV2(body: any): Observable<any> {
        const url = `${this.apiUrl}/connect/token`;
        const formData = new HttpParams()
            .set('username', body.username)
            .set('password', body.password)
            .set('grant_type', 'password')
            .set('scope', environment.scopes)
            .set('client_id', environment.clientId)
            .set('client_secret', environment.clientSecret);
        const headers = new HttpHeaders({
            'Content-Type': 'application/x-www-form-urlencoded'
        });

        return this.http.post(url, formData.toString(), { headers })
            .pipe(
                catchError((err) => {
                    console.error('Authentication error:', err);
                    return throwError(() => err);
                })
            );
    }

    logout(): Observable<any> {
        const headers = new HttpHeaders({
            'Content-Type': 'application/x-www-form-urlencoded',
            Accept: 'text/plain'
        });

        const params = new HttpParams().set('revokeAll', false);
        return this.http.post<any>(`${this.apiUrl}/connect/logout`, params.toString(), { headers });
    }
}