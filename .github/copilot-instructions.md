# Hướng dẫn Dự án

## Kiến trúc
- Không gian làm việc này là một monorepo full-stack với:

- Backend: `eShop.API` (.NET 9 Web API + EF Core + OpenIddict).

- Frontend: `eShop.Web` (Angular 17).

- Các lớp backend cốt lõi được tổ chức trong `eShop.API/Library` và `eShop.API/Services`.

- Ưu tiên các mẫu đăng ký dựa trên extension hiện có (ví dụ: `AddApplicationServices()`) hơn là thiết lập DI ad-hoc trong mã tính năng.

## Xây dựng và Chạy
- Xây dựng backend (từ `eShop.API`): `dotnet build`
- Chạy backend (từ `eShop.API`): `dotnet run`
- Cài đặt frontend (từ `eShop.Web`): `npm install`
- Máy chủ phát triển frontend (từ `eShop.Web`): `npm start`
- Xây dựng frontend phiên bản sản xuất (từ `eShop.Web`): `npm run build`
- Theo dõi quá trình xây dựng frontend (từ `eShop.Web`): `npm run watch`

## Yêu cầu kiểm thử
- Hiện tại chưa có dự án kiểm thử hoặc kịch bản kiểm thử chuyên dụng nào được cấu hình.

- Đối với các thay đổi ở backend, tối thiểu hãy chạy `dotnet build` trong `eShop.API` và đảm bảo ứng dụng vẫn khởi động thành công.

- Đối với các thay đổi ở frontend, tối thiểu hãy chạy `npm run build` trong `eShop.Web`.

## Quy ước
- Giữ cho hành vi API backend nhất quán với các middleware và mẫu phản hồi hiện có.
- Giữ nguyên thứ tự middleware trong `Program.cs` trừ khi tác vụ yêu cầu thay đổi pipeline một cách rõ ràng.

- Ưu tiên chỉnh sửa nhỏ, có mục tiêu; không nên tái cấu trúc các khu vực không liên quan trong khi triển khai tính năng/sửa lỗi.

- Tái sử dụng các mẫu module quản lý sản phẩm hiện có cho các thành phần thông minh/đơn giản của Angular.

## Các lỗi thường gặp
- CORS hiện bị giới hạn ở `http://localhost:4200` trong quá trình khởi động backend.

- Chuỗi kết nối backend mặc định sử dụng LocalDB (`(localdb)\\mssqllocaldb`) trong `appsettings.json`.

- Quá trình khởi động backend tự động áp dụng các migration của EF và gieo dữ liệu client OpenIddict; hãy cẩn thận khi thay đổi mã khởi tạo.

## Tài liệu tham khảo quan trọng
- Tổng quan về lớp Backend/API và AI: `README.md`
- Mô tả dự án bổ sung và ghi chú về AI: `README-CLAUDE.md`
- Kiến thức cơ bản về ứng dụng Angular: `eShop.Web/README.md`
- Hướng dẫn mô-đun quản lý sản phẩm: `eShop.Web/src/app/modules/product-manager/README.md`
- Thiết lập/danh sách kiểm tra quản lý sản phẩm: `eShop.Web/PRODUCT_MANAGER_SETUP.md`, `eShop.Web/PRODUCT_MANAGER_CHECKLIST.md`