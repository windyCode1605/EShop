/**
 * Product Model
 * Represents a product entity in the system
 * Matches backend API structure
 */
export interface IProduct {
  id: number | string;
  name: string;
  description: string;
  price: number;
  stock: number;
  categoryName: string;
  createdAt: string | Date;
  aI_Description?: string;
  aI_Generated?: boolean;
  sku?: string;
  image?: string;
  isActive?: boolean;
  updatedAt?: string | Date;
}

/**
 * Product DTO for API communication
 */
export class ProductModel implements IProduct {
  id: number | string = '';
  name: string = '';
  description: string = '';
  price: number = 0;
  stock: number = 0;
  categoryName: string = '';
  createdAt: string | Date = new Date();
  aI_Description?: string;
  aI_Generated?: boolean;
  sku?: string;
  image?: string;
  isActive?: boolean;
  updatedAt?: string | Date;

  constructor(data?: Partial<IProduct>) {
    if (data) {
      Object.assign(this, data);
    }
  }
}

/**
 * Backend API Response Structure
 * Wraps paginated product data
 */
export interface ApiPaginatedResponse<T> {
  success: boolean;
  data: {
    items: T[];
    totalCount: number;
    page: number;
    pageSize: number;
    totalPages: number;
    hasNext: boolean;
    hasPrev: boolean;
  };
  message?: string;
  errors?: string[];
  statusCode: number;
}

/**
 * Product Response DTO - matches API format
 */
export interface ProductResponseDto {
  items: IProduct[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
  hasNext: boolean;
  hasPrev: boolean;
}

/**
 * Product Create/Update Request DTO
 */
export interface ProductCreateUpdateDto {
  name: string;
  description: string;
  price: number;
  stock: number;
  categoryName?: string;
  sku?: string;
  image?: string;
  isActive?: boolean;
  aI_Description?: string;
}
