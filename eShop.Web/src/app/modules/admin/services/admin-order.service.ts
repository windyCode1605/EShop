import { Injectable, signal } from '@angular/core';
import { IAdminOrder, IAdminOrderPagedResult } from '../models/admin-orders.models';
import { API_ENDPOINTS } from '../../../core/constants/api-endpoints.const';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface IUpdateOrderStatus {
  newStatus: string;
  trackingNumber?: string;
  shippingProvider?: string;
  reason?: string;
}

@Injectable({
    providedIn: 'root'
})
export class AdminOrderService { // Đổi tên cho đúng file

    Orders = signal<IAdminOrder[]>([]);

    constructor(private http: HttpClient) { }

    loadOrder() {
        if (this.Orders().length > 0) return;
        this.http.get<any>(`${API_ENDPOINTS.ADMIN.ORDER.BASE}?pageNumber=1&pageSize=10`)
            .subscribe({
                next: (response) => {
                    if (response.isSuccess && response.value && response.value.items) {
                        this.Orders.set(response.value.items);
                    } else {
                        console.warn('API trả về dữ liệu không hợp lệ:', response);
                    }
                },
                error: (error: HttpErrorResponse) => {
                    console.error('Lỗi khi load danh sách đơn hàng Admin:', error);
                }
            });
    }

    updateOrderStatus(orderId: string | number, payload: IUpdateOrderStatus): Observable<any> {
        return this.http.patch<any>(API_ENDPOINTS.ADMIN.ORDER.UPDATE_STATUS(orderId), payload);
    }
}
