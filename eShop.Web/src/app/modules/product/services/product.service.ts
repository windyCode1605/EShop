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
      .set('pageNumber', filter.pageIndex.toString())
      .set('pageSize', filter.pageSize.toString());

    if (filter.keyword) params = params.set('keyword', filter.keyword);
    if (filter.categoryId) params = params.set('categoryId', filter.categoryId.toString());
    if (filter.minPrice !== undefined) params = params.set('minPrice', filter.minPrice.toString());
    if (filter.maxPrice !== undefined) params = params.set('maxPrice', filter.maxPrice.toString());

    // params = params.set('isActive', 'true');

    if (filter.sortBy) {
      let sortByBackend = filter.sortBy === 'price' ? 'BasePrice' : filter.sortBy;
      if (sortByBackend === 'createdAt') sortByBackend = 'CreatedDate';
      params = params.set('sortBy', `${sortByBackend} ${filter.sortOrder ?? 'asc'}`);
    }

    return this.http.get<ApiPaginatedResponse<IProduct>>(API_ENDPOINTS.PRODUCT.GET_ALL, { params }).pipe(
      map(response => {
        const data: any = response.data;
        return {
          items: data?.items || [],
          totalCount: data?.totalItems || 0,
          page: data?.currentPage || 1,
          pageSize: data?.pageSize || 10,
          totalPages: data?.totalPages || 0,
          hasNext: data?.hasNextPage || false,
          hasPrev: data?.hasPreviousPage || false
        };
      })
    );
  }

  getProductById(id: string | number): Observable<IProduct> {
    return this.http.get<{ success: boolean; data: IProduct; message?: string }>(API_ENDPOINTS.PRODUCT.GET_BY_ID(id)).pipe(
      map(response => response.data)
    );
  }
}
