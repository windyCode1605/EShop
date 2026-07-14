# EShop: Modern E-Commerce Platform

[![Build Status](https://img.shields.io/badge/build-passing-brightgreen.svg)]()
[![License](https://img.shields.io/badge/license-MIT-blue.svg)]()
[![.NET Version](https://img.shields.io/badge/.NET-9.0-purple.svg)]()
[![Angular Version](https://img.shields.io/badge/Angular-17%2B-dd1b16.svg)]()

EShop là hệ thống E-commerce hiện đại được xây dựng dựa trên kiến trúc Clean Architecture. Dự án cung cấp một nền tảng bán hàng trực tuyến toàn diện, hiệu năng cao và dễ dàng mở rộng, đáp ứng đầy đủ các nghiệp vụ thương mại điện tử cốt lõi từ quản lý sản phẩm, giỏ hàng đến xử lý đơn đặt hàng.

## Trợ giúp trực quan
> **Lưu ý cho nhà phát triển:** Cập nhật ảnh chụp màn hình UI trang chủ, luồng mua hàng hoặc video/GIF demo của dự án vào phần này.
> 
> `![Demo Home Page](path_to_image_or_gif)`

## Tính năng chính
- **Quản lý danh mục & Sản phẩm:** Hỗ trợ CRUD sản phẩm, phân loại theo danh mục và tìm kiếm nâng cao theo bộ lọc.
- **Giỏ hàng & Thanh toán:** Xử lý giỏ hàng an toàn, luồng checkout mượt mà và tính toán chi phí minh bạch.
- **Quản lý đơn hàng:** Trạng thái đơn hàng được cập nhật theo thời gian thực (Pending, Processing, Shipped).
- **Phân quyền & Bảo mật (RBAC):** Tích hợp xác thực bằng JWT, phân chia quyền hạn rõ ràng giữa Admin và Khách hàng.

## Phần phụ thuộc
Danh sách các thư viện và platform cần thiết để hệ thống hoạt động:
- **Backend:** .NET 9 SDK, Entity Framework Core.
- **Database:** SQL Server (LocalDB hoặc qua Docker).
- **Frontend:** Angular CLI v17+, Node.js.

## Hướng dẫn cài đặt
Yêu cầu thiết bị phải được cài đặt sẵn các phụ thuộc kể trên trước khi bắt đầu.

**1. Clone dự án**
```bash
git clone https://github.com/your-repo/EShop.git
cd EShop
```

**2. Thiết lập Database**
Hệ thống mặc định sử dụng LocalDB `(localdb)\mssqllocaldb` (đã khai báo trong `appsettings.json`). Nếu bạn ưu tiên sử dụng SQL Server qua Docker:
```bash
docker run -e 'ACCEPT_EULA=Y' -e 'SA_PASSWORD=YourPassword123' -p 1433:1433 mcr.microsoft.com/mssql/server:2022-latest
```

**3. Cài đặt Backend**
```bash
cd eShop.API
dotnet restore
dotnet ef database update
dotnet run
```
*API sẽ chạy tại `http://localhost:5000` và Swagger UI tại `/swagger`.*

**4. Cài đặt Frontend**
```bash
cd eShop.Web
npm install
npm start
```
*Giao diện người dùng sẽ được phục vụ tại `http://localhost:4200`.*

## Cách dùng và ví dụ

**Trải nghiệm mua sắm (Shopping Flow)**
- Khách hàng có thể dạo quanh trang chủ, lọc sản phẩm theo danh mục hoặc từ khóa.
- Thêm sản phẩm vào giỏ hàng và tiến hành checkout.
- Sau khi đặt hàng, hệ thống sẽ tự động chuyển trạng thái đơn sang 'Pending' và lưu trữ vào lịch sử mua hàng.

**Quản lý cửa hàng (Admin Panel)**
- Admin đăng nhập bằng tài khoản cấp quản trị.
- Truy cập Dashboard để xem thống kê.
- Thêm, sửa, xóa sản phẩm và phê duyệt/hủy các đơn hàng đang chờ xử lý.

## Giải pháp khắc phục lỗi phổ biến (FAQ)

**Q: Lỗi CORS khi thao tác gọi API từ ứng dụng Angular?**
**A:** Đảm bảo API backend đang chạy ổn định ở cổng 5000 và đã được cấu hình policy CORS hợp lệ trong `Program.cs`.

**Q: Hệ thống báo lỗi từ chối kết nối Database?**
**A:** Kiểm tra chuỗi kết nối (connection string) trong file `appsettings.json`. Nếu đang dùng Docker, hãy đảm bảo container SQL vẫn đang hoạt động và mật khẩu `SA_PASSWORD` trùng khớp.

**Q: Gặp lỗi 401 Unauthorized do token JWT hết hạn?**
**A:** Bạn chỉ cần đăng nhập lại trên giao diện frontend. HTTP Interceptor của Angular đã được cấu hình để tự động điều hướng về trang đăng nhập khi nhận lỗi 401.

## Những lỗi đã biết
- Component hiển thị ảnh sản phẩm có thể bị sai tỷ lệ trên một số thiết bị di động có màn hình quá nhỏ.
- Luồng tính toán mã giảm giá (Coupon) đang trong giai đoạn hoàn thiện và có thể chưa áp dụng được nếu gộp nhiều mã cùng lúc.

## Đóng góp
Mọi đóng góp nhằm hoàn thiện dự án đều được trân trọng. Vui lòng làm theo các bước sau:
1. Fork dự án này.
2. Tạo một nhánh tính năng mới (`git checkout -b feature/AmazingFeature`).
3. Commit các thay đổi (`git commit -m 'Add some AmazingFeature'`).
4. Push nhánh của bạn (`git push origin feature/AmazingFeature`).
5. Mở một Pull Request.
*Quy tắc được chấp nhận: Đảm bảo code của bạn tuân thủ Clean Architecture của hệ thống và pass mọi test case hiện có.*

## Lịch sử thay đổi
- **v1.0.0:** Khởi tạo cấu trúc Clean Architecture cơ sở, CRUD sản phẩm, setup hệ thống JWT và CSDL.
- **v1.1.0:** Tích hợp tính năng Giỏ hàng và luồng Checkout cơ bản.
- **v1.2.0:** Ra mắt Admin Dashboard và quản lý trạng thái đơn hàng.

## Tài liệu tham khảo
- [Clean Architecture in ASP.NET Core](https://learn.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/common-web-application-architectures)
- [Tài liệu chính thức của Angular](https://angular.io/docs)

## Hỗ trợ
Nếu bạn gặp khó khăn cần giải quyết hoặc muốn đóng góp ý tưởng, liên hệ với đội ngũ:
- **Email:** support@eshop-dev.com
- **Issue Tracker:** Tạo Issue báo lỗi / đề xuất tính năng trực tiếp trên GitHub Repository.

## Công nhận
Xin gửi lời cảm ơn đến các dự án nguồn mở và cộng đồng đã cung cấp công cụ hỗ trợ cho hệ thống này:
- Đội ngũ phát triển Microsoft ASP.NET Core.
- Đội ngũ phát triển Angular.

## Giấy phép
Dự án được phân phối dưới giấy phép MIT License. Tham khảo file `LICENSE` đi kèm mã nguồn để biết thêm chi tiết.