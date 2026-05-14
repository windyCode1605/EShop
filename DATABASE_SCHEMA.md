# E-Shop Database Schema

## Danh sách các bảng và thuộc tính

### 1. **Users** (Bảng người dùng)

**Thuộc tính:**
- `Id` (int, PK, Identity)
- `Username` (string, Max 50, Unique, Required)
- `Email` (string, Max 128, Unique, Required)
- `Phone` (string, Max 20, Optional)
- `PasswordHash` (string, Max 256, Required)
- `IsTempPassword` (bool)
- `PinCode` (string, Max 128, Optional)
- `IsTempPin` (bool)
- `UserType` (Enum)
- `Status` (int)
- `IsFirstTime` (bool)
- `LoginFailCount` (int)
- `DateTimeLoginFailCount` (DateTime, Optional)
- `TimeLockUser` (DateTime, Optional)
- `LastLogin` (DateTime, Optional)
- `CreatedDate` (DateTime, Optional)
- `ModifiedDate` (DateTime, Optional)
- `DeletedDate` (DateTime, Optional)
- `Deleted` (bool)
- `RowVersion` (byte[], Timestamp)

**Indexes:**
- `IX_Users_Email` (Unique)
- `IX_Users_Username` (Unique)

---

### 2. **UserProfile** (Hồ sơ người dùng)

**Thuộc tính:**
- `Id` (int, PK, Identity)
- `UserId` (int, FK → Users)
- `FullName` (string, Optional)
- `DateOfBirth` (DateTime, Optional)
- `Gender` (Enum, Optional)
- `AvatarUrl` (string, Optional)

**Relationship:**
- **1:1** với Users

---

### 3. **Role** (Vai trò)

**Thuộc tính:**
- `Id` (int, PK, Identity)
- `Name` (string, Max 50, Required) - ADMIN, STAFF, CUSTOMER
- `Description` (string, Max 256, Optional)
- `Status` (int)
- `CreatedDate` (DateTime, Optional)
- `ModifiedDate` (DateTime, Optional)
- `DeletedDate` (DateTime, Optional)
- `Deleted` (bool)

---

### 4. **UserRole** (Gán vai trò cho người dùng - Junction Table)

**Thuộc tính:**
- `Id` (int, PK, Identity)
- `UserId` (int, FK → Users, Required)
- `RoleId` (int, FK → Role, Required)
- `CreatedDate` (DateTime, Optional)
- `ModifiedDate` (DateTime, Optional)
- `DeletedDate` (DateTime, Optional)
- `Deleted` (bool)

**Indexes:**
- `IX_UserRole` (UserId, RoleId, Deleted)

**Relationships:**
- **N:N** giữa Users và Role (Many-to-Many)

---

### 5. **RolePermission** (Quyền hạn của vai trò)

**Thuộc tính:**
- `Id` (int, PK, Identity)
- `RoleId` (int, FK → Role)
- `PermissionKey` (string, Max 128, Required, Unicode(false))
- `CreatedDate` (DateTime, Optional)
- `CreatedBy` (int, Optional)

**Relationship:**
- **N:1** với Role

---

### 6. **Category** (Danh mục sản phẩm)

**Thuộc tính:**
- `Id` (int, PK, Identity)
- `ParentId` (int, Optional, FK → Category)
- `Name` (string, Max 256, Required)
- `Slug` (string, Max 256, Required, Unique, Unicode(false))
- `CreatedDate` (DateTime, Optional)
- `ModifiedDate` (DateTime, Optional)
- `DeletedDate` (DateTime, Optional)
- `Deleted` (bool)

**Indexes:**
- `IX_Category_Slug` (Unique)

**Relationships:**
- **Self-referencing** (1:N): Hỗ trợ danh mục cấp cha - cấp con (hierarchical categories)

---

### 7. **Product** (Sản phẩm)

**Thuộc tính:**
- `Id` (int, PK, Identity)
- `CategoryId` (int, FK → Category, Required)
- `Name` (string, Max 256, Required)
- `Slug` (string, Max 256, Required, Unique, Unicode(false))
- `BasePrice` (decimal(18,2), Required)
- `Description` (string, Optional)
- `CreatedDate` (DateTime, Optional)
- `ModifiedDate` (DateTime, Optional)
- `DeletedDate` (DateTime, Optional)
- `Deleted` (bool)

