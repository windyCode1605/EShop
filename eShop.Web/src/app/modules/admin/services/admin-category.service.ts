import { Injectable, signal } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../my-lib/shared/enviroments/enviroment';
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
  private baseUrl = `${environment.api}/api/Category`;

  // Local signal state
  categories = signal<IAdminCategory[]>([]);
  loading = signal<boolean>(false);

  constructor(private http: HttpClient) { }

  // ── Categories CRUD ───────────────────────────────────────────────────────

  /**
   * GET /api/Category/getCategory
   */
  getCategories(page: number = 1, size: number = 50, keyword?: string): Observable<ICategoryListResponse> {
    let params = new HttpParams();
    // if backend supports pagination, pass them
    // params = params.set('page', page.toString()).set('size', size.toString());
    // if (keyword) params = params.set('keyword', keyword);
    return this.http.get<Result<IAdminCategory[]>>(`${this.baseUrl}/getCategory`, { params })
      .pipe(map(res => {
        const items = res.value || [];
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
    return this.http.get<Result<IAdminCategory>>(`${this.baseUrl}/${id}`)
      .pipe(map(res => res.value));
  }

  /**
   * POST /api/Category
   */
  createCategory(dto: ICategoryCreateDto): Observable<IAdminCategory> {
    return this.http.post<Result<IAdminCategory>>(this.baseUrl, dto)
      .pipe(map(res => res.value));
  }

  /**
   * PUT /api/Category/:id
   */
  updateCategory(id: number, dto: ICategoryUpdateDto): Observable<IAdminCategory> {
    return this.http.put<Result<IAdminCategory>>(`${this.baseUrl}/${id}`, dto)
      .pipe(map(res => res.value));
  }

  /**
   * DELETE /api/Category/:id
   */
  deleteCategory(id: number): Observable<void> {
    return this.http.delete<Result<void>>(`${this.baseUrl}/${id}`)
      .pipe(map(res => res.value));
  }

  /**
   * PATCH /api/Category/:id/toggle-active
   */
  toggleCategoryActive(id: number): Observable<IAdminCategory> {
    return this.http.patch<Result<IAdminCategory>>(`${this.baseUrl}/${id}/toggle-active`, {})
      .pipe(map(res => res.value));
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
