/**
 * PERMISSIONS CONSTANT — Định nghĩa tập trung tất cả mã quyền trong hệ thống.
 * Giúp IDE Auto-complete, tránh gõ sai chuỗi string và dễ refactor.
 */
export const PERMISSIONS = {
    DASHBOARD: {
        VIEW: 'Dashboard.View',
    },
    ANALYTICS: {
        REPORTS_VIEW: 'Analytics.Reports.View',
        REPORTS_EXPORT: 'Analytics.Reports.Export',
    },
    SYSTEM: {
        SETTINGS_VIEW: 'System.Settings.View',
        SETTINGS_UPDATE: 'System.Settings.Update',
    },
    CATALOG: {
        REVIEWS_VIEW: 'Catalog.Reviews.View',
        REVIEWS_REPLY: 'Catalog.Reviews.Reply',
        REVIEWS_MODERATE: 'Catalog.Reviews.Moderate',

        CATEGORIES_VIEW: 'Catalog.Categories.View',
        CATEGORIES_CREATE: 'Catalog.Categories.Create',
        CATEGORIES_UPDATE: 'Catalog.Categories.Update',
        CATEGORIES_DELETE: 'Catalog.Categories.Delete',

        PRODUCTS_VIEW: 'Catalog.Products.View',
        PRODUCTS_CREATE: 'Catalog.Products.Create',
        PRODUCTS_UPDATE: 'Catalog.Products.Update',
        PRODUCTS_DELETE: 'Catalog.Products.Delete',
    },
    SALES: {
        ORDERS_VIEW: 'Sales.Orders.View',
        ORDERS_PROCESS: 'Sales.Orders.Process',
        ORDERS_CANCEL: 'Sales.Orders.Cancel',
        ORDERS_REFUND: 'Sales.Orders.Refund',
    },
    CUSTOMERS: {
        VIEW: 'Customers.View',
        MANAGE: 'Customers.Manage',
    },
    INVENTORY: {
        STOCK_VIEW: 'Inventory.Stock.View',
        STOCK_UPDATE: 'Inventory.Stock.Update',
    },
    MARKETING: {
        COUPONS_VIEW: 'Marketing.Coupons.View',
        COUPONS_CREATE: 'Marketing.Coupons.Create',
        COUPONS_UPDATE: 'Marketing.Coupons.Update',
        COUPONS_DELETE: 'Marketing.Coupons.Delete',
    },
    IDENTITY: {
        USERS_VIEW: 'Identity.Users.View',
        USERS_CREATE: 'Identity.Users.Create',
        USERS_MANAGE: 'Identity.Users.Manage',

        ROLES_VIEW: 'Identity.Roles.View',
        ROLES_CREATE: 'Identity.Roles.Create',
        ROLES_MANAGE: 'Identity.Roles.Manage',
    },
} as const;