**Indexes:**
- `IX_Product_Slug` (Unique)
- `IX_Product_CategoryId`

**Relationships:**
- **N:1** với Category (Restrict delete)
- **1:N** với ProductVariant (Cascade delete)
- **1:N** với ProductImage (Cascade delete)
- **1:N** với Reviews (Cascade delete)

---

### 8. **ProductImage** (Hình ảnh sản phẩm)

**Thuộc tính:**
- `Id` (int, PK, Identity)
- `ProductId` (int, FK → Product, Required)
- `Url` (string, Max 500, Required)
- `SortOrder` (int)
- `IsPrimary` (bool)
- `CreatedDate` (DateTime, Optional)
- `ModifiedDate` (DateTime, Optional)
- `DeletedDate` (DateTime, Optional)
- `Deleted` (bool)

**Relationship:**
- **N:1** với Product (Cascade delete)

---

### 9. **ProductVariant** (Phiên bản sản phẩm - Size, Color, v.v.)

**Thuộc tính:**
- `Id` (int, PK, Identity)
- `ProductId` (int, FK → Product, Required)
- `SKU` (string, Max 100, Required, Unique, Unicode(false))
- `Size` (string, Max 50, Optional)
- `Color` (string, Max 50, Optional)
- `PriceAdjustment` (decimal(18,2))
- `StockQuantity` (int)
- `CreatedDate` (DateTime, Optional)
- `ModifiedDate` (DateTime, Optional)
- `DeletedDate` (DateTime, Optional)
- `Deleted` (bool)
- `RowVersion` (byte[], Timestamp)

**Indexes:**
- `IX_ProductVariant_SKU` (Unique)

**Relationships:**
- **N:1** với Product

**Query Filter:**
- Không hiển thị ProductVariant bị xóa hoặc có Product bị xóa

---

### 10. **Cart** (Giỏ hàng)

**Thuộc tính:**
- `Id` (int, PK, Identity)
- `UserId` (int, FK → Users, Unique, Required)
- `LastUpdatedAt` (DateTime, Required)

**Indexes:**
- `IX_Cart_UserId` (Unique)

**Relationship:**
- **1:1** với Users (Cascade delete)

---

### 11. **CartItem** (Mục trong giỏ hàng)

**Thuộc tính:**
- `Id` (int, PK, Identity)
- `CartId` (int, FK → Cart, Required)
- `ProductVariantId` (int, FK → ProductVariant, Required)
- `Quantity` (int, Required)

**Relationship:**
- **N:1** với Cart (Cascade delete)
- **N:1** với ProductVariant (Cascade delete)

**Query Filter:**
- Không hiển thị items có ProductVariant bị xóa hoặc Product bị xóa

---

### 12. **Order** (Đơn hàng)

**Thuộc tính:**
- `Id` (int, PK, Identity)
- `OrderCode` (string, Max 50, Required, Unique, Unicode(false))
- `UserId` (int, FK → Users, Required)
- `ShippingAddress` (string, Max 1024, Required)
- `TotalAmount` (decimal(18,2))
- `Status` (string, Max 50, Required, Unicode(false))
- `PaymentMethod` (string, Max 50, Required, Unicode(false))
- `CreatedDate` (DateTime, Optional)
- `ModifiedDate` (DateTime, Optional)
- `DeletedDate` (DateTime, Optional)
- `Deleted` (bool)
- `RowVersion` (byte[], Timestamp)

**Indexes:**
- `IX_Order_OrderCode` (Unique)

**Relationships:**
- **N:1** với Users
- **1:N** với OrderItem (Cascade delete)
- **1:N** với Payment (Cascade delete)

**Query Filter:**
- Không hiển thị Order bị xóa

---

### 13. **OrderItem** (Mục trong đơn hàng)

**Thuộc tính:**
- `Id` (int, PK, Identity)
- `OrderId` (int, FK → Order, Required)
- `ProductVariantId` (int, FK → ProductVariant, Required)
- `Quantity` (int)
- `ProductName` (string, Max 256, Required)
- `VariantSKU` (string, Max 100, Required, Unicode(false))
- `UnitPrice` (decimal(18,2))

