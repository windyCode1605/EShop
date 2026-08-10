import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_ENDPOINTS } from '../constants/api-endpoints.const';
import { SysvarResponseDto, SysvarUpdateDto } from '../models/sysvar.model';
import { ApiResponse } from '../models/index';

@Injectable({
  providedIn: 'root'
})
export class SysvarService {
  private http = inject(HttpClient);
  private endpoint = API_ENDPOINTS.ADMIN.SYSVAR;

  getAllSysVars(): Observable<ApiResponse<SysvarResponseDto[]>> {
    return this.http.get<ApiResponse<SysvarResponseDto[]>>(this.endpoint.GET_ALL);
  }

  updateSysVar(id: number, dto: SysvarUpdateDto): Observable<ApiResponse<any>> {
    return this.http.put<ApiResponse<any>>(this.endpoint.UPDATE(id), dto);
  }
}
