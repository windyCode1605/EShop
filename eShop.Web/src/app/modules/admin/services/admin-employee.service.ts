import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_ENDPOINTS } from '../../../core/constants/api-endpoints.const';
import { IAdminEmployee, IAdminEmployeeQuery } from '../models/admin-employee.model';

export interface IPageResult<T> {
  items: T[];
  totalItems: number;
  totalPages: number;
  currentPage: number;
  pageSize: number;
  hasNextPage: boolean;
  hasPreviousPage: boolean;
}

export interface IApiResponse<T> {
  data: T;
  message: string;
  statusCode: number;
}

@Injectable({
  providedIn: 'root'
})
export class AdminEmployeeService {
  private http = inject(HttpClient);

  getEmployees(query: IAdminEmployeeQuery): Observable<IApiResponse<IPageResult<IAdminEmployee>>> {
    let params = new HttpParams()
      .set('pageNumber', query.pageNumber.toString())
      .set('pageSize', query.pageSize.toString());

    if (query.keyword) {
      params = params.set('keyword', query.keyword);
    }
    if (query.isActive !== undefined && query.isActive !== null) {
      params = params.set('isActive', query.isActive.toString());
    }
    if (query.formDate) {
      params = params.set('formDate', query.formDate);
    }
    if (query.toDate) {
      params = params.set('toDate', query.toDate);
    }

    return this.http.get<IApiResponse<IPageResult<IAdminEmployee>>>(API_ENDPOINTS.ADMIN.EMPLOYEE.GET_ALL, { params });
  }

  assignRole(userId: number, roleId: number): Observable<IApiResponse<boolean>> {
    const payload = { roleId };
    return this.http.post<IApiResponse<boolean>>(API_ENDPOINTS.ADMIN.USER.UPDATE_ROLES(userId), payload);
  }
}