**Relationships:**
- **N:1** với Order (Cascade delete)
- **N:1** với ProductVariant (Restrict delete)
- **1:N** với OrderRefund (Cascade delete)

**Query Filter:**
- Không hiển thị items có ProductVariant bị xóa hoặc Product bị xóa

---

### 14. **OrderRefund** (Hoàn hàng/Hoàn tiền)

**Thuộc tính:**
- `Id` (int, PK, Identity)
- `OrderItemId` (int, FK → OrderItem, Required)
- `RefundQuantity` (int)
- `Reason` (string, Max 500, Optional)
- `Status` (string, Max 50, Unicode(false), Default: "PENDING")
- `CreatedDate` (DateTime, Optional)
- `ModifiedDate` (DateTime, Optional)
- `DeletedDate` (DateTime, Optional)
- `Deleted` (bool)

**Relationship:**
- **N:1** với OrderItem (Cascade delete)

---

### 15. **Payment** (Thanh toán)

**Thuộc tính:**
- `Id` (int, PK, Identity)
- `OrderId` (int, FK → Order, Required)
- `Method` (string, Required, Default: PaymentMethod.Cash)
- `Status` (string, Max 50, Required, Default: PaymentStatus.Pending)
- `Amount` (decimal(18,2))
- `PaidAt` (DateTime, Optional) - Thời điểm thanh toán thành công

**Relationship:**
- **N:1** với Order

---

### 16. **Addresses** (Địa chỉ giao hàng)

**Thuộc tính:**
- `Id` (int, PK, Identity)
- `UserId` (int, Optional, FK → Users)
- `Street` (string, Max 200, Required)
- `City` (string, Max 100, Required)
- `Province` (string, Max 100, Required)
- `IsDefault` (bool, Default: false)
- `CreatedDate` (DateTime, Optional)
- `ModifiedDate` (DateTime, Optional)
- `DeletedDate` (DateTime, Optional)
- `Deleted` (bool)

**Relationship:**
- **N:1** với Users

---

### 17. **Reviews** (Đánh giá sản phẩm)

**Thuộc tính:**
- `Id` (int, PK, Identity)
- `UserId` (int, FK → Users, Required)
- `ProductId` (int, FK → Product, Required)
- `Rating` (int, Range: 1-5)
- `Comment` (string, Required)
- `CreatedAt` (DateTime, Default: DateTime.UtcNow)

**Relationships:**
- **N:1** với Users
- **N:1** với Product (Cascade delete)

---

### 18. **AuthOtp** (OTP xác thực)

**Thuộc tính:**
- `Id` (int, PK, Identity)
- `OtpCode` (string, Max 128, Required, Unicode(false)) - Mã OTP đã mã hóa (hash)
- `ExpireTime` (DateTime, Required) - Thời gian hết hạn
- `IsUsed` (bool) - Đánh dấu OTP đã được sử dụng
- `UserId` (int, FK → Users, Required)
- `VerifyTime` (int) - Số lần verify

**Relationship:**
- **N:1** với Users

---

### 19. **Coupons** (Mã giảm giá)

**Thuộc tính:**
- `Id` (int, PK, Identity)
- `Code` (string, Required, Unique) - Mã coupon (VD: SALE50, FREESHIP)
- `DiscountType` (Enum) - Percentage hoặc Fixed
- `DiscountValue` (decimal) - Giá trị giảm (% hoặc tiền)
- `MinOrderValue` (decimal, Optional) - Giá trị đơn hàng tối thiểu
- `MaxDiscountValue` (decimal, Optional) - Tiền giảm tối đa (chỉ với Percentage)
- `StartDate` (DateTime) - Thời điểm bắt đầu
- `ExpiryDate` (DateTime) - Thời điểm hết hạn
- `UsageLimit` (int, Optional) - Tổng số lần sử dụng toàn cộng
- `UsedCount` (int, Default: 0) - Số lần đã sử dụng
- `UsageLimitPerUser` (int, Optional) - Giới hạn sử dụng theo user
- `IsActive` (bool, Default: true) - Trạng thái hoạt động
- `CreatedDate` (DateTime, Optional)
- `ModifiedDate` (DateTime, Optional)
- `DeletedDate` (DateTime, Optional)
- `Deleted` (bool)

