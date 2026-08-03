export interface IAdminCustomerFilter {
    pageIndex: number;
    pageSize: number;
    keyword?: string;
    isActive?: boolean;
    customerSegment?: string;
    fromDate?: string;
    toDate?: string;
    minSpent?: number;
    maxSpent?: number;
}

export interface ICustomerListItem {
    id: number;
    username: string;
    fullName: string;
    email: string;
    phone: string;
    initials: string;
    isActive: boolean;
    totalOrders: number;
    totalSpent: number;
    createdAt?: string;
    lastLoginDate?: string;
}

export interface AdminCustomerListResponse {
    items: ICustomerListItem[];
    totalCount: number;
    page: number;
    pageSize: number;
    totalPages: number;
    hasNext: boolean;
    hasPrev: boolean;
}
