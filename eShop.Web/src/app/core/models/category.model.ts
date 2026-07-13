export interface ICategory {
  categoryId: number;
  categoryName: string;
  slug?: string;
  description?: string;
  imageUrl?: string;
  parentId?: number | null;
  parentName?: string;
  isActive?: boolean;
  sortOrder?: number;
  productCount?: number;
  createdAt?: string | Date;
  updatedAt?: string | Date;
}