**Indexes:**
- `IX_Coupons_Code` (Unique)

---

### 20. **NotificationToken** (Token thông báo)

**Thuộc tính:**
- `Id` (int, PK, Identity)
- `UserId` (int, FK → Users)
- [Các field khác từ base entity...]

**Relationship:**
- **N:1** với Users

---

### 21. **CouponUsage** (Lịch sử sử dụng mã giảm giá) ⭐ **NEW**

**Thuộc tính:**
- `Id` (int, PK, Identity)
- `CouponId` (int, FK → Coupons, Required)
- `UserId` (int, FK → Users, Required)
- `OrderId` (int, FK → Order, Required)
- `DiscountAmount` (decimal(18,2), Required) - Số tiền thực tế đã được giảm cho đơn hàng
- `UsedAt` (DateTime, Default: DateTime.UtcNow) - Thời điểm áp dụng mã

**Indexes:**
- `IX_CouponUsage_UserId_CouponId` - Hỗ trợ truy vấn cực nhanh: "User này đã xài mã này bao nhiêu lần?"

**Relationships:**
- **N:1** với Coupons (Restrict delete - không cho xóa Coupon nếu đã có người xài)
- **N:1** với Users (Cascade - khi xóa User thì xóa luôn lịch sử xài coupon)
- **N:1** với Order (Cascade - khi hủy đơn hàng thì xóa vết xài Coupon để user xài lại)

**Ý nghĩa kinh doanh:**
- Ghi vết lịch sử sử dụng mã giảm giá để kiểm tra `UsageLimitPerUser`
- Audit trail để đối soát kế toán xem hệ thống đã "đốt" bao nhiêu tiền cho các chiến dịch Marketing

---

### 22. **Shipment** (Vận đơn / Thông tin giao hàng) ⭐ **NEW**

**Thuộc tính:**
- `Id` (int, PK, Identity)
- `OrderId` (int, FK → Order, Required)
- `ShippingProvider` (string, Max 100, Required) - VD: "GHN", "NinjaVan", "GrabExpress", "Viettel Post"
- `TrackingNumber` (string, Max 100, Optional, Unicode(false)) - Mã vận đơn do đối tác vận chuyển cấp
- `ShippingFee` (decimal(18,2), Required) - Phí vận chuyển thực tế
- `ReceiverName` (string, Max 256, Required) - Tên người nhận (Khách có thể đặt mua hộ người khác)
- `ReceiverPhone` (string, Max 20, Required) - Số điện thoại người nhận
- `ShippingAddress` (string, Max 1024, Required) - Địa chỉ giao hàng (có thể khác Order nếu tách kiện)
- `EstimatedDelivery` (DateTime, Optional) - Ngày dự kiến giao
- `ActualDelivery` (DateTime, Optional) - Ngày giao thành công
- `Status` (string, Max 50, Required, Unicode(false), Default: "PENDING")
  - Giá trị: `PENDING`, `PICKED_UP`, `IN_TRANSIT`, `DELIVERED`, `FAILED`, `RETURNED`
- `CreatedDate` (DateTime, Optional)
- `ModifiedDate` (DateTime, Optional)
- `DeletedDate` (DateTime, Optional)
- `Deleted` (bool)

**Indexes:**
- `IX_Shipment_TrackingNumber` - Hỗ trợ tra cứu vận đơn nhanh

**Relationships:**
- **N:1** với Order (Cascade - 1 Order → 1 hoặc nhiều Shipment)

**Ý nghĩa kinh doanh:**
- Tách riêng khỏi Order: Order chỉ đóng vai trò "chứng từ thương mại" (lưu thanh toán + tổng tiền)
- Hỗ trợ tính năng **Tách kiện hàng (Split Shipments)**: 1 order → nhiều shipment nếu hàng từ kho khác nhau
- Tính năng **Multi-Carrier**: Hỗ trợ nhiều đơn vị vận chuyển (GHN, NinjaVan, v.v.)
- Tracking thực tế: theo dõi trạng thái vận chuyển chi tiết từng kiện hàng

**Query Filter:**
- Không hiển thị Shipment bị xóa

---

## Mối Quan Hệ Giữa Các Bảng (Relationships)

### **Sơ đồ tổng quan:**

