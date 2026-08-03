import { Injectable, signal } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { API_ENDPOINTS } from '../../../core/constants/api-endpoints.const';
import { ApiResponse } from '../../../core/models';
import { AdminCustomerListResponse, IAdminCustomerFilter } from '../models/admin-customer.model';

@Injectable({ providedIn: 'root' })
export class AdminCustomerService {
  loading = signal<boolean>(false);

  constructor(private http: HttpClient) { }

  /**
   * GET /api/admin/customers — paginated list with filters
   */
  getCustomers(filter: IAdminCustomerFilter): Observable<AdminCustomerListResponse> {
    let params = new HttpParams()
      .set('pageNumber', filter.pageIndex.toString())
      .set('pageSize', filter.pageSize.toString());

    if (filter.keyword) params = params.set('keyword', filter.keyword);
    if (filter.isActive !== undefined) params = params.set('isActive', filter.isActive.toString());
    if (filter.customerSegment) params = params.set('customerSegment', filter.customerSegment);
    if (filter.fromDate) params = params.set('fromDate', filter.fromDate);
    if (filter.toDate) params = params.set('toDate', filter.toDate);
    if (filter.minSpent !== undefined) params = params.set('minSpent', filter.minSpent.toString());
    if (filter.maxSpent !== undefined) params = params.set('maxSpent', filter.maxSpent.toString());

    return this.http.get<ApiResponse<AdminCustomerListResponse>>(API_ENDPOINTS.ADMIN.CUSTOMER.GET_ALL, { params })
      .pipe(map(res => res.data!));
  }
}
