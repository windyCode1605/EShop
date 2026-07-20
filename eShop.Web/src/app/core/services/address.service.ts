import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface AddressResponseDto {
  id: number;
  street: string;
  city: string;
  province: string;
  receiverName: string;
  receiverPhone: string;
  isDefault: boolean;
}

export interface SaveAddressRequestDto {
  street: string;
  city: string;
  province: string;
  receiverName: string;
  receiverPhone: string;
  isDefault: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class AddressService {
  private http = inject(HttpClient);
  private endpoint = '/api/Address';

  getAddresses(): Observable<any> {
    return this.http.get<any>(this.endpoint);
  }

  saveAddress(dto: SaveAddressRequestDto): Observable<any> {
    return this.http.post<any>(this.endpoint, dto);
  }

  setAsDefault(addressId: number): Observable<any> {
    return this.http.patch<any>(`${this.endpoint}/${addressId}/default`, {});
  }
}
