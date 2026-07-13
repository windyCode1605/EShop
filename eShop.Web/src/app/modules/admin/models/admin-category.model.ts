/**
 * ─────────────────────────────────────────────
 * ADMIN CATEGORY MODELS
 * Skeleton interfaces — sẵn sàng kết nối API
 * ─────────────────────────────────────────────
 */

// ── Full Admin Category ───────────────────────────────────────────────────────
export interface IAdminCategory {
  categoryId: number;
  categoryName: string;
  slug: string;
  description?: string;
  imageUrl?: string;
  parentId?: number | null;
  parentName?: string;
  isActive: boolean;
  sortOrder: number;
  productCount?: number;
  createdAt?: string | Date;
  updatedAt?: string | Date;
}

// ── DTOs ──────────────────────────────────────────────────────────────────────
export interface ICategoryCreateDto {
  categoryName: string;
  slug: string;
  description?: string;
  imageUrl?: string;
  parentId?: number | null;
  isActive: boolean;
  sortOrder: number;
}

export interface ICategoryUpdateDto extends Partial<ICategoryCreateDto> { }

// ── Paginated Response ─────────────────────────────────────────────────────────
export interface ICategoryListResponse {
  items: IAdminCategory[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}
