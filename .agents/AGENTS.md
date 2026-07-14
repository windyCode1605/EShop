# EShop Design System Premium v2

> **Mục tiêu:** Xây dựng một hệ thống UI/UX cao cấp cho EShop theo phong cách **anti-generic**, tập trung vào trải nghiệm người dùng, khả năng chuyển đổi (Conversion), hiệu năng và khả năng mở rộng trong môi trường production.

---

# 1. Design Philosophy

## Nguyên tắc cốt lõi

Thiết kế phải mang lại cảm giác:

* Minimal
* Premium
* Editorial
* Human-centered
* Fast
* Accessible
* Conversion First
* Anti Generic AI

### Phong cách tham khảo

* Apple Store
* Linear
* Notion
* COS
* Nike
* Aesop

### Không được

* Giao diện giống template Bootstrap
* Dashboard AI rập khuôn
* Quá nhiều màu sắc
* Quá nhiều animation
* Gradient Neon
* Glassmorphism lòe loẹt

---

# 2. Visual Atmosphere

## Vibe

Daily App Balanced kết hợp Studio Quality.

## Density

* Gọn gàng
* Thoáng
* Whitespace lớn

## Layout

Ưu tiên:

* Asymmetric Layout
* Editorial Layout
* Visual Rhythm

Hạn chế:

* Card bằng nhau
* Layout chia đều nhàm chán

## Motion

* Fluid
* Spring Physics
* Smooth
* Có trọng lượng

---

# 3. Brand Identity

## Tone

* Modern
* Trustworthy
* Quiet Luxury
* Clean
* Premium

Không sử dụng các từ mang tính AI Marketing như:

* Elevate
* Seamless
* Next Gen
* Unleash

---

# 4. Color System

## Neutral Palette

| Thành phần        | Màu       |
| ----------------- | --------- |
| Canvas            | `#F9FAFB` |
| Surface           | `#FFFFFF` |
| Secondary Surface | `#F4F4F5` |
| Primary Text      | `#18181B` |
| Secondary Text    | `#71717A` |
| Border            | `#E4E4E7` |

---

## Accent Color

Chỉ sử dụng **01 Accent duy nhất**.

Ví dụ:

```css
Blue 600
#2563EB
```

hoặc

```css
Slate 900
#0F172A
```

### Không được

* Neon
* Glow
* Sci-fi
* Dùng nhiều Accent cùng lúc

---

# 5. Typography

## Font

Chỉ sử dụng:

* Geist
* Outfit
* Satoshi
* Cabinet Grotesk

Không sử dụng:

* Inter
* Times New Roman
* Georgia

---

## Font Scale

| Level      | Size |
| ---------- | ---- |
| Display    | 48   |
| H1         | 32   |
| H2         | 24   |
| Card Title | 18   |
| Body       | 16   |
| Caption    | 14   |
| Meta       | 12   |

---

## Font Weight

* 400
* 500
* 600
* 700

Không dùng:

* 900
* 1000

---

# 6. Spacing System

Chỉ sử dụng:

```
4
8
12
16
24
32
48
64
96
```

Không dùng các khoảng cách lẻ:

```
13
17
21
29
```

---

# 7. Border Radius

| Component | Radius |
| --------- | ------ |
| Button    | 14px   |
| Input     | 16px   |
| Card      | 24px   |
| Modal     | 32px   |

---

# 8. Elevation

Chỉ có 3 cấp Shadow

* None
* Soft
* Medium

Không dùng:

* Shadow đậm
* Glow
* Floating Card

---

# 9. Grid System

Desktop

* 12 Columns

Tablet

* 8 Columns

Mobile

* 4 Columns

Container

```
1280px
```

Gap

```
24px
```

---

# 10. Layout Principles

Luôn ưu tiên:

* CSS Grid
* Mobile First
* Responsive
* Không chồng chéo
* Không Horizontal Scroll

Breakpoint

| Device  | Width   |
| ------- | ------- |
| Desktop | ≥1280px |
| Laptop  | 1024px  |
| Tablet  | 768px   |
| Mobile  | 390px   |

---

# 11. Header

Header bao gồm:

* Logo
* Navigation
* Search
* Wishlist
* Cart
* User Menu

Yêu cầu:

* Sticky
* Blur nhẹ khi scroll
* Search luôn hiển thị

---

# 12. Hero Section

Bao gồm:

* Title
* Description
* Primary CTA
* Secondary CTA
* Hero Image

Không dùng:

* Welcome
* Happy Shopping
* Generic Banner

---

# 13. Search Experience

Bắt buộc hỗ trợ:

* Autocomplete
* Recent Search
* Trending Search
* Search Suggestion
* Search History
* Loading Skeleton
* Empty State
* No Result

---

# 14. Category Section

Không dùng:

* 3 Card bằng nhau
* Grid nhàm chán

Khuyến khích:

* Zigzag Layout
* Carousel
* Editorial Layout
* Featured Category

---

# 15. Product Card

Mỗi Product Card nên gồm:

* Product Image
* Hover Image
* Product Name
* Brand
* Rating
* Review Count
* Price
* Discount
* Wishlist
* Quick Add
* Quick View
* Stock Status
* Badge

---

# 16. Product Detail

Bao gồm:

* Gallery
* Thumbnail
* Zoom
* Variant
* Color
* Size
* Quantity
* Shipping
* Return Policy
* Description
* Specification
* Reviews
* Related Products
* Recently Viewed

---

# 17. Shopping Cart

Hiển thị:

* Products
* Quantity
* Coupon
* Shipping
* Discount
* Tax
* Estimated Total

Không để người dùng tự tính.

---

# 18. Checkout Flow

Flow chuẩn:

```
Cart
    ↓
Address
    ↓
Shipping
    ↓
Payment
    ↓
Review
    ↓
Success
```

Tính năng:

* Guest Checkout
* Auto Fill
* Validation
* Progress Stepper

---

# 19. Wishlist

Cho phép:

* Save
* Remove
* Move to Cart
* Share

---

# 20. User Account

Bao gồm:

* Dashboard
* Orders
* Wishlist
* Addresses
* Payment Methods
* Notifications
* Security
* Personal Information

---

# 21. Admin Dashboard

Bao gồm:

* Dashboard
* Products
* Categories
* Orders
* Customers
* Inventory
* Coupons
* Reviews
* Analytics
* Settings

---

# 22. Motion System

Animation Duration

```
150ms
250ms
350ms
```

Spring

```
stiffness:100
damping:20
```

Hover

```css
transform: scale(1.02);
```

Active

```css
transform: translateY(1px);
```

Chỉ animate:

* opacity
* transform

Không animate:

* width
* height
* left
* top

---

# 23. Loading Strategy

Bắt buộc Skeleton Loading cho:

* Home
* Product Grid
* Product Detail
* Search
* Cart
* Checkout
* Orders
* Profile

Không sử dụng Spinner Loading mặc định.

---

# 24. Empty States

Thiết kế riêng cho:

* Empty Cart
* Empty Wishlist
* Empty Orders
* Empty Search
* 404
* No Internet

Không chỉ hiển thị một dòng text.

---

# 25. Accessibility

Tuân thủ WCAG AA.

Bao gồm:

* Contrast đạt chuẩn
* Keyboard Navigation
* Focus Ring
* Screen Reader
* ARIA
* Touch Target ≥44px

---

# 26. Performance

Bắt buộc:

* Lazy Loading
* Image Optimization
* Code Splitting
* Caching
* Prefetch
* Virtual List

Theo dõi:

* LCP
* CLS
* INP

---

# 27. Anti Generic AI Rules

## Tuyệt đối không

* Emoji trong UI
* Fake Dashboard
* Fake Review
* Fake Rating
* Fake Statistics
* Fake Badge
* Fake Uptime
* Neon
* Glow
* Sci-fi
* Card bằng nhau
* Gradient lòe loẹt
* Shadow quá mạnh
* Broken Image
* Text sáo rỗng kiểu AI

---

# 28. Frontend Technology Stack (Angular)

## Framework

* Angular 20
* Standalone Components

## Styling

* Tailwind CSS v4

## Icons

* Lucide Icons

## Animation

* Angular Animations
* Motion One

## State Management

* Signals
* NgRx Signal Store

## Forms

* Reactive Forms
* Zod
* Valibot

## Charts

* Apache ECharts

## Images

* NgOptimizedImage

## Fonts

* Geist
* Satoshi

---

# 29. EShop Information Architecture

```text
Landing
├── Home
├── Categories
│   ├── Category
│   └── Sub Category
├── Search
├── Product Listing
├── Product Detail
├── Cart
├── Checkout
│   ├── Address
│   ├── Shipping
│   ├── Payment
│   └── Success
├── Wishlist
├── Orders
│   ├── Order List
│   └── Order Detail
├── Profile
│   ├── Personal Information
│   ├── Addresses
│   ├── Security
│   └── Payment Methods
├── Authentication
│   ├── Login
│   ├── Register
│   ├── Forgot Password
│   └── Verify OTP
└── Admin
    ├── Dashboard
    ├── Products
    ├── Categories
    ├── Orders
    ├── Customers
    ├── Coupons
    ├── Inventory
    ├── Analytics
    ├── Reviews
    └── Settings
```

---

# 30. Design Goal

Mọi màn hình trong hệ thống EShop phải đáp ứng đồng thời các tiêu chí sau:

* Premium nhưng tối giản
* Khác biệt với template AI phổ biến
* Tối ưu trải nghiệm mua sắm
* Responsive trên mọi thiết bị
* Accessibility đạt chuẩn
* Hiệu năng cao
* Dễ mở rộng và bảo trì
* Nhất quán trong toàn bộ hệ thống
* Tối ưu Conversion và User Experience

---

## Agent skills

### Issue tracker

Issues and PRDs for this repo live as GitHub issues. See `docs/agents/issue-tracker.md`.

### Triage labels

The default 5 canonical roles are used. See `docs/agents/triage-labels.md`.

### Domain docs

Single-context repo layout. See `docs/agents/domain.md`.
