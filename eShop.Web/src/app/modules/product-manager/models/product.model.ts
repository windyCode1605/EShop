/**
 * Thuộc tính động của một variant (Color: Red, Size: XL, Material: Cotton...)
 */
export interface IVariantAttribute {
  attributeId: number;
  attributeName: string;
  attributeType: string; // 'Text' | 'Color' | 'Number' | 'Boolean'
  attributeValueId?: number;
  attributeValue?: string;
  customValue?: string;
  displayValue: string; // = attributeValue ?? customValue
}

/**
 * Một biến thể sản phẩm (SKU cụ thể, Size, Color, Giá điều chỉnh, Tồn kho)
 */
export interface IProductVariant {
  id: number;
  sku: string;
  size?: string;
  color?: string;
  priceAdjustment: number;
  stockQuantity: number;
  imageUrls?: string[];
  attributes: IVariantAttribute[];
}

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
  categoryId?: number;
  categoryName: string;
  createdAt: string | Date;
  aI_Description?: string;
  aI_Generated?: boolean;
  sku?: string;
  image?: string;
  imageUrls?: string[];
  isActive?: boolean;
  updatedAt?: string | Date;
  variants?: IProductVariant[];
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
  imageUrls?: string[];
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
