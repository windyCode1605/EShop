import { Injectable, signal } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, of, throwError } from 'rxjs';
import { API_ENDPOINTS } from '../../../core/constants/api-endpoints.const';
import { map } from 'rxjs/operators';
import { ApiResponse } from '../../../core/models';
import {
  IAdminProduct,
  IProductCreateDto,
  IProductUpdateDto,
  IProductImage,
  IProductImageCreateDto,
  IProductAttribute,
  IProductAttributeCreateDto,
  IAdminProductVariant,
  IProductVariantCreateDto,
  IProductVariantUpdateDto,
  IAdminProductFilter
} from '../models/admin-product.model';

export interface AdminProductListResponse {
  items: IAdminProduct[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
  hasNext: boolean;
  hasPrev: boolean;
}

@Injectable({ providedIn: 'root' })
export class AdminProductService {
  // Local state — signal-based
  products = signal<IAdminProduct[]>([]);
  loading = signal<boolean>(false);

  constructor(private http: HttpClient) { }

  // ── Products CRUD ─────────────────────────────────────────────────────────

  /**
   * GET /api/Admin/Product — paginated list with filters
   * TODO: Kết nối khi backend sẵn sàng
   */
  getProducts(filter: IAdminProductFilter): Observable<AdminProductListResponse> {
    let params = new HttpParams()
      .set('page', filter.pageIndex.toString())
      .set('size', filter.pageSize.toString());

    if (filter.keyword) params = params.set('keyword', filter.keyword);
    if (filter.categoryId !== undefined) params = params.set('categoryId', filter.categoryId.toString());
    if (filter.isActive !== undefined) params = params.set('isActive', filter.isActive.toString());
    if (filter.minPrice !== undefined) params = params.set('minPrice', filter.minPrice.toString());
    if (filter.maxPrice !== undefined) params = params.set('maxPrice', filter.maxPrice.toString());
    if (filter.sortBy) params = params.set('sortBy', filter.sortBy);
    if (filter.sortOrder) params = params.set('sortOrder', filter.sortOrder);

    return this.http.get<ApiResponse<AdminProductListResponse>>(API_ENDPOINTS.ADMIN.PRODUCT.GET_ALL, { params })
      .pipe(map(res => res.data!));
  }

  /**
   * GET /api/Admin/Product/:id — full product detail with images, attributes, variants
   * TODO: Kết nối khi backend sẵn sàng
   */
  getProductById(id: number | string): Observable<IAdminProduct> {
    return this.http.get<ApiResponse<IAdminProduct>>(API_ENDPOINTS.ADMIN.PRODUCT.GET_BY_ID(id))
      .pipe(map(res => res.data!));
  }

  /**
   * POST /api/Product — Luồng 1: Tạo Product + Variants cùng 1 lần.
   * Variants là optional — có thể gửi mảng rỗng nếu muốn tạo variant riêng sau.
   */
  createProduct(dto: IProductCreateDto): Observable<IAdminProduct> {
    return this.http.post<ApiResponse<IAdminProduct>>(API_ENDPOINTS.PRODUCT.CREATE, dto)
      .pipe(map(res => res.data!));
  }

  /**
   * PUT /api/Admin/Product/:id — cập nhật sản phẩm
   * TODO: Kết nối khi backend sẵn sàng
   */
  updateProduct(id: number | string, dto: IProductUpdateDto): Observable<IAdminProduct> {
    return this.http.put<ApiResponse<IAdminProduct>>(API_ENDPOINTS.ADMIN.PRODUCT.UPDATE(id), dto)
      .pipe(map(res => res.data!));
  }

  /**
   * DELETE /api/Admin/Product/:id — xóa sản phẩm
   * TODO: Kết nối khi backend sẵn sàng
   */
  deleteProduct(id: number | string): Observable<void> {
    return this.http.delete<ApiResponse<void>>(API_ENDPOINTS.ADMIN.PRODUCT.DELETE(id))
      .pipe(map(res => res.data!));
  }

  // ── Product Images ─────────────────────────────────────────────────────────

  /**
   * GET /api/Admin/ProductImage?productId=:id — danh sách ảnh của product
   * TODO: Kết nối khi backend sẵn sàng
   */
  getProductImages(productId: number | string): Observable<IProductImage[]> {
    const params = new HttpParams().set('productId', productId.toString());
    return this.http.get<IProductImage[]>(API_ENDPOINTS.ADMIN.PRODUCT_IMAGE.GET_ALL, { params });
  }

  /**
   * POST /api/Admin/ProductImage — thêm ảnh mới
   * TODO: Kết nối khi backend sẵn sàng
   */
  addProductImage(dto: IProductImageCreateDto): Observable<IProductImage> {
    return this.http.post<IProductImage>(API_ENDPOINTS.ADMIN.PRODUCT_IMAGE.CREATE, dto);
  }

