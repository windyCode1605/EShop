import { Injectable, signal } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_ENDPOINTS } from '../../../core/constants/api-endpoints.const';
import { map } from 'rxjs/operators';
import { ApiResponse } from '../../../core/models';
import {
  IAdminCategory,
  ICategoryCreateDto,
  ICategoryUpdateDto,
  ICategoryListResponse
} from '../models/admin-category.model';

export interface Result<T> {
  value: T;
  isSuccess: boolean;
  isFailure: boolean;
  errorCode: number;
}

@Injectable({ providedIn: 'root' })
export class AdminCategoryService {
  // Local signal state
  categories = signal<IAdminCategory[]>([]);
  loading = signal<boolean>(false);

  constructor(private http: HttpClient) { }


  // GET /api/Category/getCategory
  getCategories(page: number = 1, size: number = 50, keyword?: string): Observable<ICategoryListResponse> {
    let params = new HttpParams();
    return this.http.get<ApiResponse<IAdminCategory[]>>(API_ENDPOINTS.CATEGORY.GET_ALL, { params })
      .pipe(map(res => {
        const items = res.data || [];
        return {
          items: items.map(c => ({
            ...c,
            slug: c.slug || this.generateSlug(c.categoryName),
            isActive: c.isActive ?? true,
            sortOrder: c.sortOrder ?? 0
          })),
          totalCount: items.length,
          page: 1,
          pageSize: items.length,
          totalPages: 1,
          hasNext: false,
          hasPrev: false
        };
      }));
  }

  /**
   * GET /api/Category/:id
   */
  getCategoryById(id: number): Observable<IAdminCategory> {
    return this.http.get<ApiResponse<IAdminCategory>>(API_ENDPOINTS.CATEGORY.GET_BY_ID(id))
      .pipe(map(res => res.data!));
  }

  /**
   * POST /api/Category
   */
  createCategory(dto: ICategoryCreateDto): Observable<IAdminCategory> {
    return this.http.post<ApiResponse<IAdminCategory>>(API_ENDPOINTS.CATEGORY.CREATE, dto)
      .pipe(map(res => res.data!));
  }

  /**
   * PUT /api/Category/:id
   */
  updateCategory(id: number, dto: ICategoryUpdateDto): Observable<IAdminCategory> {
    return this.http.put<ApiResponse<IAdminCategory>>(API_ENDPOINTS.CATEGORY.UPDATE(id), dto)
      .pipe(map(res => res.data!));
  }

  /**
   * DELETE /api/Category/:id
   */
  deleteCategory(id: number): Observable<void> {
    return this.http.delete<ApiResponse<void>>(API_ENDPOINTS.CATEGORY.DELETE(id))
      .pipe(map(res => res.data!));
  }

  /**
   * PATCH /api/Category/:id/toggle-active
   */
  toggleCategoryActive(id: number): Observable<IAdminCategory> {
    return this.http.patch<ApiResponse<IAdminCategory>>(API_ENDPOINTS.CATEGORY.TOGGLE_ACTIVE(id), {})
      .pipe(map(res => res.data!));
  }

  // ── Utility ─────────────────────────────────────────────────────────────

  /**
   * Sinh slug từ tên category
   */
  generateSlug(name: string): string {
    return name
      .toLowerCase()
      .normalize('NFD')
      .replace(/[\u0300-\u036f]/g, '')
      .replace(/đ/g, 'd')
      .replace(/[^a-z0-9\s-]/g, '')
      .trim()
      .replace(/\s+/g, '-')
      .replace(/-+/g, '-');
  }
}
