import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { IProduct, ProductResponseDto, ProductCreateUpdateDto, ApiPaginatedResponse } from '../models/product.model';
import { ProductFilterModel } from '../models/product-filter.model';

/**
 * Product HTTP Service
 * Handles all API calls for product management
 * API: http://localhost:5178/api/Product
 */
@Injectable({
  providedIn: 'root'
})
export class ProductHttpService {
  private apiUrl = 'http://localhost:5178/api/Product'; // Backend API URL

  constructor(private http: HttpClient) {}

  /**
   * Get all products with pagination and filtering
   * Maps frontend filter model to backend API parameters
   */
  getProducts(filter: ProductFilterModel): Observable<ProductResponseDto> {
    let params = new HttpParams()
      .set('page', filter.pageIndex.toString())
      .set('size', filter.pageSize.toString());

    if (filter.keyword) {
      params = params.set('keyword', filter.keyword);
    }
    if (filter.categoryId) {
      params = params.set('categoryId', filter.categoryId);
    }
    if (filter.minPrice !== undefined) {
      params = params.set('minPrice', filter.minPrice.toString());
    }
    if (filter.maxPrice !== undefined) {
      params = params.set('maxPrice', filter.maxPrice.toString());
    }
    if (filter.isActive !== undefined) {
      params = params.set('isActive', filter.isActive.toString());
    }
    if (filter.sortBy) {
      params = params.set('sortBy', filter.sortBy);
    }
    if (filter.sortOrder) {
      params = params.set('sortOrder', filter.sortOrder);
    }

    // Map API response structure to our DTO
    return this.http.get<ApiPaginatedResponse<IProduct>>(this.apiUrl, { params }).pipe(
      map(response => ({
        items: response.data.items,
        totalCount: response.data.totalCount,
        page: response.data.page,
        pageSize: response.data.pageSize,
        totalPages: response.data.totalPages,
        hasNext: response.data.hasNext,
        hasPrev: response.data.hasPrev
      }))
    );
  }

  /**
   * Get product by ID
   */
  getProductById(id: string | number): Observable<IProduct> {
    return this.http.get<IProduct>(`${this.apiUrl}/${id}`);
  }

  /**
   * Create new product
   */
  createProduct(product: ProductCreateUpdateDto): Observable<IProduct> {
    return this.http.post<IProduct>(this.apiUrl, product);
  }

  /**
   * Update product
   */
  updateProduct(id: string | number, product: ProductCreateUpdateDto): Observable<IProduct> {
    return this.http.put<IProduct>(`${this.apiUrl}/${id}`, product);
  }

  /**
   * Delete product
   */
  deleteProduct(id: string | number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  /**
   * Search products by keyword
   */
  searchProducts(keyword: string, pageIndex: number = 1, pageSize: number = 10): Observable<ProductResponseDto> {
    const params = new HttpParams()
      .set('keyword', keyword)
      .set('page', pageIndex.toString())
      .set('size', pageSize.toString());

    return this.http.get<ApiPaginatedResponse<IProduct>>(this.apiUrl, { params }).pipe(
      map(response => ({
        items: response.data.items,
        totalCount: response.data.totalCount,
        page: response.data.page,
        pageSize: response.data.pageSize,
        totalPages: response.data.totalPages,
        hasNext: response.data.hasNext,
        hasPrev: response.data.hasPrev
      }))
    );
  }

  /**
   * Get products by category
   */
  getProductsByCategory(categoryName: string, pageIndex: number = 1, pageSize: number = 10): Observable<ProductResponseDto> {
    const params = new HttpParams()
      .set('categoryName', categoryName)
      .set('page', pageIndex.toString())
      .set('size', pageSize.toString());

    return this.http.get<ApiPaginatedResponse<IProduct>>(this.apiUrl, { params }).pipe(
      map(response => ({
        items: response.data.items,
        totalCount: response.data.totalCount,
        page: response.data.page,
        pageSize: response.data.pageSize,
        totalPages: response.data.totalPages,
        hasNext: response.data.hasNext,
        hasPrev: response.data.hasPrev
      }))
    );
  }
}
