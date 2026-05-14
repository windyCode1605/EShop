/**
 * Product Manager Constants
 */

export const PRODUCT_MANAGER_ROUTES = {
  LIST: '/product-manager',
  DETAIL: '/product-manager/detail',
  CREATE: '/product-manager/create',
  EDIT: '/product-manager/edit'
};

export const PRODUCT_DEFAULTS = {
  PAGE_SIZE: 10,
  PAGE_INDEX: 1,
  MIN_PRICE: 0,
  MAX_PRICE: 999999,
  STOCK_THRESHOLD_LOW: 100
};

export const PRODUCT_VALIDATION = {
  NAME_MIN_LENGTH: 3,
  NAME_MAX_LENGTH: 255,
  DESCRIPTION_MIN_LENGTH: 10,
  DESCRIPTION_MAX_LENGTH: 2000,
  SKU_MIN_LENGTH: 3
};

export const PRODUCT_STATUS = {
  ACTIVE: 'Active',
  INACTIVE: 'Inactive'
};

export const STOCK_STATUS = {
  IN_STOCK: 'In Stock',
  LOW_STOCK: 'Low Stock',
  OUT_OF_STOCK: 'Out of Stock'
};

export const SORT_OPTIONS = [
  { value: 'name', label: 'Name' },
  { value: 'price', label: 'Price' },
  { value: 'stock', label: 'Stock' },
  { value: 'createdAt', label: 'Created Date' }
];
