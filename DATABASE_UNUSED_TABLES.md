# 📋 BẢNG CHƯA SỬ DỤNG / PENDING REVIEW

## ⚠️ DANH SÁCH CẢN CLEANUP & REVIEW

### 1. **PermissionMax** ❌ (Chưa dùng)
**Vị trí:** `CR.Core.Domain/Permission/PermissionMax.cs`

```csharp
public class PermissionMax
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public required string PermissionKey { get; set; }
}
```

**Lý do chưa dùng:**
- Mục đích không rõ ràng
- Không có navigation property hoặc relationship
- Khác biệt với `CustomerRolePermission` không rõ

**Quyết định:**
- ❓ Cần clarify: Dùng để lưu quyền tối đa hay cắt mức quyền?
- 🗑️ Nếu không dùng → xóa

---

### 2. **CustomerRole, CustomerUserRole, CustomerRolePermission** ⚠️ (Overlap với Role)
**Vị trí:** `CR.Core.Domain/Permission/`

```csharp
// Hiện tại đã có Role → RolePermission → UserRole
// Nhưng còn có thêm CustomerRole → CustomerRolePermission → CustomerUserRole
```

**Lý do chưa dùng:**
- Overlap 100% với Role system
- Cách naming không rõ "Customer" vs "Role"
- Tăng complexity và duplicate logic

**Lưu ý:**
```
Role system:
  ├─ Role (ADMIN, STAFF, CUSTOMER)
  ├─ RolePermission (Role → Permission)
  └─ UserRole (User → Role)

Customer system (possibly obsolete?):
  ├─ CustomerRole (custom role for customer)
  ├─ CustomerRolePermission (CustomRole → Permission)
  └─ CustomerUserRole (User → CustomRole)
```

**Quyết định:**
- ❓ Hỏi: Có phải CustomerRole là role tùy chỉnh per-customer không?
- 🔄 Nếu có: cần tài liệu rõ use case
- 🗑️ Nếu không: gộp vào Role system

---

### 3. **NotificationToken.ApnsToken** 🗑️ (Deprecated)
**Vị trí:** `CR.Core.Domain/NotificationToken.cs/NotificationToken.cs`

```csharp
[MaxLength(128)]
[Unicode(false)]
[Obsolete("bỏ chỉ cần dùng FcmToken")]
public string? ApnsToken { get; set; }  // ❌ ĐÁNH DẤU OBSOLETE
```

**Lý do chưa dùng:**
- Apple Push Notification Service (APNS) token không dùng
- Dùng `FcmToken` cho cả iOS và Android

**Quyết định:**
- ✅ Xóa `ApnsToken` hoàn toàn (hiện tại đã marked Obsolete)

---

### 4. **SendOtp** ⚠️ (Rate Limiting - Chưa kiểm chứng)
**Vị trí:** `CR.Core.Domain/Opts/SendOtp.cs`

```csharp
public class SendOtp 
{
    public int Id { get; set; }
    public required string Username { get; set; }
    public int SendCount { get; set; } = 0;
    public DateTime LastSentDateTime { get; set; }
    public DateTime TimeLimitCanVerifyOtp { get; set; }
}
```

**Lý do chưa kiểm chứng:**
- Chưa có business logic call tới table này
- Cần verify xem OTP service có dùng không?

**Quyết định:**
- ✅ Giữ lại (dùng cho rate limiting OTP)
- ⚠️ Nhưng cần implementation OTP service gọi tới

---

### 5. **OrderRefund** ⚠️ (Chưa triển khai)
**Vị trí:** `CR.Core.Domain/Orders/OrderRefund.cs`

```csharp
public class OrderRefund : AuditableEntity
{
    public int Id { get; set; }
    public int OrderItemId { get; set; }
    public int RefundQuantity { get; set; }
    public string? Reason { get; set; }
    public string Status { get; set; } = "PENDING";
    public virtual OrderItem OrderItem { get; set; } = null!;
}
```

**Lý do chưa triển khai:**
- Entity đã được tạo nhưng chưa có service/controller
- Chưa có API endpoint xử lý hoàn lại

**Quyết định:**
- ✅ Giữ lại (schema đã sẵn sàng)
- ⚠️ Cần implement refund service & API

---

## 📊 BẢNG KIỂM TRA TÍNH NĂNG

