import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../my-lib/shared/enviroments/enviroment';
import { API_ENDPOINTS } from '../../../core/constants/api-endpoints.const';

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

  constructor(private http: HttpClient) {}

  createOrder(payload: CreateOrderRequest): Observable<any> {
    return this.http.post<any>(API_ENDPOINTS.ORDER.CREATE, payload);
  }

  getOrderById(id: number): Observable<any> {
    return this.http.get<any>(API_ENDPOINTS.ORDER.GET_BY_ID(id));
  }

  getMyOrders(params: any = {}): Observable<any> {
    return this.http.get<any>(API_ENDPOINTS.ORDER.MY_ORDERS, { params });
  }
}
