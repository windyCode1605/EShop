/**
 * Product Filter Model
 * Used for filtering and paging product list
 */
export interface IProductFilter {
  keyword?: string;
  categoryId?: string;
  minPrice?: number;
  maxPrice?: number;
  isActive?: boolean;
  pageIndex: number;
  pageSize: number;
  sortBy?: string;
  sortOrder?: 'asc' | 'desc';
}

/**
 * Product Filter Implementation
 */
export class ProductFilterModel implements IProductFilter {
  keyword?: string;
  categoryId?: string;
  minPrice?: number;
  maxPrice?: number;
  isActive: boolean = true;
  pageIndex: number = 1;
  pageSize: number = 10;
  sortBy?: string = 'name';
  sortOrder?: 'asc' | 'desc' = 'asc';

  constructor(data?: Partial<IProductFilter>) {
    if (data) {
      Object.assign(this, data);
    }
  }

  /**
   * Reset filter to default values
   */
  reset(): void {
    this.keyword = undefined;
    this.categoryId = undefined;
    this.minPrice = undefined;
    this.maxPrice = undefined;
    this.isActive = true;
    this.pageIndex = 1;
    this.pageSize = 10;
    this.sortBy = 'name';
    this.sortOrder = 'asc';
  }

  /**
   * Check if any filter is applied
   */
  hasFilters(): boolean {
    return !!(
      this.keyword ||
      this.categoryId ||
      this.minPrice ||
      this.maxPrice ||
      !this.isActive
    );
  }
}
