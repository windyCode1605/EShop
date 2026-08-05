
function createCrudEndpoints(basePath: string) {
    return {
        BASE: basePath,
        GET_ALL: basePath,
        GET_BY_ID: (id: string | number) => `${basePath}/${id}`,
        CREATE: basePath,
        UPDATE: (id: string | number) => `${basePath}/${id}`,
        DELETE: (id: string | number) => `${basePath}/${id}`,
    };
}

export const API_ENDPOINTS = {
    // PUBLIC / CUSTOMER ENDPOINTS
    ADDRESS: {
        ...createCrudEndpoints('/api/Address'),
        SET_DEFAULT: (id: string | number) => `/api/Address/${id}/default`,
    },

    CUSTOMER: {
        ME: '/api/Customer/me',
    },

    CART: {
        ...createCrudEndpoints('/api/Cart'),
        ADD_TO_CART: '/api/Cart/add-to-cart',
        GET_MY_CART: '/api/Cart/get-my-cart',
        UPDATE_ITEM: '/api/Cart/update-item',
        REMOVE_ITEM: '/api/Cart/remove-item',
        CLEAR: '/api/Cart/clear',
        VALIDATE: '/api/Cart/validate',
        CHECKOUT_PREVIEW: '/api/Cart/checkout-preview',
    },

    CATEGORY: {
        ...createCrudEndpoints('/api/Category'),
        GET_ALL: '/api/Category/getCategory',
        TOGGLE_ACTIVE: (id: string | number) => `/api/Category/${id}/toggle-active`,
    },

    ORDER: {
        ...createCrudEndpoints('/api/Order'),
        CREATE: '/api/Order/create',
        MY_ORDERS: '/api/Order/my-orders',
        CANCEL: (id: string | number) => `/api/Order/${id}/cancel`,
    },

    PAYMENT: {
        GET_BY_ORDER_ID: (orderId: string | number) => `/api/payments/order/${orderId}`,
        CREATE_URL: (orderId: string | number) => `/api/payments/order/${orderId}/create-url`,
        GATEWAY_CALLBACK: '/api/payments/gateway/callback',
        CONFIRM_BANK_TRANSFER: (orderId: string | number) => `/api/payments/order/${orderId}/confirm-bank-transfer`,
        REFUND: (orderId: string | number) => `/api/payments/order/${orderId}/refund`,
    },

    PRODUCT: {
        ...createCrudEndpoints('/api/Product'),
        VARIANT_CREATE: '/api/Product/variants',
        VARIANT_UPDATE: (id: string | number) => `/api/Product/variants/${id}`,
    },

    SHIPMENT: {
        GET_BY_ORDER_ID: (orderId: string | number) => `/api/shipments/order/${orderId}`,
        WEBHOOK: '/api/shipments/webhook',
        TRACKING: '/api/shipments/tracking',
        UPDATE_STATUS: (shipmentId: string | number) => `/api/shipments/${shipmentId}/status`,
    },

    // AUTH & USER ENDPOINTS
    AUTH: {
        REGISTER: '/api/auth/register',
        VERIFY_OTP: '/api/auth/verify-otp',
        SET_PASSWORD: '/api/auth/set-password',
        FORGOT_PASSWORD: '/api/auth/forgot-password',
        VERIFY_RESET_OTP: '/api/auth/verify-reset-otp',
        RESET_PASSWORD: '/api/auth/reset-password',
        CONNECT_AUTHORIZE: '/connect/authorize',
        AUTHENTICATE_LOGIN: '/authenticate/login',
        CONNECT_TOKEN: '/connect/token',
    },

    ME: {
        PERMISSIONS: '/api/me/permissions',
    },

    // ADMIN ENDPOINTS
    ADMIN: {
        ATTRIBUTE: {
            ...createCrudEndpoints('/api/attribute/AdminAttribute'),
            GET_VALUE: '/api/attribute/AdminAttribute/value',
            POST_VALUE: '/api/attribute/AdminAttribute/value',
        },
        ORDER: {
            ...createCrudEndpoints('/api/admin/orders'),
            UPDATE_STATUS: (id: string | number) => `/api/admin/orders/${id}/status`,
        },
        ROLE: {
            ...createCrudEndpoints('/api/admin/roles'),
            GET_PERMISSIONS: (roleId: string | number) => `/api/admin/roles/${roleId}/permissions`,
            UPDATE_PERMISSIONS: (roleId: string | number) => `/api/admin/roles/${roleId}/permissions`,
            GET_ALL_PERMISSIONS: '/api/admin/roles/all-permissions',
        },
        USER: {
            UPDATE_ROLES: (userId: string | number) => `/api/admin/users/${userId}/roles`,
            TEST_PERMISSION: '/api/admin/users/test-permission',
        },

        // Vẫn giữ lại để không vỡ service cũ (AdminProductService đang dùng)
        PRODUCT: {
            ...createCrudEndpoints('/api/Product'),
        },
        PRODUCT_IMAGE: {
            ...createCrudEndpoints('/api/admin/ProductImage'),
            UPLOAD: '/api/admin/ProductImage/upload',
        },
        PRODUCT_ATTRIBUTE: {
            ...createCrudEndpoints('/api/admin/ProductAttribute'),
        },
        PRODUCT_VARIANT: {
            ...createCrudEndpoints('/api/admin/ProductVariant'),
        },
        CUSTOMER: {
            ...createCrudEndpoints('/api/admin/customers'),
            STATISTICS: (id: string | number) => `/api/admin/customers/${id}/statistics`,
            LOCK: (id: string | number) => `/api/admin/customers/${id}/lock`,
            UNLOCK: (id: string | number) => `/api/admin/customers/${id}/unlock`,
        },
    },
} as const;
