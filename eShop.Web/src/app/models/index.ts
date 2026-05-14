export interface LoginRequest {
  username: string;
  password: string;
}

export interface RegisterRequest {
  userName: string;
  email: string;
  userCode: string;
}

export interface ApiResponse<T> {
  success: boolean;
  data?: T;
  message?: string;
  errors?: string[];
  statusCode: number;
}

export interface AuthResponse {
  accessToken: string;
  email: string;
  role: string;
  expiresAt: string;
}

export interface UserDto {
  id: number;
  userName: string;
  fullName?: string;
  email?: string;
  userType?: number;
  status?: number;
}

export interface ProductDto {
  id: number;
  name: string;
  description?: string;
  price: number;
  stock: number;
  categoryName: string;
  ai_Description?: string;
  ai_Generated: boolean;
}

export interface CreateProductDto {
  name: string;
  description: string;
  price: number;
  stock: number;
  categoryId: number;
}

export interface PaginatedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
  hasNext: boolean;
  hasPrev: boolean;
}
