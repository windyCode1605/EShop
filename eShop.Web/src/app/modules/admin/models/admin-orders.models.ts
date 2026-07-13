export interface IAdminOrderItem {
    id: number;
    productName: string;
    variantSKU: string;
    quantity: number;
    unitPrice: number;
    lineTotal: number;
}

export interface IAdminOrderPayment {
    id: number;
    orderId: number;
    method: string;
    status: string;
    amount: number;
    paidAt: string | null;
    transactionId: string | null;
    gatewayResponseCode: string | null;
    paymentUrl: string | null;
    createdAt: string | null;
    refundedAmount: number | null;
    refundedAt: string | null;
    refundReason: string | null;
}

export interface IAdminOrderShipment {
    id: number;
    orderId: number;
    shippingProvider: string;
    trackingNumber: string | null;
    shippingFee: number;
    receiverName: string;
    receiverPhone: string;
    shippingAddress: string;
    status: string;
    estimatedDelivery: string | null;
    actualDelivery: string | null;
    createdDate: string;
}

export interface IAdminOrder {
    id: number;
    orderCode: string;
    status: string;
    paymentMethod: string;
    subtotal: number;
    discountAmount: number;
    shippingFee: number;
    totalAmount: number;
    shippingAddress: string;
    note: string | null;
    createdDate: string;
    items: IAdminOrderItem[];
    payment: IAdminOrderPayment | null;
    shipment: IAdminOrderShipment | null;
}


export interface IAdminOrderPagedResult {
    items: IAdminOrder[];
    totalItems: number;
    totalPages: number | null;
    currentPage: number | null;
    pageSize: number | null;
    hasNextPage: boolean;
    hasPreviousPage: boolean;
}