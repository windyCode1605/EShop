import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { ProductResponseDto, ApiPaginatedResponse, IProduct } from '../../product-manager/models/product.model';
import { ProductFilterModel } from '../../product-manager/models/product-filter.model';
import { environment } from '../../../my-lib/shared/enviroments/enviroment';
import { API_ENDPOINTS } from '../../../core/constants/api-endpoints.const';

@Injectable({ providedIn: 'root' })
export class ProductService {
  constructor(private http: HttpClient) { }

  getProducts(filter: ProductFilterModel): Observable<ProductResponseDto> {
    let params = new HttpParams()
      .set('page', filter.pageIndex.toString())
      .set('size', filter.pageSize.toString());

    if (filter.keyword) params = params.set('keyword', filter.keyword);
    if (filter.categoryId) params = params.set('categoryId', filter.categoryId);
    if (filter.minPrice !== undefined) params = params.set('minPrice', filter.minPrice.toString());
    if (filter.maxPrice !== undefined) params = params.set('maxPrice', filter.maxPrice.toString());

    // params = params.set('isActive', 'true');

    if (filter.sortBy) params = params.set('sortBy', filter.sortBy);
    if (filter.sortOrder) params = params.set('sortOrder', filter.sortOrder);

    return this.http.get<ApiPaginatedResponse<IProduct>>(API_ENDPOINTS.PRODUCT.GET_ALL, { params }).pipe(
      map(response => ({
        items: response.data?.items || [],
        totalCount: response.data?.totalCount || 0,
        page: response.data?.page || 1,
        pageSize: response.data?.pageSize || 10,
        totalPages: response.data?.totalPages || 0,
        hasNext: response.data?.hasNext || false,
        hasPrev: response.data?.hasPrev || false
      }))
    );
  }

  getProductById(id: string | number): Observable<IProduct> {
    return this.http.get<{ success: boolean; data: IProduct; message?: string }>(API_ENDPOINTS.PRODUCT.GET_BY_ID(id)).pipe(
      map(response => response.data)
    );
  }
}
