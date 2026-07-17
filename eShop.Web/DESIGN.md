# Design System: EShop Premium v2

## 1. Visual Theme & Atmosphere
Giao diện kết hợp giữa sự tối giản của **Editorial Layout** và sự mượt mà của **Daily App Balanced**. Vibe chủ đạo là *Quiet Luxury* — gọn gàng, nhiều khoảng trắng (whitespace lớn), lưới Asymmetric (bất đối xứng) mang lại cảm giác cao cấp như Apple Store hoặc COS. Motion mượt mà dựa trên Spring Physics (tạo cảm giác có trọng lượng thực tế). Không sử dụng các yếu tố "AI Slop" như Gradient lòe loẹt hay Glassmorphism quá đà.

## 2. Color Palette & Roles
- **Canvas White** (`#F9FAFB`) — Màu nền mặc định cho toàn bộ trang (Background).
- **Pure Surface** (`#FFFFFF`) — Màu nền của các Card, Modal, hoặc Container trôi nổi.
- **Charcoal Ink** (`#18181B`) — Màu text chính (Primary Text), tiêu đề. Không dùng `#000000`.
- **Muted Steel** (`#71717A`) — Màu text phụ (Secondary Text), mô tả, placeholder, timestamp.
- **Whisper Border** (`#E4E4E7`) — Đường viền (Border) mỏng 1px dùng cho thẻ, input, divider.
- **Accent Blue** (`#18181B`) — Thay vì dùng màu xanh/đỏ truyền thống, EShop v2 dùng Đen (Zinc 950) làm màu nhấn chủ đạo cho các nút Primary CTA, tạo cảm giác Premium Monochrome. (Màu cảnh báo lỗi dùng `#EF4444`, thành công dùng `#10B981` nhưng ở tone dịu).

## 3. Typography Rules
- **Font-Family:** `Geist` hoặc `Satoshi` (Tuyệt đối KHÔNG dùng `Inter`, `Times New Roman`, `Georgia`).
- **Display/Headlines:** Tracking hẹp (letter-spacing âm nhẹ), phân cấp thị giác bằng kích thước lớn và font-weight (500-600), không dùng bôi đen quá mức (900). 
- **Body:** Dãn dòng (leading) thư giãn, tối đa 65 ký tự trên một dòng để dễ đọc.
- **Mono:** Dùng font Monospace cho các thông số kỹ thuật, mã đơn hàng, giá tiền nếu cần mật độ cao.

## 4. Component Stylings
- **Buttons:** Bo góc `14px`. Nút Primary có nền đen chữ trắng, Hover scale lên `1.02` (Spring). Active `translateY(1px)`. Nút Secondary dùng nền trắng viền xám hoặc Ghost. Không có glow shadow.
- **Cards:** Bo góc `24px`. Đổ bóng cực nhẹ (Soft Shadow) phân tương. Khi có mật độ thông tin cao, bỏ qua bóng và viền card, chuyển sang dùng đường kẻ ngang (border-top dividers) và negative space.
- **Inputs:** Bo góc `16px`. Label nằm trên input. Khi Focus, viền đổi sang đen `Charcoal Ink` và shadow rất mỏng. Không dùng floating labels.
- **Loaders:** Dùng Skeleton loading (Shimmer animation) khớp đúng kích thước nội dung sẽ hiển thị. Tuyệt đối không dùng Spinner xoay tròn mặc định.
- **Empty States:** Thiết kế dưới dạng một layout có cấu trúc, icon mờ + text phụ + Nút CTA ("Xóa bộ lọc" / "Mua sắm ngay"). Không chỉ hiện 1 dòng text trống rỗng.

## 5. Layout Principles
- **Grid System:** Dùng CSS Grid. Không dùng `calc()` rườm rà. Max-width `1280px` cho Desktop.
- **Banned:** Tuyệt đối không làm layout 3 cột chia đều (Card bằng nhau) kiểu template AI cũ. Sử dụng Asymmetric Layout (Vd: Cột trái lớn, cột phải nhỏ hoặc Zig-zag).
- **No Overlapping:** Không chồng chéo các khối văn bản lên hình ảnh nếu độ tương phản thấp. Mỗi element có không gian riêng.
- **Responsive:** Mobile First, cột đơn khi màn hình < `768px`. Touch target tối thiểu `44px`.

## 6. Motion & Interaction
- **Physics:** Mọi chuyển động dùng tham số Spring (`stiffness: 100`, `damping: 20`).
- **Tránh đổi layout đột ngột:** Cascade reveal cho danh sách. Animate trên `opacity` và `transform` thay vì thay đổi chiều cao/rộng trực tiếp gây giật layout (Reflow).

## 7. Anti-Patterns (NEVER DO)
- **No Emojis** trên UI chính (Trừ các icon SVG tinh xảo: Lucide Icons).
- **No Neon / Sci-Fi / Purple Glow** shadow.
- **No Filler Text:** Tránh dùng các từ ngữ AI Marketing sáo rỗng như *"Elevate"*, *"Seamless"*, *"Next-Gen"*.
- **No Default Shadow:** Không dùng đổ bóng quá đen (`rgba(0,0,0,0.5)`).
- **No Fake Data:** Tránh dùng tên giả kiểu *"John Doe"*, *"Acme"*. Dùng tên mang tính thực tế hoặc bỏ trống nếu không cần thiết.