```
┌─────────────────────────────────────────────────────────────┐
│                    USERS (Người dùng)                       │
├─────────────────────────────────────────────────────────────┤
│ PK: Id | Username | Email | PasswordHash | ... | RowVersion│
└────────────┬────────────────────────────────────────────────┘
             │
             ├─ 1:1 ─→ UserProfile (Hồ sơ)
             │
             ├─ 1:N ─→ UserRole ─→ Role (Vai trò)
             │
             ├─ 1:N ─→ NotificationToken (Token thông báo)
             │
             ├─ 1:1 ─→ Cart (Giỏ hàng)
             │
             ├─ 1:N ─→ Order (Đơn hàng)
             │
             ├─ 1:N ─→ AuthOtp (OTP)
             │
             ├─ 1:N ─→ Addresses (Địa chỉ)
             │
             └─ 1:N ─→ Reviews (Đánh giá)

┌─────────────────────────────────────────┐
│    CATEGORY (Danh mục sản phẩm)         │
├─────────────────────────────────────────┤
│ PK: Id | Name | Slug | ParentId (FK)   │
└─────────┬───────────────────────────────┘
          │
          ├─ Self-referencing: ParentId → Id (Danh mục cấp cha)
          │
          └─ 1:N ─→ Product (Sản phẩm)

┌────────────────────────────────────────────────┐
│         PRODUCT (Sản phẩm)                     │
├────────────────────────────────────────────────┤
│ PK: Id | Name | BasePrice | CategoryId (FK)   │
└────────┬──────────────────────────────────────┘
         │
         ├─ N:1 ← Category (Danh mục)
         │
         ├─ 1:N → ProductVariant (Phiên bản: Size, Color)
         │
         ├─ 1:N → ProductImage (Hình ảnh)
         │
         └─ 1:N → Reviews (Đánh giá)

┌──────────────────────────┐
│  PRODUCTVARIANT (SKU)    │
├──────────────────────────┤
│ PK: Id | SKU | ProductId │
└──────┬───────────────────┘
       │
       └─ N:1 ← Product

┌─────────────────────────┐
│   PRODUCTIMAGE          │
├─────────────────────────┤
│ PK: Id | ProductId (FK) │
└──────┬──────────────────┘
       │
       └─ N:1 ← Product

┌────────────────────────────┐
│  CART (Giỏ hàng)          │
├────────────────────────────┤
│ PK: Id | UserId (FK,UNQ)  │
└────────┬───────────────────┘
         │
         ├─ 1:1 ← Users
         │
         └─ 1:N → CartItem

┌──────────────────────────────────────────┐
│  CARTITEM                                 │
├──────────────────────────────────────────┤
│ PK: Id | CartId (FK) | ProductVariantId  │
└──────┬───────────────────────────────────┘
       │
       ├─ N:1 ← Cart
       │
       └─ N:1 ← ProductVariant

┌─────────────────────────────────────┐
│  ORDER (Đơn hàng)                   │
├─────────────────────────────────────┤
│ PK: Id | OrderCode | UserId (FK)    │
└────────┬──────────────────────────────┘
         │
         ├─ N:1 ← Users
         │
         ├─ 1:N → OrderItem
         │
         └─ 1:N → Payment

┌──────────────────────────────────────────┐
│  ORDERITEM                                │
├──────────────────────────────────────────┤
│ PK: Id | OrderId (FK) | ProductVariantId │
└──────┬───────────────────────────────────┘
       │
       ├─ N:1 ← Order
       │
       ├─ N:1 ← ProductVariant
       │
       └─ 1:N → OrderRefund

┌─────────────────────┐
│  ORDERREFUND        │
├─────────────────────┤
│ PK: Id | OrderItemId│
└──────┬──────────────┘
       │
       └─ N:1 ← OrderItem

┌────────────────────┐
│  PAYMENT           │
├────────────────────┤
│ PK: Id | OrderId   │
└──────┬─────────────┘
       │
       └─ N:1 ← Order

┌──────────────────────┐
│  ADDRESSES           │
├──────────────────────┤
│ PK: Id | UserId (FK) │
└──────┬───────────────┘
       │
       └─ N:1 ← Users

┌──────────────────────┐
│  REVIEWS             │
├──────────────────────┤
│ PK: Id | UserId (FK) │
│       | ProductId(FK)│
└──────┬───────────────┘
       │
       ├─ N:1 ← Users
       │
       └─ N:1 ← Product

┌──────────────────────┐
│  AUTHOTP             │
├──────────────────────┤
│ PK: Id | UserId (FK) │
└──────┬───────────────┘
       │
       └─ N:1 ← Users

┌──────────────────────┐
│  COUPONS             │
├──────────────────────┤
│ PK: Id | Code (UNQ) │
└──────┬───────────────┘
       │
       └─ 1:N ← CouponUsage ⭐

┌────────────────────────────────────────────┐
│  COUPONUSAGE ⭐ (Ghi vết sử dụng mã)        │
├────────────────────────────────────────────┤
│ PK: Id | CouponId (FK) | UserId (FK)       │
│       | OrderId (FK) | DiscountAmount      │
└────────────────────────────────────────────┘
       │
       ├─ N:1 ← Coupons (Restrict)
       ├─ N:1 ← Users (Cascade)
       └─ N:1 ← Order (Cascade)

┌─────────────────────────────────────────────────────┐
│  SHIPMENT ⭐ (Vận đơn - hỗ trợ Split Shipment)      │
├─────────────────────────────────────────────────────┤
│ PK: Id | OrderId (FK) | ShippingProvider           │
│       | TrackingNumber | ShippingFee                │
│       | ReceiverName | Status | ...                │
└─────────────────────────────────────────────────────┘
       │
       └─ N:1 ← Order (Cascade)
              (1 Order → 1 hoặc nhiều Shipment)

┌──────────────────────┐
│  ROLE                │
├──────────────────────┤
│ PK: Id | Name       │
└──────┬───────────────┘
       │
       ├─ 1:N ← UserRole (N:N với Users)
       │
       └─ 1:N → RolePermission

┌──────────────────────┐
│  USERROLE (N:N)      │
├──────────────────────┤
│ PK: Id | UserId (FK) │
│       | RoleId (FK)  │
└──────────────────────┘

┌──────────────────────┐
│  ROLEPERMISSION      │
├──────────────────────┤
│ PK: Id | RoleId (FK) │
│       | PermissionKey│
└──────┬───────────────┘
       │
       └─ N:1 ← Role
```

