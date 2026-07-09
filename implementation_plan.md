# Hướng Dẫn Thiết Kế Nghiệp Vụ Quản Lý Admin

Tài liệu này là bản **hướng dẫn thiết kế kiến trúc và nghiệp vụ (Business Logic Design Guide)** cho hệ thống Admin của eShop. Tài liệu chỉ đóng vai trò định hướng thiết kế, không tự động can thiệp hay sửa đổi bất kỳ mã nguồn nào trong dự án. 

Bạn có thể dựa vào tài liệu này để tự triển khai mã nguồn hoặc phân chia công việc cho team.

---

## 1. Tổng quan Kiến trúc Admin (Admin Architecture Overview)

Theo tài liệu thiết kế hệ thống, chức năng quản trị (Admin) chia thành 8 phân hệ chính:
1. **Dashboard & Analytics:** Thống kê tổng quan, biểu đồ doanh thu.
2. **Product Management:** Quản lý sản phẩm, hình ảnh, trạng thái.
3. **Category Management:** Quản lý danh mục sản phẩm.
4. **Order Management:** Quản lý đơn hàng, vận chuyển, hoàn tiền.
5. **Customer Management:** Quản lý người dùng, phân quyền.
6. **Inventory Management:** Quản lý tồn kho, cảnh báo hết hàng.
7. **Coupon Management:** Quản lý mã giảm giá.
8. **Review Management:** Quản lý đánh giá từ khách hàng.

> [!TIP]
> **Khuyến nghị thiết kế (Design Pattern):**
> - Đối với API: Nên giữ kiến trúc **Application Services** hiện hành. Các chức năng của Admin có thể gộp chung vào Service hiện tại (ví dụ: `IProductService`) nhưng cần tách biệt **Endpoint (Controller)** thành khu vực `/api/admin/...` và gắn cờ `[Authorize(Roles = "Admin")]`.
> - DTOs: Nên tách biệt các DTO dùng cho Admin (ví dụ: `ProductAdminResponseDto`) để tránh rò rỉ dữ liệu nhạy cảm ra ngoài (khách hàng không cần thấy lịch sử nhập giá, tồn kho thực tế).

---

## 2. Chi tiết Thiết kế từng Phân hệ (Module Design)

### 2.1. Phân hệ Thống kê (Analytics / Dashboard)
Nghiệp vụ:
- **GetDashboardSummary:** Lấy số liệu tổng quan (Tổng doanh thu, số lượng đơn hàng, số khách hàng đăng ký mới).
- **GetRevenueChart:** Lấy dữ liệu dạng chuỗi thời gian (Time-series) để vẽ biểu đồ doanh thu (theo Ngày/Tuần/Tháng).
- **GetTopSellingProducts:** Lấy danh sách sản phẩm bán chạy nhất trong một khoảng thời gian.

### 2.2. Phân hệ Sản phẩm (Product Management)
Nghiệp vụ:
- **Create/Update Product:** Ngoài các trường cơ bản, Admin cần cập nhật `BasePrice`, `CostPrice` (giá vốn), `Status` (Draft, Published, Hidden).
- **Soft Delete:** Không xóa cứng (Hard Delete) dữ liệu trong database, thay vào đó cập nhật trường `IsDeleted = true`.
- **Image Gallery:** Quản lý thêm/xóa nhiều ảnh cho một sản phẩm.

### 2.3. Phân hệ Đơn hàng (Order Management)
Nghiệp vụ:
- **Filter & Search:** Lọc đơn hàng theo trạng thái (Pending, Processing, Shipped, Delivered, Cancelled), khoảng ngày, và tìm theo số điện thoại khách hàng.
- **Order Processing:** Admin chuyển trạng thái đơn hàng. Nếu chuyển sang `Shipped`, cần bắt buộc điền `TrackingNumber` (Mã vận đơn) và `ShippingProvider` (Đơn vị vận chuyển).
- **Refund Logic:** Nếu đơn hàng đã thanh toán bị hủy, hệ thống cần ghi nhận trạng thái `Refunded` (Hoàn tiền).

### 2.4. Phân hệ Người dùng (Customer Management)
Nghiệp vụ:
- **Get All Users:** Lấy danh sách toàn bộ khách hàng, có phân trang.
- **Toggle User Status:** Admin có quyền Khóa (Ban) hoặc Mở khóa tài khoản. Khi Khóa cần cung cấp lý do (`Reason`).
- **User Activity:** Xem chi tiết một khách hàng: Tổng số tiền đã tiêu, danh sách đơn hàng đã mua, lần đăng nhập cuối cùng.

### 2.5. Phân hệ Tồn kho (Inventory Management)
Nghiệp vụ:
- **Stock Adjustment:** Nhập/Xuất kho thủ công bằng tay (ví dụ: hàng hư hỏng, kiểm kê kho). Yêu cầu lưu lại lịch sử thay đổi kho (`StockHistory`).
- **Low Stock Alert:** API trả về danh sách các sản phẩm có số lượng tồn kho thấp hơn mức cho phép (ví dụ: < 10) để Admin có kế hoạch nhập thêm hàng.

### 2.6. Phân hệ Đánh giá (Review Management)
Nghiệp vụ:
- **Moderate Reviews:** Admin xem tất cả đánh giá, có thể Ẩn/Xóa (Hide) các đánh giá vi phạm tiêu chuẩn cộng đồng.
- **Reply to Review:** Admin gửi phản hồi (Reply) lại đánh giá của khách hàng.

---

## 3. Các Ràng buộc Hệ thống (System Constraints & Open Decisions)

> [!WARNING]
> Khi tiến hành code, hãy cân nhắc các vấn đề sau:
> 1. **Role & Permission:** Đảm bảo `AuthenticationModule` đã hỗ trợ Role (ví dụ claim `Role = "Admin"`). Nếu chưa, bạn cần thiết kế lại JWT Token để chứa Role.
> 2. **Audit Logging:** Các thao tác nhạy cảm (Xóa sản phẩm, Khóa người dùng, Hủy đơn hàng) nên được ghi log lại (Ai làm? Làm lúc nào? Dữ liệu cũ là gì?) vào bảng `AuditLogs`.
> 3. **Concurrency:** Khi Admin cập nhật số lượng tồn kho (Inventory) cùng lúc khách hàng đang đặt hàng (Order), cần sử dụng Database Transaction hoặc Optimistic Concurrency Control để tránh sai lệch dữ liệu.
