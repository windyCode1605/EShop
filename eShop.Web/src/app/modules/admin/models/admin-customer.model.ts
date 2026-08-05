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

export interface ICustomerAddressItem {
    id: number;
    receiverName: string;
    phone: string;
    fullAddress: string;
    isDefault: boolean;
}

export interface ICustomerRecentOrder {
    orderId: number;
    orderCode: string;
    orderDate: string;
    totalAmount: number;
    orderStatus: string;
}

export interface ICustomerDetail extends ICustomerListItem {
    addresses: ICustomerAddressItem[];
}

export interface ICustomerStatistics {
    totalOrders: number;
    totalSpent: number;
    recentOrders: ICustomerRecentOrder[];
}