---

## Tóm Tắt Mối Quan Hệ

| Bảng | Quan Hệ | Bảng Liên Kết | Delete Behavior |
|------|---------|---------------|-----------------|
| **Users** | 1:1 | UserProfile | - |
| **Users** | 1:N | UserRole | - |
| **Users** | 1:1 | Cart | Cascade |
| **Users** | 1:N | Order | - |
| **Users** | 1:N | NotificationToken | - |
| **Users** | 1:N | AuthOtp | - |
| **Users** | 1:N | Addresses | - |
| **Users** | 1:N | Reviews | - |
| **Category** | Self-Ref | Category | Restrict |
| **Category** | 1:N | Product | - |
| **Product** | N:1 | Category | Restrict |
| **Product** | 1:N | ProductVariant | Cascade |
| **Product** | 1:N | ProductImage | Cascade |
| **Product** | 1:N | Reviews | Cascade |
| **ProductVariant** | N:1 | Product | - |
| **ProductVariant** | 1:N | CartItem | Cascade |
| **ProductVariant** | 1:N | OrderItem | Cascade |
| **Cart** | 1:1 | Users | Cascade |
| **Cart** | 1:N | CartItem | Cascade |
| **CartItem** | N:1 | ProductVariant | Cascade |
| **Order** | N:1 | Users | - |
| **Order** | 1:N | OrderItem | Cascade |
| **Order** | 1:N | Payment | Cascade |
| **OrderItem** | N:1 | Order | Cascade |
| **OrderItem** | N:1 | ProductVariant | Restrict |
| **OrderItem** | 1:N | OrderRefund | Cascade |
| **OrderRefund** | N:1 | OrderItem | Cascade |
| **Payment** | N:1 | Order | - |
| **Addresses** | N:1 | Users | - |
| **Reviews** | N:1 | Users | - |
| **Reviews** | N:1 | Product | Cascade |
| **AuthOtp** | N:1 | Users | - |
| **Coupons** | - | - | - | Bảng độc lập (chưa có FK) |
| **CouponUsage** ⭐ | N:1 | Coupons | Restrict | Ghi vết sử dụng mã |
| **CouponUsage** ⭐ | N:1 | Users | Cascade | - |
| **CouponUsage** ⭐ | N:1 | Order | Cascade | - |
| **Shipment** ⭐ | N:1 | Order | Cascade | Hỗ trợ tách kiện hàng (Split Shipment) |
| **Role** | 1:N | UserRole | - |
| **Role** | 1:N | RolePermission | - |
| **UserRole** | N:1 | Users | - |
| **UserRole** | N:1 | Role | - |
| **RolePermission** | N:1 | Role | - |

