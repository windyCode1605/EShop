import { Injectable, signal } from "@angular/core";
import { environment } from "../../../my-lib/shared/enviroments/enviroment";
import { HttpClient } from "@angular/common/http";
import { Observable, of, tap } from "rxjs";

@Injectable({ providedIn: 'root' })

export class AddressService {

    private apiUrl = environment.api;
    savedAddresses = signal<Address[]>([]);

    constructor(private http: HttpClient) { }
    loadAddress(): Observable<any> {
        if (this.savedAddresses().length > 0) return of(null);
        
        return this.http.get<any>(`${this.apiUrl}/api/Address`).pipe(
            tap({
                next: (response) => {
                    if (response.success && response.data) {
                        this.savedAddresses.set(response.data);
                    }
                },
                error: (err) => {
                    console.error('Lỗi khi tải địa chỉ:', err);
                }
            })
        );
    }
}
interface Address {
    id: number;
    receiverName: string;
    receiverPhone: string;
    street: string;
    city: string;
    province: string;
    isDefault: boolean;
}