import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../my-lib/shared/enviroments/enviroment';

export interface CreateOrderRequest {
  addressId: number;
  receiverName: string;
  receiverPhone: string;
  street: string;
  city: string;
  province: string;
  paymentMethod: string;
  shippingProvider: string;
  couponCode: string;
  note: string;
}

@Injectable({
  providedIn: 'root'
})
export class OrderService {
  private apiUrl = environment.api;

  constructor(private http: HttpClient) {}

  createOrder(payload: CreateOrderRequest): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/api/Order/create`, payload);
  }

  getOrderById(id: number): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/api/Order/${id}`);
  }

  getMyOrders(params: any = {}): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/api/Order/my-orders`, { params });
  }
}