---

## Query Filters

- **Product**: `HasQueryFilter(p => !p.Deleted)`
- **ProductVariant**: `HasQueryFilter(v => !v.Deleted && !v.Product.Deleted)`
- **CartItem**: `HasQueryFilter(ci => !ci.ProductVariant.Product.Deleted && !ci.ProductVariant.Deleted)`
- **OrderItem**: `HasQueryFilter(oi => !oi.ProductVariant.Product.Deleted && !oi.ProductVariant.Deleted)`
- **Order**: `HasQueryFilter(o => !o.Deleted)`

---

## 📋 TRẠNG THÁI CẤU HÌNH VÀ SỬ DỤNG CÁC BẢNG

### ✅ **Bảng đã thiết lập (Configuration) + Đầy đủ mối quan hệ:**
| # | Bảng | Configuration File | Trạng thái | Ghi chú |
|---|------|--------------------|-----------|---------|
| 1 | **Users** | ✓ UserConfiguration.cs | ✓ Đầy đủ | Bảng chính, có nhiều FK tới các bảng khác |
| 2 | **Category** | ✓ CategoryConfiguration.cs | ✓ Đầy đủ | Self-referencing (cha-con), FK → Product |
| 3 | **Product** | ✓ ProductConfiguration.cs | ✓ Đầy đủ | FK → Category, 1:N → Image/Variant/Review |
| 4 | **ProductVariant** | ✓ ProductVariantsConfiguration.cs | ✓ Đầy đủ | FK → Product, có Query Filter |
| 5 | **ProductImage** | ✓ ProductImageConfiguration.cs | ✓ Đầy đủ | FK → Product (Cascade), có Query Filter |
| 6 | **Cart** | ✓ CartConfiguration.cs | ✓ Đầy đủ | 1:1 với Users (Cascade), 1:N → CartItem |
| 7 | **CartItem** | ✓ CartItemConfiguration.cs | ✓ Đầy đủ | FK → Cart, ProductVariant, có Query Filter |
| 8 | **Order** | ✓ OrderConfiguration.cs | ✓ Đầy đủ | FK → Users, 1:N → OrderItem/Payment, Query Filter |
| 9 | **OrderItem** | ✓ OrderItemConfiguration.cs | ✓ Đầy đủ | FK → Order/ProductVariant, 1:N → Refund, Query Filter |
| 10 | **Payment** | ✓ PaymentConfiguration.cs | ✓ Đầy đủ | FK → Order (Cascade) |
| 11 | **Addresses** | ✓ AddressConfiguration.cs | ✓ Đầy đủ | FK → Users (SetNull), 1:N relationship |
| 12 | **Reviews** | ✓ ReviewsConfiguration.cs | ✓ Đầy đủ | FK → Users/Product, Query Filter |
| 13 | **AuthOtp** | ✓ AuthOtpConfiguration.cs | ✓ Đầy đủ | FK → Users (Cascade) |
| 14 | **Coupons** | ✓ CouponsConfiguration.cs | ✓ Cơ bản | ⚠️ **Chưa có FK nối** - Là bảng độc lập, chưa liên kết với đơn hàng/giỏ hàng |
| 15 | **CouponUsage** ⭐ | ✓ CouponUsageConfiguration.cs | ✓ Đầy đủ | Ghi vết sử dụng mã, FK → Coupons/Users/Order |
| 16 | **Shipment** ⭐ | ✓ ShipmentConfiguration.cs | ✓ Đầy đủ | Hỗ trợ tách kiện hàng (Split Shipment), FK → Order |

