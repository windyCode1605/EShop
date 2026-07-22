/**
 * ─────────────────────────────────────────────
 * ADMIN PRODUCT MODELS
 * Skeleton interfaces — sẵn sàng kết nối API
 * ─────────────────────────────────────────────
 */

// ── Attribute Value (e.g. Red, XL, Cotton) ──────────────────────────────────
export interface IAttributeValue {
  id: number;
  attributeId: number;
  value: string;   // raw value, e.g. "#FF0000"
  label: string;   // display label, e.g. "Red"
}

// ── Product Attribute (e.g. Color, Size, Material) ───────────────────────────
export type AttributeType = 'Text' | 'Color' | 'Number' | 'Boolean';

export interface IProductAttribute {
  id: number;
  productId?: number | string;  // nếu là attribute gắn với product cụ thể
  name: string;           // "Color", "Size", "Material"
  type: AttributeType;
  isRequired?: boolean;
  values: IAttributeValue[];
}

// ── Product Image ─────────────────────────────────────────────────────────────
export interface IProductImage {
  id: number;
  productId: number | string;
  imageUrl: string;
  altText?: string;
  sortOrder: number;
  isPrimary: boolean;
  createdAt?: string | Date;
}

// ── Product Variant Attribute ─────────────────────────────────────────────────
export interface IProductVariantAttribute {
  id?: number;
  variantId: number;
  attributeId: number;
  attributeName: string;
  attributeType: AttributeType;
  attributeValueId?: number;
  attributeValue?: string;  // from IAttributeValue.label
  customValue?: string;     // khi nhập tự do không qua predefined values
  displayValue: string;     // = attributeValue ?? customValue
}

// ── Product Variant (SKU cụ thể) ──────────────────────────────────────────────
export interface IAdminProductVariant {
  id: number;
  productId: number | string;
  sku: string;
  priceAdjustment: number;   // giá điều chỉnh so với base price
  stockQuantity: number;
  isDefault: boolean;
  isActive: boolean;
  imageUrls?: string[];         // mảng ảnh riêng của variant
  attributes: IProductVariantAttribute[];
}

// ── Full Admin Product ─────────────────────────────────────────────────────────
export interface IAdminProduct {
  id: number | string;
  name: string;
  description: string;
  price: number;
  stock: number;
  sku?: string;
  categoryId?: number;
  categoryName: string;
  isActive: boolean;
  aI_Description?: string;
  aI_Generated?: boolean;
  createdAt: string | Date;
  updatedAt?: string | Date;
  // Sub-entities
  images: IProductImage[];
  attributes: IProductAttribute[];
  variants: IAdminProductVariant[];
}

// ── DTOs ──────────────────────────────────────────────────────────────────────
export interface IProductCreateDto {
  name: string;
  description: string;
  price: number;
  stock: number;
  sku?: string;
  categoryId?: number;
  isActive: boolean;
  aI_Description?: string;
}

export interface IProductUpdateDto extends Partial<IProductCreateDto> { }

export interface IProductImageCreateDto {
  productId: number | string;
  imageUrl: string;
  altText?: string;
  sortOrder: number;
  isPrimary: boolean;
}

export interface IProductAttributeCreateDto {
  name: string;
  type: AttributeType;
  isRequired?: boolean;
  values: { value: string; label: string }[];
}

export interface IProductVariantCreateDto {
  productId: number | string;
  sku: string;
  priceAdjustment: number;
  stockQuantity: number;
  isDefault: boolean;
  isActive: boolean;
  imageUrls?: string[];
  attributes: {
    attributeId: number;
    attributeValueId?: number;
    customValue?: string;
  }[];
}

export interface IProductVariantUpdateDto extends Partial<IProductVariantCreateDto> { }

// ── Admin Product Filter ───────────────────────────────────────────────────────
export interface IAdminProductFilter {
  keyword?: string;
  categoryId?: number | string;
  isActive?: boolean;
  minPrice?: number;
  maxPrice?: number;
  pageIndex: number;
  pageSize: number;
  sortBy?: string;
  sortOrder?: 'asc' | 'desc';
}
