import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { ApiResponse } from '../../../core/models';
import { API_ENDPOINTS } from '../../../core/constants/api-endpoints.const';

export interface DashboardSummaryDto {
  totalRevenue: number;
  totalOrders: number;
  newCustomers: number;
  pendingOrders: number;
  revenueGrowthPercent: number;
  orderGrowthPercent: number;
  customerGrowthPercent: number;
}

export interface ChartPointDto {
  label: string;
  revenue: number;
}

export interface TopSellingProductDto {
  productId: number;
  productName: string;
  imageUrl: string | null;
  price: number;
  totalSold: number;
}

export interface DashboardResponseDto {
  summary: DashboardSummaryDto;
  revenueChart: ChartPointDto[];
  topSellingProducts: TopSellingProductDto[];
}

@Injectable({
  providedIn: 'root'
})
export class AdminDashboardService {
  constructor(private http: HttpClient) {}

  getSummary(): Observable<DashboardResponseDto> {
    return this.http
      .get<ApiResponse<DashboardResponseDto>>(API_ENDPOINTS.ADMIN.DASHBOARD.SUMMARY)
      .pipe(map(res => res.data!));
  }
}