---

### ⚠️ **Bảng có Configuration nhưng Chưa hoàn chỉnh (Missing Relationship Setup):**
| # | Bảng | Configuration File | Trạng thái | Vấn đề |
|---|------|--------------------|-----------|--------|
| 17 | **OrderRefund** | ✓ OrderRefundConfiguration.cs | ⚠️ Bất đầy đủ | Configuration chỉ có Query Filter, **chưa setup FK** trong config (FK được định nghĩa ở Entity nhưng chưa configure HasOne().WithMany()) |

---

### ❌ **Bảng có Entity nhưng Chưa có Configuration (Chưa dùng / Chưa thiết lập):**
| # | Bảng | Entity File | Trạng thái | Ghi chú |
|---|------|------------|-----------|---------|
| 18 | **UserProfile** | ✓ UserProfile.cs | ❌ Chưa config | Có định nghĩa FK UserId → Users, nhưng chưa có IEntityTypeConfiguration |
| 19 | **Role** | ✓ Role.cs | ❌ Chưa config | Định nghĩa cơ bản, nhưng chưa thiết lập relationship với UserRole/RolePermission |
| 20 | **UserRole** | ✓ UserRole.cs | ❌ Chưa config | N:N junction table (Users ↔ Role), nhưng chưa có configuration |
| 21 | **RolePermission** | ✓ RolePermission.cs | ❌ Chưa config | FK → Role, nhưng chưa có configuration |
| 22 | **NotificationToken** | ✓ Được định nghĩa trong Users | ❌ Chưa config | Chỉ có navigation property ở Users, chưa có entity/config riêng |
| 23 | **SendOtp** | ✓ SendOtp.cs | ❌ Chưa config | **Chưa có FK** - Bảng độc lập, chưa liên kết với Users |
| 24 | **SysVar** | ✓ SysVar.cs | ❌ Chưa config | **Chưa có FK** - Bảng cấu hình toàn cục, độc lập |

---

## 🔍 TÓM TẮT NHANH

| Tiêu chỉ | Số lượng | Ghi chú |
|---------|---------|--------|
| **Tổng cộng bảng** | 24 | Thêm CouponUsage + Shipment |
| **✓ Đã config + Đầy đủ** | 15 | ⭐ CouponUsage + Shipment mới |
| **⚠️ Config nhưng chưa hoàn chỉnh** | 1 | OrderRefund: cần setup FK trong config |
| **❌ Chưa config** | 8 | UserProfile, Role, UserRole, RolePermission, NotificationToken, SendOtp, SysVar |
| **Bảng có FK chưa config** | 4 | UserProfile, UserRole, RolePermission, SendOtp |
| **Bảng độc lập (không FK)** | 3 | Coupons, SendOtp, SysVar |

---

## 📝 DANH SÁCH TODO - CẦN XỬ LÝ

### Priority 1: Khẩn cấp (Ảnh hưởng đến Logic Chính)
- [x] **CouponUsage** ⭐ - ✅ Hoàn thành: Entity + Configuration + Relationship (FK → Coupons/Users/Order)
- [x] **Shipment** ⭐ - ✅ Hoàn thành: Entity + Configuration + Relationship (FK → Order)
- [ ] **SendOtp** - Cần FK → Users hoặc cấu hình lại logic OTP
- [ ] **UserProfile** - Cần Configuration để thiết lập 1:1 relationship với Users
- [ ] **OrderRefund** - Cần setup FK `.HasOne(r => r.OrderItem).WithMany()` trong OrderRefundConfiguration

### Priority 2: Quan trọng (Hệ thống Phân quyền)
- [ ] **Role** - Cần Configuration với relationships tới UserRole/RolePermission
- [ ] **UserRole** - Cần Configuration để thiết lập N:N giữa Users ↔ Role
- [ ] **RolePermission** - Cần Configuration FK → Role

### Priority 3: Bổ sung (Có thể để sau)
- [ ] **Coupons** - Cân nhắc thêm FK nếu muốn theo dõi ai tạo/dùng coupon
- [ ] **SysVar** - Không cần FK (bảng cấu hình toàn cục)
- [ ] **NotificationToken** - Xem xét extracting thành entity riêng hoặc giữ như hiện tại