| Bảng | Được Định Nghĩa | Có Service | Có API | Có Sử Dụng | Ghi Chú |
|------|---------|---------|--------|-----------|---------|
| **Category** | ✅ | ✅ | ✅ | ✅ | Hoạt động |
| **Product** | ✅ | ✅ | ✅ | ✅ | Hoạt động |
| **ProductVariant** | ✅ | ✅ | ✅ | ✅ | Hoạt động |
| **ProductImage** | ✅ | ✅ | ✅ | ✅ | Hoạt động |
| **Cart** | ✅ | ✅ | ✅ | ✅ | Hoạt động |
| **CartItem** | ✅ | ✅ | ✅ | ✅ | Hoạt động |
| **Order** | ✅ | ✅ | ✅ | ✅ | Hoạt động |
| **OrderItem** | ✅ | ✅ | ✅ | ✅ | Hoạt động |
| **OrderRefund** | ✅ | ❌ | ❌ | ⚠️ | Chưa triển khai |
| **Payments** | ✅ | ✅ | ✅ | ✅ | Hoạt động |
| **Shipment** | ✅ | ⚠️ | ⚠️ | ⚠️ | Mới thêm |
| **Coupons** | ✅ | ✅ | ✅ | ✅ | Hoạt động |
| **CouponUsage** | ✅ | ⚠️ | ⚠️ | ⚠️ | Mới thêm |
| **Users** | ✅ | ✅ | ✅ | ✅ | Hoạt động |
| **UserProfile** | ✅ | ✅ | ✅ | ✅ | Hoạt động |
| **Addresses** | ✅ | ✅ | ✅ | ✅ | Hoạt động |
| **AuthOtp** | ✅ | ✅ | ✅ | ✅ | Hoạt động |
| **SendOtp** | ✅ | ⚠️ | ⚠️ | ⚠️ | Rate limiting |
| **NotificationToken** | ✅ | ✅ | ⚠️ | ✅ | Hoạt động |
| **Role** | ✅ | ✅ | ✅ | ✅ | Hoạt động |
| **RolePermission** | ✅ | ✅ | ✅ | ✅ | Hoạt động |
| **UserRole** | ✅ | ✅ | ✅ | ✅ | Hoạt động |
| **CustomerRole** | ✅ | ❌ | ❌ | ❌ | Overlap với Role |
| **CustomerUserRole** | ✅ | ❌ | ❌ | ❌ | Overlap với Role |
| **CustomerRolePermission** | ✅ | ❌ | ❌ | ❌ | Overlap với Role |
| **PermissionMax** | ✅ | ❌ | ❌ | ❌ | **Chưa rõ mục đích** |
| **Reviews** | ✅ | ✅ | ✅ | ✅ | Hoạt động |
| **SysVar** | ✅ | ✅ | ✅ | ✅ | Hoạt động |

---

## 🚀 CẦN LÀM (TODO)

### Ngay lập tức (Critical)
- [ ] **Clarify PermissionMax**: Mục đích của table này?
- [ ] **Merge CustomerRole system**: Nó có phải obsolete không?
- [ ] **Remove ApnsToken**: Xóa field deprecated

### Ngắn hạn (This Quarter)
- [ ] **Implement Shipment Service**: 
  - [ ] IntegrationService với GHN, NinjaVan, etc.
  - [ ] API endpoints for shipment tracking
  - [ ] Webhook support from carriers
  
- [ ] **Implement CouponUsage Service**:
  - [ ] API để track coupon usage
  - [ ] Marketing analytics queries
  
- [ ] **Implement OrderRefund Service**:
  - [ ] Refund processing workflow
  - [ ] Refund status tracking
  - [ ] Integration với payment gateway

- [ ] **Implement SendOtp Rate Limiting**:
  - [ ] OTP service call this table
  - [ ] Rate limiting logic

### Trung hạn (Next Quarter)
- [ ] **Database optimization**:
  - [ ] Add missing indexes
  - [ ] Review cascade delete behaviors
  - [ ] Query performance tuning
  
- [ ] **Clean up Permission system**:
  - [ ] Decide on CustomerRole vs Role
  - [ ] Simplify permission hierarchy

---

## 📝 CÁC BẢNG CẢN MIGRATION

### Khi Xóa PermissionMax
```sql
-- Down Migration
DROP TABLE [dbo].[PermissionMaxs];

-- Up Migration  
CREATE TABLE [dbo].[PermissionMaxs] (
    [Id] int NOT NULL IDENTITY,
    [CustomerId] int NOT NULL,
    [PermissionKey] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_PermissionMaxs] PRIMARY KEY ([Id])
);
```

### Khi Xóa ApnsToken
```csharp
// Migration
migrationBuilder.DropColumn(
    name: "ApnsToken",
    table: "NotificationTokens");
```

### Khi Merge CustomerRole vào Role
```sql
-- Consolidate CustomerRole data into Role
INSERT INTO [dbo].[Roles] (Name, Description, Status, CreatedDate)
SELECT DISTINCT Name, Description, Status, CreatedDate 
FROM [dbo].[CustomerRoles]
WHERE NOT EXISTS (
    SELECT 1 FROM [dbo].[Roles] r 
    WHERE r.Name = [dbo].[CustomerRoles].Name
);

-- Delete deprecated tables
DROP TABLE [dbo].[CustomerUserRoles];
DROP TABLE [dbo].[CustomerRolePermissions];
DROP TABLE [dbo].[CustomerRoles];
```

---

## 💡 RECOMMENDATION

**Priority:**
1. ✅ **DELETE** PermissionMax (chưa rõ)
2. ✅ **REMOVE** ApnsToken (obsolete)
3. 🔄 **REVIEW** CustomerRole system (decide merge or keep)
4. ⚠️ **IMPLEMENT** Shipment service (đang pending)
5. ⚠️ **IMPLEMENT** CouponUsage service (đang pending)
6. ⚠️ **IMPLEMENT** OrderRefund service (đang pending)

---

**Generated:** 14/05/2026  
**Status:** 🔴 PENDING REVIEW
