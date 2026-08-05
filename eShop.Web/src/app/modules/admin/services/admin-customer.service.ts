import { Injectable, signal } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { API_ENDPOINTS } from '../../../core/constants/api-endpoints.const';
import { ApiResponse } from '../../../core/models';
import { AdminCustomerListResponse, IAdminCustomerFilter, ICustomerDetail, ICustomerStatistics } from '../models/admin-customer.model';

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

  // GET /api/admin/customers/{id} 
  getCustomerDetail(id: number): Observable<ICustomerDetail> {
    return this.http.get<ApiResponse<ICustomerDetail>>(API_ENDPOINTS.ADMIN.CUSTOMER.GET_BY_ID(id))
      .pipe(map(res => res.data!));
  }

  // GET /api/admin/customers/{id}/statistics 
  getCustomerStatistics(id: number): Observable<ICustomerStatistics> {
    return this.http.get<ApiResponse<ICustomerStatistics>>(API_ENDPOINTS.ADMIN.CUSTOMER.STATISTICS(id))
      .pipe(map(res => res.data!));
  }

  // POST /api/admin/customers/{id}/lock
  lockCustomer(id: number): Observable<ApiResponse<boolean>> {
    return this.http.post<ApiResponse<boolean>>(API_ENDPOINTS.ADMIN.CUSTOMER.LOCK(id), {});
  }

  // POST /api/admin/customers/{id}/unlock
  unlockCustomer(id: number): Observable<ApiResponse<boolean>> {
    return this.http.post<ApiResponse<boolean>>(API_ENDPOINTS.ADMIN.CUSTOMER.UNLOCK(id), {});
  }
}
