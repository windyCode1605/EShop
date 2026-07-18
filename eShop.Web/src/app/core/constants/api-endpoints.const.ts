/**
 * Helper function to generate standard CRUD endpoints for a given base path.
 */
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
    PRODUCT: {
        ...createCrudEndpoints('/api/Product'),
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
    },

    ADDRESS: {
        ...createCrudEndpoints('/api/Address'),
    },

    CART: {
        ...createCrudEndpoints('/api/Cart'),
        ADD_TO_CART: '/api/Cart/add-to-cart',
        GET_MY_CART: '/api/Cart/get-my-cart',
        UPDATE_ITEM: '/api/Cart/update-item',
        REMOVE_ITEM: '/api/Cart/remove-item',
    },

    // ---------------------------------------------------------
    // AUTH & USER ENDPOINTS
    // ---------------------------------------------------------
    AUTH: {
        REGISTER: '/api/auth/register',
        VERIFY_OTP: '/api/auth/verify-otp',
        SET_PASSWORD: '/api/auth/set-password',
    },

    ME: {
        PERMISSIONS: '/api/me/permissions',
    },

    // ---------------------------------------------------------
    // ADMIN ENDPOINTS
    // ---------------------------------------------------------
    ADMIN: {
        PRODUCT: {
            ...createCrudEndpoints('/api/Admin/Product'),
        },
        PRODUCT_IMAGE: {
            ...createCrudEndpoints('/api/Admin/ProductImage'),
            UPLOAD: '/api/Admin/ProductImage/upload',
        },
        PRODUCT_ATTRIBUTE: {
            ...createCrudEndpoints('/api/Admin/ProductAttribute'),
        },
        PRODUCT_VARIANT: {
            ...createCrudEndpoints('/api/Admin/ProductVariant'),
        },
        ORDER: {
            ...createCrudEndpoints('/api/admin/orders'),
            UPDATE_STATUS: (id: string | number) => `/api/admin/orders/${id}/status`,
        },
    },
} as const;
