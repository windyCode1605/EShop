import { Injectable, signal } from "@angular/core";
import { API_ENDPOINTS } from "../../../core/constants/api-endpoints.const";
import { HttpClient } from "@angular/common/http";
import { BehaviorSubject, Observable, of, tap, catchError, EMPTY } from "rxjs";
import { Address, AddressCreatDto } from "../models/address.model";

@Injectable({ providedIn: 'root' })
export class AddressService {

    savedAddresses = signal<Address[]>([]);

    private loadingSubject = new BehaviorSubject<boolean>(false);
    public loading$ = this.loadingSubject.asObservable();

    constructor(private http: HttpClient) { }

    loadAddress(): Observable<any> {
        if (this.savedAddresses().length > 0) return of(null);

        return this.http.get<any>(API_ENDPOINTS.ADDRESS.GET_ALL).pipe(
            tap({
                next: (response) => {
                    if (response.success && response.data) {
                        this.savedAddresses.set(response.data);
                    }
                }
            }),
            catchError((err) => {
                console.error('Lỗi khi tải địa chỉ:', err);
                return of(null); // Trả về null thay vì throw để tránh crash component
            })
        );
    }

    // Fix: Dùng pipe(tap) trực tiếp thay vì new Observable bọc subscribe (anti-pattern)
    createNewAddress(addressCreatDto: AddressCreatDto): Observable<any> {
        this.loadingSubject.next(true);

        return this.http.post<any>(API_ENDPOINTS.ADDRESS.CREATE, addressCreatDto).pipe(
            tap({
                next: (response) => {
                    if (response.success && response.data) {
                        const newAddress: Address = {
                            id: response.data,
                            receiverName: addressCreatDto.receiverName,
                            receiverPhone: addressCreatDto.receiverPhone,
                            street: addressCreatDto.street,
                            city: addressCreatDto.city,
                            province: addressCreatDto.province,
                            isDefault: this.savedAddresses().length === 0
                        };
                        this.savedAddresses.update(addresses => [...addresses, newAddress]);
                    }
                    this.loadingSubject.next(false);
                },
                error: () => {
                    this.loadingSubject.next(false);
                }
            }),
            catchError((err) => {
                console.error('Lỗi khi thêm địa chỉ mới:', err);
                this.loadingSubject.next(false);
                throw err; // Re-throw để component có thể bắt ở error handler
            })
        );
    }
}