  /**
   * PUT /api/Admin/ProductImage/:id — cập nhật ảnh (set primary, sort order)
   * TODO: Kết nối khi backend sẵn sàng
   */
  updateProductImage(id: number, dto: Partial<IProductImageCreateDto>): Observable<IProductImage> {
    return this.http.put<IProductImage>(API_ENDPOINTS.ADMIN.PRODUCT_IMAGE.UPDATE(id), dto);
  }

  /**
   * DELETE /api/Admin/ProductImage/:id — xóa ảnh
   * TODO: Kết nối khi backend sẵn sàng
   */
  deleteProductImage(id: number): Observable<void> {
    return this.http.delete<void>(API_ENDPOINTS.ADMIN.PRODUCT_IMAGE.DELETE(id));
  }

  /**
   * POST /api/Admin/ProductImage/upload — upload file ảnh
   * TODO: Kết nối khi backend sẵn sàng
   */
  uploadImage(productId: number | string, file: File): Observable<{ url: string }> {
    const formData = new FormData();
    formData.append('file', file);
    formData.append('productId', productId.toString());
    return this.http.post<{ url: string }>(API_ENDPOINTS.ADMIN.PRODUCT_IMAGE.UPLOAD, formData);
  }

  // ── Product Attributes ─────────────────────────────────────────────────────

  /**
   * GET /api/Admin/ProductAttribute?productId=:id
   * TODO: Kết nối khi backend sẵn sàng
   */
  getProductAttributes(productId: number | string): Observable<IProductAttribute[]> {
    const params = new HttpParams().set('productId', productId.toString());
    return this.http.get<IProductAttribute[]>(API_ENDPOINTS.ADMIN.PRODUCT_ATTRIBUTE.GET_ALL, { params });
  }

  /**
   * POST /api/Admin/ProductAttribute
   * TODO: Kết nối khi backend sẵn sàng
   */
  createProductAttribute(productId: number | string, dto: IProductAttributeCreateDto): Observable<IProductAttribute> {
    return this.http.post<IProductAttribute>(API_ENDPOINTS.ADMIN.PRODUCT_ATTRIBUTE.CREATE, { ...dto, productId });
  }

  /**
   * PUT /api/Admin/ProductAttribute/:id
   * TODO: Kết nối khi backend sẵn sàng
   */
  updateProductAttribute(id: number, dto: Partial<IProductAttributeCreateDto>): Observable<IProductAttribute> {
    return this.http.put<IProductAttribute>(API_ENDPOINTS.ADMIN.PRODUCT_ATTRIBUTE.UPDATE(id), dto);
  }

  /**
   * DELETE /api/Admin/ProductAttribute/:id
   * TODO: Kết nối khi backend sẵn sàng
   */
  deleteProductAttribute(id: number): Observable<void> {
    return this.http.delete<void>(API_ENDPOINTS.ADMIN.PRODUCT_ATTRIBUTE.DELETE(id));
  }

  // ── Product Variants ───────────────────────────────────────────────────────

  /**
   * GET /api/Admin/ProductVariant?productId=:id
   * TODO: Kết nối khi backend sẵn sàng
   */
  getProductVariants(productId: number | string): Observable<IAdminProductVariant[]> {
    const params = new HttpParams().set('productId', productId.toString());
    return this.http.get<IAdminProductVariant[]>(API_ENDPOINTS.ADMIN.PRODUCT_VARIANT.GET_ALL, { params });
  }

  /**
   * POST /api/Product/variants — Luồng 2: Tạo Variant độc lập cho Product đã có.
   * dto.productId là bắt buộc.
   */
  createProductVariant(dto: IProductVariantCreateDto): Observable<IAdminProductVariant> {
    return this.http.post<ApiResponse<IAdminProductVariant>>(API_ENDPOINTS.PRODUCT.VARIANT_CREATE, dto)
      .pipe(map(res => res.data!));
  }

  /**
   * PUT /api/Admin/ProductVariant/:id — cập nhật variant
   * TODO: Kết nối khi backend sẵn sàng
   */
  updateProductVariant(id: number, dto: IProductVariantUpdateDto): Observable<IAdminProductVariant> {
    return this.http.put<IAdminProductVariant>(API_ENDPOINTS.ADMIN.PRODUCT_VARIANT.UPDATE(id), dto);
  }

  /**
   * DELETE /api/Admin/ProductVariant/:id — xóa variant
   * TODO: Kết nối khi backend sẵn sàng
   */
  deleteProductVariant(id: number): Observable<void> {
    return this.http.delete<void>(API_ENDPOINTS.ADMIN.PRODUCT_VARIANT.DELETE(id));
  }
}
