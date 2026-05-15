# 📊 DATABASE SCHEMA HOÀN CHỈNH - eShop

**Ngày cập nhật:** 14/05/2026  
**Tổng bảng:** 28 + OpenIddict (4 bảng hệ thống)  
**Schema mặc định:** `dbo`

---

## 📑 MỤC LỤC

- [1. Bảng Thương Mại (Catalog)](#1-bảng-thương-mại-catalog)
- [2. Bảng Giỏ Hàng & Đơn Hàng (Cart & Order)](#2-bảng-giỏ-hàng--đơn-hàng-cart--order)
- [3. Bảng Thanh Toán & Vận Chuyển (Payment & Logistics)](#3-bảng-thanh-toán--vận-chuyển-payment--logistics)
- [4. Bảng Khuyến Mại (Promotion)](#4-bảng-khuyến-mại-promotion)
- [5. Bảng Người Dùng & Xác Thực (User & Authentication)](#5-bảng-người-dùng--xác-thực-user--authentication)
- [6. Bảng Quyền (Permission)](#6-bảng-quyền-permission)
- [7. Bảng Đánh Giá (Review)](#7-bảng-đánh-giá-review)
- [8. Bảng Cấu Hình Hệ Thống (System Configuration)](#8-bảng-cấu-hình-hệ-thống-system-configuration)
- [9. Mối Quan Hệ Chi Tiết](#9-mối-quan-hệ-chi-tiết)
- [10. Bảng Chưa Sử Dụng](#10-bảng-chưa-sử-dụng)

---

## 1. Bảng Thương Mại (Catalog)

### 📌 **Category** (Danh mục sản phẩm)
```
Bảng: Categories
Schema: dbo
Type: Hierarchy (Self-join với ParentId)
```

| Trường | Kiểu Dữ Liệu | Required | Constraints | Mô Tả |
|--------|-------------|----------|------------|-------|
| **Id** | int | ✅ | PK, Identity | Mã danh mục |
| **ParentId** | int? | ❌ | FK (Self) | Danh mục cha (dùng cho category con) |
| **Name** | nvarchar(256) | ✅ | | Tên danh mục (VD: Điện Thoại, Máy Tính) |
| **Slug** | varchar(256) | ✅ | Unique | URL-friendly name (VD: dien-thoai) |
| CreatedDate | datetime | ❌ | | Ngày tạo (Audit) |
| ModifiedDate | datetime | ❌ | | Ngày sửa (Audit) |
| Deleted | bit | ❌ | Default: 0 | Soft-delete |

**Navigation Properties:**
- ↕️ **Parent** (Self): Category? (Danh mục cha)
- ↕️ **Children** (Self): ICollection<Category> (Danh mục con)
- ➡️ **Products**: ICollection<Product> (Sản phẩm trong danh mục)

**Index:**
- `IX_Category_Slug` (Unique) - Tìm kiếm theo slug

---

### 📌 **Product** (Sản phẩm)
```
Bảng: Products
Schema: dbo
Type: Core catalog
```

| Trường | Kiểu Dữ Liệu | Required | Constraints | Mô Tả |
|--------|-------------|----------|------------|-------|
| **Id** | int | ✅ | PK, Identity | Mã sản phẩm |
| **CategoryId** | int | ✅ | FK → Categories | Danh mục sản phẩm |
| **Name** | nvarchar(256) | ✅ | | Tên sản phẩm |
| **Slug** | varchar(256) | ✅ | Unique | URL-friendly name |
| **BasePrice** | decimal(18,2) | ❌ | | Giá cơ bản (trước khi áp dụng variant) |
| **Description** | nvarchar(max) | ❌ | | Mô tả chi tiết sản phẩm |
| CreatedDate | datetime | ❌ | | Ngày tạo (Audit) |
| ModifiedDate | datetime | ❌ | | Ngày sửa (Audit) |
| Deleted | bit | ❌ | Default: 0 | Soft-delete |

**Navigation Properties:**
- ➡️ **Category**: Category (Danh mục cha)
- ➡️ **Images**: ICollection<ProductImage> (Ảnh sản phẩm)
- ➡️ **Variants**: ICollection<ProductVariant> (Biến thể sản phẩm)
- ➡️ **Reviews**: ICollection<Reviews> (Đánh giá từ users)

**Index:**
- `IX_Product_Slug` (Unique) - Tìm kiếm theo slug

---

### 📌 **ProductVariant** (Biến thể sản phẩm)
```
Bảng: ProductVariants
Schema: dbo
Type: Child of Product
```

| Trường | Kiểu Dữ Liệu | Required | Constraints | Mô Tả |
|--------|-------------|----------|------------|-------|
| **Id** | int | ✅ | PK, Identity | Mã biến thể |
| **ProductId** | int | ✅ | FK → Products | Sản phẩm cha |
| **SKU** | varchar(100) | ✅ | Unique | Stock Keeping Unit (Mã kho) |
| **Size** | nvarchar(50) | ❌ | | Kích thước (VD: S, M, L, XL) |
| **Color** | nvarchar(50) | ❌ | | Màu sắc (VD: Red, Blue) |
| **PriceAdjustment** | decimal(18,2) | ❌ | | Điều chỉnh giá so với BasePrice |
| **StockQuantity** | int | ❌ | | Số lượng trong kho |
| **RowVersion** | byte[] | ❌ | Timestamp | Optimistic concurrency |
| CreatedDate | datetime | ❌ | | Ngày tạo |
| ModifiedDate | datetime | ❌ | | Ngày sửa |
| Deleted | bit | ❌ | | Soft-delete |

**Navigation Properties:**
- ➡️ **Product**: Product (Sản phẩm cha)
- ⬅️ **CartItems**: ICollection<CartItem>
- ⬅️ **OrderItems**: ICollection<OrderItem>

**Index:**
- `IX_ProductVariant_SKU` (Unique) - Mã kho duy nhất

---

### 📌 **ProductImage** (Ảnh sản phẩm)
```
Bảng: ProductImages
Schema: dbo
Type: Child of Product
```

| Trường | Kiểu Dữ Liệu | Required | Constraints | Mô Tả |
|--------|-------------|----------|------------|-------|
| **Id** | int | ✅ | PK, Identity | Mã ảnh |
| **ProductId** | int | ✅ | FK → Products | Sản phẩm |
| **Url** | nvarchar(500) | ✅ | | URL của ảnh |
| **SortOrder** | int | ❌ | | Thứ tự hiển thị (0, 1, 2...) |
| **IsPrimary** | bit | ❌ | Default: 0 | Ảnh chính (thumbnail) |
| CreatedDate | datetime | ❌ | | Ngày tạo |
| ModifiedDate | datetime | ❌ | | Ngày sửa |
| Deleted | bit | ❌ | | Soft-delete |

**Navigation Properties:**
- ➡️ **Product**: Product (Sản phẩm cha)

---

## 2. Bảng Giỏ Hàng & Đơn Hàng (Cart & Order)

### 📌 **Cart** (Giỏ hàng)
```
Bảng: Carts
Schema: dbo
Type: User personal
Constraint: 1 User → 1 Cart (Unique UserId)
```

| Trường | Kiểu Dữ Liệu | Required | Constraints | Mô Tả |
|--------|-------------|----------|------------|-------|
| **Id** | int | ✅ | PK, Identity | Mã giỏ hàng |
| **UserId** | int | ✅ | FK → Users, Unique | Người dùng sở hữu giỏ |
| **LastUpdatedAt** | datetime | ✅ | | Lần cập nhật cuối cùng |

**Navigation Properties:**
- ➡️ **User**: Users (Chủ giỏ)
- ➡️ **Items**: ICollection<CartItem> (Chi tiết trong giỏ)

**Index:**
- `IX_Cart_UserId` (Unique) - 1 user chỉ có 1 giỏ

---

### 📌 **CartItem** (Chi tiết giỏ hàng)
```
Bảng: CartItems
Schema: dbo
Type: Child of Cart
```

| Trường | Kiểu Dữ Liệu | Required | Constraints | Mô Tả |
|--------|-------------|----------|------------|-------|
| **Id** | int | ✅ | PK, Identity | Mã item |
| **CartId** | int | ✅ | FK → Carts | Giỏ hàng |
| **ProductVariantId** | int | ✅ | FK → ProductVariants | Biến thể sản phẩm |
| **Quantity** | int | ✅ | | Số lượng |

**Navigation Properties:**
- ➡️ **Cart**: Cart (Giỏ cha)
- ➡️ **ProductVariant**: ProductVariant (Chi tiết sản phẩm)

---

### 📌 **Order** (Đơn hàng)
```
Bảng: Orders
Schema: dbo
Type: Core transaction
Soft-delete: Yes
Query Filter: WHERE Deleted = 0
```

| Trường | Kiểu Dữ Liệu | Required | Constraints | Mô Tả |
|--------|-------------|----------|------------|-------|
| **Id** | int | ✅ | PK, Identity | Mã đơn hàng |
| **OrderCode** | varchar(50) | ✅ | Unique | Mã đơn hàng hiển thị (VD: ORD-20260514-001) |
| **UserId** | int | ✅ | FK → Users | Người mua |
| **ShippingAddress** | nvarchar(1024) | ✅ | | Địa chỉ giao hàng |
| **TotalAmount** | decimal(18,2) | ✅ | | Tổng tiền đơn hàng |
| **Status** | varchar(50) | ✅ | | Trạng thái (Pending, Processing, Shipped, Delivered, Cancelled) |
| **PaymentMethod** | varchar(50) | ✅ | | Phương thức thanh toán (Cash, CreditCard, Bank Transfer) |
| **RowVersion** | byte[] | ❌ | Timestamp | Optimistic concurrency |
| CreatedDate | datetime | ❌ | | Ngày tạo |
| ModifiedDate | datetime | ❌ | | Ngày sửa |
| Deleted | bit | ❌ | Default: 0 | Soft-delete |

**Navigation Properties:**
- ➡️ **User**: Users (Người mua)
- ➡️ **OrderItems**: ICollection<OrderItem> (Chi tiết đơn hàng)
- ➡️ **Payments**: ICollection<Payments> (Thanh toán liên quan)
- ⬅️ **CouponUsages**: ICollection<CouponUsage> (Mã giảm giá sử dụng)
- ⬅️ **Shipments**: ICollection<Shipment> (Vận đơn)

**Index:**
- `IX_Order_OrderCode` (Unique) - Tìm kiếm theo mã đơn

---

### 📌 **OrderItem** (Chi tiết đơn hàng)
```
Bảng: OrderItems
Schema: dbo
Type: Child of Order
```

| Trường | Kiểu Dữ Liệu | Required | Constraints | Mô Tả |
|--------|-------------|----------|------------|-------|
| **Id** | int | ✅ | PK, Identity | Mã item |
| **OrderId** | int | ✅ | FK → Orders | Đơn hàng |
| **ProductVariantId** | int | ✅ | FK → ProductVariants | Biến thể sản phẩm |
| **Quantity** | int | ✅ | | Số lượng mua |
| **ProductName** | nvarchar(256) | ✅ | | Tên sản phẩm snapshot |
| **VariantSKU** | varchar(100) | ✅ | | SKU snapshot |
| **UnitPrice** | decimal(18,2) | ✅ | | Giá một sản phẩm tại thời điểm mua |

**Navigation Properties:**
- ➡️ **Order**: Order (Đơn cha)
- ➡️ **ProductVariant**: ProductVariant (Chi tiết sản phẩm)
- ➡️ **Refunds**: ICollection<OrderRefund> (Hoàn lại)

---

### 📌 **OrderRefund** (Hoàn lại/Trả hàng)
```
Bảng: OrderRefunds
Schema: dbo
Type: Child of OrderItem
```

| Trường | Kiểu Dữ Liệu | Required | Constraints | Mô Tả |
|--------|-------------|----------|------------|-------|
| **Id** | int | ✅ | PK, Identity | Mã hoàn lại |
| **OrderItemId** | int | ✅ | FK → OrderItems | Item được hoàn |
| **RefundQuantity** | int | ✅ | | Số lượng hoàn |
| **Reason** | nvarchar(500) | ❌ | | Lý do hoàn lại (VD: Hỏng, Sai hàng) |
| **Status** | varchar(50) | ✅ | | Trạng thái (PENDING, APPROVED, REJECTED, REFUNDED) |
| CreatedDate | datetime | ❌ | | Ngày tạo |
| ModifiedDate | datetime | ❌ | | Ngày sửa |
| Deleted | bit | ❌ | | Soft-delete |

**Navigation Properties:**
- ➡️ **OrderItem**: OrderItem (Item cha)

---

## 3. Bảng Thanh Toán & Vận Chuyển (Payment & Logistics)

### 📌 **Payments** (Thanh toán)
```
Bảng: Payments
Schema: dbo
Type: Payment transaction
```

| Trường | Kiểu Dữ Liệu | Required | Constraints | Mô Tả |
|--------|-------------|----------|------------|-------|
| **Id** | int | ✅ | PK, Identity | Mã thanh toán |
| **OrderId** | int | ✅ | FK → Orders | Đơn hàng thanh toán |
| **Method** | nvarchar(max) | ✅ | | Phương thức (Cash, CreditCard, Bank Transfer) |
| **Status** | nvarchar(50) | ✅ | Default: Pending | Trạng thái (Pending, Success, Failed) |
| **Amount** | decimal(18,2) | ✅ | | Số tiền thanh toán |
| **PaidAt** | datetime? | ❌ | | Thời điểm thanh toán thành công |

**Navigation Properties:**
- ➡️ **Order**: Order (Đơn hàng)

---

### 📌 **Shipment** (Vận đơn)
```
Bảng: Shipments
Schema: dbo
Type: Logistics tracking
Note: 1 Order → Multiple Shipments (Split Shipment Support)
Soft-delete: Yes
```

| Trường | Kiểu Dữ Liệu | Required | Constraints | Mô Tả |
|--------|-------------|----------|------------|-------|
| **Id** | int | ✅ | PK, Identity | Mã vận đơn |
| **OrderId** | int | ✅ | FK → Orders | Đơn hàng liên quan |
| **ShippingProvider** | nvarchar(100) | ✅ | | Nhà vận chuyển (GHN, NinjaVan, GrabExpress, Viettel Post, GHTK) |
| **TrackingNumber** | varchar(100)? | ❌ | | Mã theo dõi từ nhà vận chuyển |
| **ShippingFee** | decimal(18,2) | ✅ | | Phí vận chuyển |
| **ReceiverName** | nvarchar(256) | ✅ | | Tên người nhận |
| **ReceiverPhone** | nvarchar(20) | ✅ | | SĐT người nhận |
| **ShippingAddress** | nvarchar(1024) | ✅ | | Địa chỉ giao hàng |
| **EstimatedDelivery** | datetime? | ❌ | | Ngày giao dự kiến |
| **ActualDelivery** | datetime? | ❌ | | Ngày giao thực tế |
| **Status** | varchar(50) | ✅ | Default: PENDING | Trạng thái (PENDING, PICKED_UP, IN_TRANSIT, DELIVERED, FAILED, RETURNED) |
| CreatedDate | datetime | ❌ | | Ngày tạo |
| ModifiedDate | datetime | ❌ | | Ngày sửa |
| Deleted | bit | ❌ | Default: 0 | Soft-delete |

**Navigation Properties:**
- ➡️ **Order**: Order (Đơn hàng)

**Index:**
- `IX_Shipment_TrackingNumber` - Tìm kiếm theo mã theo dõi

**Tính năng:**
- ✅ Hỗ trợ **Split Shipment** (1 đơn → nhiều vận đơn nếu từ nhiều kho)
- ✅ Multi-carrier (GHN, NinjaVan, GrabExpress, Viettel Post, GHTK)
- ✅ Individual tracking per shipment

---

## 4. Bảng Khuyến Mại (Promotion)

### 📌 **Coupons** (Mã giảm giá)
```
Bảng: Coupons
Schema: dbo
Type: Promotion code
```

| Trường | Kiểu Dữ Liệu | Required | Constraints | Mô Tả |
|--------|-------------|----------|------------|-------|
| **Id** | int | ✅ | PK, Identity | Mã coupon |
| **Code** | nvarchar(max) | ✅ | Unique | Mã giảm giá (VD: SALE50, FREESHIP) |
| **DiscountType** | int | ✅ | Enum | Loại giảm (Percentage, Fixed) |
| **DiscountValue** | decimal(18,2) | ✅ | | Giá trị giảm (% hoặc số tiền) |
| **MinOrderValue** | decimal(18,2)? | ❌ | | Tối thiểu giá trị đơn để áp dụng |
| **MaxDiscountValue** | decimal(18,2)? | ❌ | | Giảm tối đa (cho % type) |
| **StartDate** | datetime | ✅ | | Ngày bắt đầu có hiệu lực |
| **ExpiryDate** | datetime | ✅ | | Ngày hết hạn |
| **UsageLimit** | int? | ❌ | | Tổng lần được sử dụng (null = vô hạn) |
| **UsedCount** | int | ✅ | Default: 0 | Đã sử dụng mấy lần |
| **UsageLimitPerUser** | int? | ❌ | | Giới hạn dùng per user (null = vô hạn) |
| **IsActive** | bit | ✅ | Default: 1 | Đang hoạt động? |
| CreatedDate | datetime | ❌ | | Ngày tạo |
| ModifiedDate | datetime | ❌ | | Ngày sửa |
| Deleted | bit | ❌ | | Soft-delete |

**Navigation Properties:**
- ⬅️ **CouponUsages**: ICollection<CouponUsage> (Lịch sử sử dụng)

**Index:**
- `IX_Coupons_Code` (Unique) - Tìm kiếm mã

---

### 📌 **CouponUsage** (Lịch sử sử dụng mã giảm giá)
```
Bảng: CouponUsages
Schema: dbo
Type: Audit trail
Purpose: Track coupon usage for UsageLimitPerUser check & Marketing spend analysis
Constraint: 1 Coupon + 1 User + 1 Order → Unique CouponUsage
```

| Trường | Kiểu Dữ Liệu | Required | Constraints | Mô Tả |
|--------|-------------|----------|------------|-------|
| **Id** | int | ✅ | PK, Identity | Mã bản ghi |
| **CouponId** | int | ✅ | FK → Coupons, Restrict | Mã giảm giá (Restrict: không cho xóa mã nếu đã dùng) |
| **UserId** | int | ✅ | FK → Users, Restrict | Người dùng (Restrict: không cho xóa user nếu dùng mã) |
| **OrderId** | int | ✅ | FK → Orders, Cascade | Đơn hàng áp dụng (Cascade: xóa order → xóa bản ghi) |
| **DiscountAmount** | decimal(18,2) | ✅ | | Số tiền thực tế đã giảm |
| **UsedAt** | datetime | ✅ | Default: UtcNow | Thời điểm sử dụng |

**Navigation Properties:**
- ➡️ **Coupon**: Coupons (Mã giảm)
- ➡️ **User**: Users (Người dùng)
- ➡️ **Order**: Order (Đơn hàng)

**Index:**
- `IX_CouponUsage_UserId_CouponId` - Kiểm tra UsageLimitPerUser

**Delete Behavior:**
- CouponId: `Restrict` (không cho xóa coupon nếu đã dùng)
- UserId: `Restrict` (không cho xóa user nếu dùng coupon)
- OrderId: `Cascade` (xóa order → xóa coupon usage)

**Tính năng:**
- ✅ Audit trail cho marketing spend analysis
- ✅ Hỗ trợ kiểm tra UsageLimitPerUser
- ✅ Tránh cascade cycle error

---

## 5. Bảng Người Dùng & Xác Thực (User & Authentication)

### 📌 **Users** (Người dùng)
```
Bảng: Users
Schema: dbo
Type: Core identity
Soft-delete: Yes
Concurrency: Timestamp (RowVersion)
```

| Trường | Kiểu Dữ Liệu | Required | Constraints | Mô Tả |
|--------|-------------|----------|------------|-------|
| **Id** | int | ✅ | PK, Identity | Mã người dùng |
| **Username** | varchar(128) | ✅ | Unique | Tên đăng nhập |
| **Email** | varchar(128) | ✅ | Unique | Email |
| **Phone** | varchar(20)? | ❌ | | Số điện thoại |
| **PasswordHash** | varchar(256) | ✅ | | Mật khẩu (hash) |
| **IsTempPassword** | bit | ✅ | | Mật khẩu tạm thời? |
| **PinCode** | varchar(128)? | ❌ | | Mã PIN |
| **IsTempPin** | bit | ✅ | | PIN tạm thời? |
| **UserType** | int | ✅ | Enum | Loại user (CUSTOMER, STAFF, ADMIN) |
| **Status** | int | ✅ | | Trạng thái (ACTIVE, INACTIVE, BANNED) |
| **IsFirstTime** | bit | ✅ | | Lần đầu đăng nhập? |
| **LoginFailCount** | int | ✅ | Default: 0 | Số lần đăng nhập thất bại |
| **DateTimeLoginFailCount** | datetime? | ❌ | | Lần thất bại cuối |
| **TimeLockUser** | datetime? | ❌ | | Thời gian khóa account (nếu >5 lần fail) |
| **LastLogin** | datetime? | ❌ | | Lần đăng nhập cuối |
| **RowVersion** | byte[] | ❌ | Timestamp | Optimistic concurrency |
| CreatedDate | datetime | ❌ | | Ngày tạo |
| ModifiedDate | datetime | ❌ | | Ngày sửa |
| Deleted | bit | ❌ | Default: 0 | Soft-delete |

**Navigation Properties:**
- ➡️ **Profile**: UserProfile (Thông tin cá nhân 1-1)
- ➡️ **UserRoles**: List<UserRole> (Các role của user)
- ➡️ **NotificationTokens**: List<NotificationToken> (FCM tokens)
- ➡️ **Cart**: Cart? (Giỏ hàng)
- ➡️ **Orders**: ICollection<Order> (Đơn hàng)
- ➡️ **AuthOtps**: ICollection<AuthOtp> (Mã OTP)
- ➡️ **Addresses**: ICollection<Addresses> (Địa chỉ)
- ➡️ **Reviews**: ICollection<Reviews> (Đánh giá)

**Index:**
- `IX_Users_Email` (Unique)
- `IX_Users_Username` (Unique)

---

### 📌 **UserProfile** (Thông tin cá nhân)
```
Bảng: UserProfiles
Schema: dbo
Type: 1-1 with Users
```

| Trường | Kiểu Dữ Liệu | Required | Constraints | Mô Tả |
|--------|-------------|----------|------------|-------|
| **Id** | int | ✅ | PK, Identity | Mã profile |
| **UserId** | int | ✅ | FK → Users, 1-1 | Người dùng |
| **FullName** | nvarchar(max)? | ❌ | | Họ và tên |
| **PhoneNumber** | nvarchar(max) | ✅ | Required | Số điện thoại |
| **DateOfBirth** | datetime? | ❌ | | Ngày sinh |
| **Gender** | int? | ❌ | Enum | Giới tính (Male, Female, Other) |
| **AvatarUrl** | nvarchar(max)? | ❌ | | URL ảnh đại diện |

---

### 📌 **Addresses** (Địa chỉ)
```
Bảng: Addresses
Schema: dbo
Type: User address book
```

| Trường | Kiểu Dữ Liệu | Required | Constraints | Mô Tả |
|--------|-------------|----------|------------|-------|
| **Id** | int | ✅ | PK, Identity | Mã địa chỉ |
| **UserId** | int? | ❌ | FK → Users | Chủ sở hữu địa chỉ |
| **Street** | nvarchar(200) | ✅ | | Đường (VD: 123 Nguyễn Huệ) |
| **City** | nvarchar(100) | ✅ | | Thành phố (VD: Hồ Chí Minh) |
| **Province** | nvarchar(100) | ✅ | | Tỉnh/Thành (VD: TP. Hồ Chí Minh) |
| **IsDefault** | bit | ✅ | Default: 0 | Địa chỉ mặc định? |
| CreatedDate | datetime | ❌ | | Ngày tạo |
| ModifiedDate | datetime | ❌ | | Ngày sửa |
| Deleted | bit | ❌ | | Soft-delete |

**Navigation Properties:**
- ➡️ **User**: Users? (Chủ sở hữu)
- ⬅️ **Orders**: ICollection<Order>

---

### 📌 **AuthOtp** (OTP xác thực)
```
Bảng: AuthOtps
Schema: dbo
Type: Authentication token
```

| Trường | Kiểu Dữ Liệu | Required | Constraints | Mô Tả |
|--------|-------------|----------|------------|-------|
| **Id** | int | ✅ | PK, Identity | Mã OTP |
| **OtpCode** | varchar(128) | ✅ | | Mã OTP (hash) |
| **ExpireTime** | datetime | ✅ | | Hết hạn lúc |
| **IsUsed** | bit | ✅ | Default: 0 | Đã dùng? |
| **UserId** | int | ✅ | FK → Users | Người dùng |
| **VerifyTime** | int | ✅ | Default: 0 | Số lần xác nhận |

**Navigation Properties:**
- ➡️ **User**: Users (Người dùng)

---

### 📌 **SendOtp** (Lịch sử gửi OTP)
```
Bảng: SendOtps
Schema: dbo
Type: OTP rate limiting
Purpose: Prevent OTP spam
```

| Trường | Kiểu Dữ Liệu | Required | Constraints | Mô Tả |
|--------|-------------|----------|------------|-------|
| **Id** | int | ✅ | PK, Identity | Mã bản ghi |
| **Username** | varchar(128) | ✅ | | Tên đăng nhập / Email |
| **SendCount** | int | ✅ | Default: 0 | Số lần gửi |
| **LastSentDateTime** | datetime | ✅ | | Lần gửi cuối |
| **TimeLimitCanVerifyOtp** | datetime | ✅ | | Có thể gửi lại lúc nào |

**Index:**
- `IX_SendOtp` - Tìm theo username

---

### 📌 **NotificationToken** (FCM Token)
```
Bảng: NotificationTokens
Schema: dbo
Type: Push notification
```

| Trường | Kiểu Dữ Liệu | Required | Constraints | Mô Tả |
|--------|-------------|----------|------------|-------|
| **Id** | int | ✅ | PK, Identity | Mã token |
| **FcmToken** | varchar(128)? | ❌ | | Firebase Cloud Messaging token |
| **ApnsToken** | varchar(128)? | ❌ | ⚠️ Obsolete | Apple Push Notification token (không dùng) |
| **UserId** | int | ✅ | FK → Users | Người dùng |

**Navigation Properties:**
- ➡️ **User**: Users

---

## 6. Bảng Quyền (Permission)

### 📌 **Role** (Vai trò hệ thống)
```
Bảng: Roles
Schema: dbo
Type: System role
Values: ADMIN, STAFF, CUSTOMER
```

| Trường | Kiểu Dữ Liệu | Required | Constraints | Mô Tả |
|--------|-------------|----------|------------|-------|
| **Id** | int | ✅ | PK, Identity | Mã role |
| **Name** | nvarchar(50) | ✅ | | Tên (ADMIN, STAFF, CUSTOMER) |
| **Description** | nvarchar(256)? | ❌ | | Mô tả |
| **Status** | int | ✅ | | Trạng thái (Active/Inactive) |
| CreatedDate | datetime | ❌ | | Ngày tạo |
| ModifiedDate | datetime | ❌ | | Ngày sửa |
| Deleted | bit | ❌ | | Soft-delete |

**Navigation Properties:**
- ⬅️ **RolePermissions**: ICollection<RolePermission>
- ⬅️ **UserRoles**: ICollection<UserRole>

---

### 📌 **RolePermission** (Quyền của role)
```
Bảng: RolePermissions
Schema: dbo
Type: Role-Permission junction
```

| Trường | Kiểu Dữ Liệu | Required | Constraints | Mô Tả |
|--------|-------------|----------|------------|-------|
| **Id** | int | ✅ | PK, Identity | Mã bản ghi |
| **RoleId** | int | ✅ | FK → Roles | Role |
| **PermissionKey** | varchar(128) | ✅ | | Mã quyền (VD: "Product.View", "Product.Create") |
| CreatedDate | datetime | ❌ | | Ngày tạo |
| CreatedBy | int? | ❌ | | Người tạo |

**Navigation Properties:**
- ➡️ **Role**: Role

---

### 📌 **UserRole** (Vai trò của user)
```
Bảng: UserRoles
Schema: dbo
Type: User-Role junction
```

| Trường | Kiểu Dữ Liệu | Required | Constraints | Mô Tả |
|--------|-------------|----------|------------|-------|
| **Id** | int | ✅ | PK, Identity | Mã bản ghi |
| **UserId** | int | ✅ | FK → Users | Người dùng |
| **RoleId** | int | ✅ | FK → Roles | Vai trò |
| CreatedDate | datetime | ❌ | | Ngày tạo |
| ModifiedDate | datetime | ❌ | | Ngày sửa |
| DeletedDate | datetime | ❌ | | Ngày xóa |
| Deleted | bit | ✅ | Default: 0 | Soft-delete |

**Navigation Properties:**
- ➡️ **User**: Users
- ➡️ **Role**: Role

**Index:**
- `IX_UserRole` (UserId, RoleId, Deleted)

---

### 📌 **CustomerRole** (Vai trò khách hàng)
```
Bảng: CustomerRoles
Schema: dbo
Type: Custom role for customers
Purpose: Define custom roles beyond standard CUSTOMER role
```

| Trường | Kiểu Dữ Liệu | Required | Constraints | Mô Tả |
|--------|-------------|----------|------------|-------|
| **Id** | int | ✅ | PK, Identity | Mã role |
| **Name** | nvarchar(max) | ✅ | | Tên role tùy chỉnh |
| **UserType** | int | ✅ | Enum | Loại user (UserTypeEnum) |
| **CustomerId** | int | ✅ | | ID khách hàng |
| **Description** | nvarchar(max)? | ❌ | | Mô tả |
| **Status** | int | ✅ | | Trạng thái |
| **PermissionInWeb** | int | ✅ | | Quyền trên website |
| CreatedDate | datetime | ❌ | | Ngày tạo |
| ModifiedDate | datetime | ❌ | | Ngày sửa |
| Deleted | bit | ❌ | | Soft-delete |

**Navigation Properties:**
- ⬅️ **CustomerUserRoles**: List<CustomerUserRole>
- ⬅️ **CustomerRolePermissions**: ICollection<CustomerRolePermission>

---

### 📌 **CustomerUserRole** (Vai trò khách hàng của user)
```
Bảng: CustomerUserRoles
Schema: dbo
Type: User-CustomerRole junction
```

| Trường | Kiểu Dữ Liệu | Required | Constraints | Mô Tả |
|--------|-------------|----------|------------|-------|
| **Id** | int | ✅ | PK, Identity | Mã bản ghi |
| **UserId** | int | ✅ | FK → Users | Người dùng |
| **CustomerRoleId** | int | ✅ | FK → CustomerRoles | Vai trò khách hàng |
| CreatedDate | datetime | ❌ | | Ngày tạo |
| ModifiedDate | datetime | ❌ | | Ngày sửa |
| DeletedDate | datetime | ❌ | | Ngày xóa |
| Deleted | bit | ✅ | | Soft-delete |

**Navigation Properties:**
- ➡️ **User**: Users
- ➡️ **CustomerRole**: CustomerRole

---

### 📌 **CustomerRolePermission** (Quyền của CustomerRole)
```
Bảng: CustomerRolePermissions
Schema: dbo
Type: CustomerRole-Permission junction
```

| Trường | Kiểu Dữ Liệu | Required | Constraints | Mô Tả |
|--------|-------------|----------|------------|-------|
| **Id** | int | ✅ | PK, Identity | Mã bản ghi |
| **CustomerRoleId** | int | ✅ | FK → CustomerRoles | Role khách hàng |
| **PermissionKey** | varchar(128) | ✅ | | Mã quyền |
| CreatedDate | datetime | ❌ | | Ngày tạo |
| CreatedBy | int? | ❌ | | Người tạo |

**Navigation Properties:**
- ➡️ **CustomerRole**: CustomerRole

---

### 📌 **PermissionMax** (Quyền tối đa)
```
Bảng: PermissionMaxs
Schema: dbo
Type: Permission cap/limit
Purpose: Track max permissions for user
```

| Trường | Kiểu Dữ Liệu | Required | Constraints | Mô Tả |
|--------|-------------|----------|------------|-------|
| **Id** | int | ✅ | PK, Identity | Mã bản ghi |
| **CustomerId** | int | ✅ | | ID khách hàng |
| **PermissionKey** | nvarchar(max) | ✅ | | Mã quyền tối đa |

---

## 7. Bảng Đánh Giá (Review)

### 📌 **Reviews** (Đánh giá sản phẩm)
```
Bảng: Reviews
Schema: dbo
Type: Product review
```

| Trường | Kiểu Dữ Liệu | Required | Constraints | Mô Tả |
|--------|-------------|----------|------------|-------|
| **Id** | int | ✅ | PK, Identity | Mã đánh giá |
| **UserId** | int | ✅ | FK → Users | Người đánh giá |
| **ProductId** | int | ✅ | FK → Products | Sản phẩm được đánh giá |
| **Rating** | int | ✅ | Range: 1-5 | Số sao (1-5) |
| **Comment** | nvarchar(max) | ✅ | Default: "" | Bình luận |
| **CreatedAt** | datetime | ✅ | Default: UtcNow | Thời điểm đánh giá |

**Navigation Properties:**
- ➡️ **User**: Users (Người đánh giá)
- ➡️ **Product**: Product (Sản phẩm)

---

## 8. Bảng Cấu Hình Hệ Thống (System Configuration)

### 📌 **SysVar** (Biến hệ thống)
```
Bảng: SysVars
Schema: dbo
Type: System configuration
Purpose: Store system-wide settings (GrName-VarName-VarValue)
```

| Trường | Kiểu Dữ Liệu | Required | Constraints | Mô Tả |
|--------|-------------|----------|------------|-------|
| **Id** | int | ✅ | PK, Identity | Mã biến |
| **GrName** | varchar(128) | ✅ | | Nhóm biến (VD: "LOGIN", "EMAIL") |
| **VarName** | varchar(128) | ✅ | | Tên biến (VD: "LOGINMAXTURN", "EMAILHOST") |
| **VarValue** | varchar(128) | ✅ | | Giá trị (VD: "5", "smtp.gmail.com") |
| **VarDesc** | nvarchar(128)? | ❌ | | Mô tả biến |

**Index:**
- `IX_SysVar` (GrName, VarName)

**Common SysVars:**
```
GrName=LOGIN
  VarName=LOGINMAXTURN → VarValue=5 (tối đa 5 lần fail)
  VarName=LOCKTIME → VarValue=30 (khóa 30 phút)
  
GrName=EMAIL
  VarName=EMAILHOST → VarValue=smtp.gmail.com
  VarName=EMAILPORT → VarValue=587
  
GrName=OTP
  VarName=OTPEXPIRETIME → VarValue=300 (hết hạn 5 phút)
  VarName=OTPSENDLIMIT → VarValue=3 (gửi tối đa 3 lần)
```

---

## 9. Mối Quan Hệ Chi Tiết

### 9.1 Mối Quan Hệ 1-to-Many (1:N)

```
Category (1) ────────────────> (N) Product
├─ 1 Category có nhiều Product

Category (1) ────────────────> (N) Category (Self-join: Parent-Children)
├─ 1 Category cha có nhiều Category con

Product (1) ────────────────> (N) ProductVariant
├─ 1 Product có nhiều Variant (Size, Color)

Product (1) ────────────────> (N) ProductImage
├─ 1 Product có nhiều Ảnh

Product (1) ────────────────> (N) Reviews
├─ 1 Product được review bởi nhiều User

Users (1) ────────────────> (N) Cart (1-1 unique)
├─ 1 User có 1 Giỏ hàng

Cart (1) ────────────────> (N) CartItem
├─ 1 Giỏ hàng chứa nhiều Item

Users (1) ────────────────> (N) Order
├─ 1 User tạo nhiều Đơn hàng

Order (1) ────────────────> (N) OrderItem
├─ 1 Đơn hàng chứa nhiều Item

Order (1) ────────────────> (N) Shipment
├─ 1 Đơn có 1+ Vận đơn (Split Shipment Support)

Order (1) ────────────────> (N) Payments
├─ 1 Đơn có thể có nhiều giao dịch thanh toán

OrderItem (1) ────────────────> (N) OrderRefund
├─ 1 OrderItem có thể hoàn lại 1+ lần

Coupons (1) ────────────────> (N) CouponUsage
├─ 1 Coupon được dùng bởi nhiều User

Users (1) ────────────────> (N) CouponUsage
├─ 1 User dùng nhiều Coupon

Users (1) ────────────────> (N) UserRole
├─ 1 User có 1+ Role

Users (1) ────────────────> (N) AuthOtp
├─ 1 User có nhiều OTP

Users (1) ────────────────> (N) Addresses
├─ 1 User có nhiều Địa chỉ

Users (1) ────────────────> (N) Reviews
├─ 1 User review nhiều Sản phẩm

Users (1) ────────────────> (N) NotificationToken
├─ 1 User có nhiều Device Token (mobile, tablet, web)

Role (1) ────────────────> (N) RolePermission
├─ 1 Role có nhiều Permission

Role (1) ────────────────> (N) UserRole
├─ 1 Role được gán cho nhiều User

CustomerRole (1) ────────────────> (N) CustomerUserRole
├─ 1 CustomerRole gán cho nhiều User

CustomerRole (1) ────────────────> (N) CustomerRolePermission
├─ 1 CustomerRole có nhiều Permission
```

### 9.2 Mối Quan Hệ Many-to-Many (M:N) Qua Junction Table

```
Users (N) ──────────── UserRole ────────────── (N) Role
│                    Junction Table                │
└─────────────────────────────────────────────────┘
Purpose: 1 User có 1+ Roles, 1 Role có nhiều Users

Users (N) ──────── CustomerUserRole ──────── (N) CustomerRole
│                  Junction Table                │
└───────────────────────────────────────────────┘
Purpose: Customer assignment to custom roles

ProductVariant (N) ────── CartItem ──────────── (N) Cart
│                        Junction Table          │
└──────────────────────────────────────────────┘
Purpose: 1 Cart chứa nhiều Variant, 1 Variant có thể trong nhiều Cart

ProductVariant (N) ──── OrderItem ──────────── (N) Order
│                      Junction Table           │
└─────────────────────────────────────────────┘
Purpose: 1 Order chứa nhiều Variant, 1 Variant có thể trong nhiều Order
```

### 9.3 Mối Quan Hệ 1-to-1 (1:1)

```
Users (1) ══════════════════════ (1) UserProfile
├─ 1 User có 1 Profile (bắt buộc)
└─ 1 Profile thuộc 1 User

Cart (1) ══════════════════════ (1) Users
├─ 1 Cart thuộc 1 User (Unique UserId)
└─ 1 User có 1 Cart
```

### 9.4 Bảng Audit Fields (Mặc định)

```
BaseEntity (Abstract):
  ├─ Id (int, PK)
  └─ (No audit fields)

AuditableEntity (Abstract):
  ├─ Id (int, PK)
  ├─ CreatedDate (datetime?)
  ├─ ModifiedDate (datetime?)
  ├─ Deleted (bit, Default: 0)
  └─ (Used by: Product, Category, Shipment, OrderRefund, Role, etc.)

IFullAudited (Interface):
  ├─ CreatedDate (datetime?)
  ├─ CreatedBy (int?)
  ├─ ModifiedDate (datetime?)
  ├─ ModifiedBy (int?)
  ├─ DeletedDate (datetime?)
  ├─ DeletedBy (int?)
  └─ Deleted (bit)
  
ICreatedBy (Interface):
  ├─ CreatedDate (datetime?)
  └─ CreatedBy (int?)
```

---

## 10. Bảng Chưa Sử Dụng

### ⚠️ BẢNG ĐÃ DEPRECATED / CHƯA DÙNG ĐẾN

| Tên Bảng | Vị Trí | Lý Do | Ghi Chú |
|----------|--------|------|---------|
| **NotificationToken.ApnsToken** | AuthToken folder | Obsolete | Dùng `FcmToken` cho cả iOS và Android |
| **PermissionMax** | Permission folder | Không rõ mục đích | Cần review lại |
| **CustomerRole*** | Permission folder | Overlap với Role | Xem xét merge |
| **CustomerUserRole** | Permission folder | Overlap với UserRole | Xem xét merge |
| **CustomerRolePermission** | Permission folder | Overlap với RolePermission | Xem xét merge |

---

## 11. Tóm Tắt Số Liệu

```
📊 THỐNG KÊ DATABASE:

CATALOG (Thương mại):
  - Category (1, Hierarchy)
  - Product (1, with Images & Variants)
  - ProductVariant (nhiều)
  - ProductImage (nhiều)
  
CART & ORDER (Mua hàng):
  - Cart (1 per user)
  - CartItem (nhiều per cart)
  - Order (nhiều per user)
  - OrderItem (nhiều per order)
  - OrderRefund (hoàn lại)
  
PAYMENT & LOGISTICS (Thanh toán & vận chuyển):
  - Payments (giao dịch)
  - Shipment (vận đơn, hỗ trợ split)
  
PROMOTION (Khuyến mại):
  - Coupons (mã giảm)
  - CouponUsage (lịch sử audit)
  
USER & AUTH (Người dùng):
  - Users (core identity)
  - UserProfile (1-1)
  - Addresses (address book)
  - AuthOtp (xác thực OTP)
  - SendOtp (rate limiting)
  - NotificationToken (FCM)
  
PERMISSION (Quyền):
  - Role (system role)
  - RolePermission
  - UserRole
  - CustomerRole (custom)
  - CustomerUserRole
  - CustomerRolePermission
  - PermissionMax
  
REVIEW (Đánh giá):
  - Reviews (1-5 stars)
  
SYSTEM CONFIG (Cấu hình):
  - SysVar (key-value config)
  
OPENIDDICT (OAuth2/OIDC - hệ thống):
  - OpenIddictApplications
  - OpenIddictAuthorizationCodes
  - OpenIddictTokens
  - OpenIddictScopes

📈 TỔNG CỘNG:
  ✅ Domain Entities: 28 bảng
  ✅ OpenIddict: 4 bảng (hệ thống)
  ✅ GRAND TOTAL: 32 bảng trong database
```

---

## 12. Soft-Delete & Query Filter

**Các bảng áp dụng Soft-Delete (WHERE Deleted = 0):**
- ✅ Category
- ✅ Product
- ✅ ProductVariant
- ✅ ProductImage
- ✅ Order
- ✅ OrderRefund
- ✅ Addresses
- ✅ Role
- ✅ UserRole
- ✅ CustomerRole
- ✅ CustomerUserRole
- ✅ Shipment
- ✅ Coupons
- ✅ Users

---

## 13. Cascade Delete Behavior

| Từ Bảng | Đến Bảng | Behavior | Lý Do |
|---------|----------|----------|------|
| Category | Product | Cascade | Category xóa → xóa sản phẩm |
| Product | ProductVariant | Cascade | Sản phẩm xóa → xóa variant |
| Product | ProductImage | Cascade | Sản phẩm xóa → xóa ảnh |
| ProductVariant | CartItem | Cascade | Variant xóa → xóa khỏi giỏ |
| ProductVariant | OrderItem | Cascade | Variant xóa → xóa khỏi order |
| Order | OrderItem | Cascade | Đơn xóa → xóa items |
| Order | Shipment | Cascade | Đơn xóa → xóa vận đơn |
| Order | CouponUsage | Cascade | Đơn xóa → xóa coupon usage |
| **Coupons | CouponUsage | Restrict** | ⚠️ Prevent coupon deletion if used |
| **Users | CouponUsage | Restrict** | ⚠️ Prevent user deletion if used coupon |
| OrderItem | OrderRefund | Cascade | Item xóa → xóa hoàn lại |
| Users | UserRole | Cascade | User xóa → xóa role assignment |
| Users | Order | Cascade | User xóa → xóa đơn |
| Users | Cart | Cascade | User xóa → xóa giỏ |
| Users | CartItem | Cascade | User xóa (via Cart) → xóa items |
| Users | Addresses | Cascade | User xóa → xóa địa chỉ |
| Users | Reviews | Cascade | User xóa → xóa review |

---

**Tài liệu này được tự động sinh từ CR.Core.Domain entities**  
**Cập nhật lần cuối: 14/05/2026**
