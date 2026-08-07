import { Injectable, signal } from '@angular/core';
import { IAdminOrder, IAdminOrderPagedResult } from '../models/admin-orders.models';
import { API_ENDPOINTS } from '../../../core/constants/api-endpoints.const';
import { HttpClient, HttpErrorResponse, HttpParams } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { ApiResponse } from '../../../core/models';

export interface IUpdateOrderStatus {
  newStatus: string;
  trackingNumber?: string;
  shippingProvider?: string;
  reason?: string;
}

export interface IOrderFilter {
  pageNumber?: number;
  pageSize?: number;
  keyword?: string;
  status?: string;
}

@Injectable({
    providedIn: 'root'
})
export class AdminOrderService {

    Orders = signal<IAdminOrder[]>([]);
    totalCount = signal<number>(0);

    constructor(private http: HttpClient) { }

    /** Load orders vào signal (dùng cho các component cần reactive state) */
    loadOrder(filter: IOrderFilter = { pageNumber: 1, pageSize: 10 }) {
        this.getOrders(filter).subscribe({
            next: (data) => {
                this.Orders.set(data.items ?? []);
                this.totalCount.set(data.totalItems ?? 0);
            },
            error: (error: HttpErrorResponse) => {
                console.error('Lỗi khi load danh sách đơn hàng Admin:', error);
            }
        });
    }

    /** Trả về Observable — dùng cho component cần xử lý pagination thủ công */
    getOrders(filter: IOrderFilter = {}): Observable<any> {
        let params = new HttpParams()
            .set('pageNumber', (filter.pageNumber ?? 1).toString())
            .set('pageSize', (filter.pageSize ?? 10).toString());

        if (filter.keyword) params = params.set('keyword', filter.keyword);
        if (filter.status) params = params.set('status', filter.status);

        return this.http.get<ApiResponse<any>>(API_ENDPOINTS.ADMIN.ORDER.GET_ALL, { params })
            .pipe(map(res => res.data));
    }

    updateOrderStatus(orderId: string | number, payload: IUpdateOrderStatus): Observable<any> {
        return this.http.patch<ApiResponse<any>>(API_ENDPOINTS.ADMIN.ORDER.UPDATE_STATUS(orderId), payload)
            .pipe(map(res => res.data));
    }
}
