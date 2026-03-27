# 🛒 eShop — AI-Powered Commerce System

> **Định vị rõ ràng:** Đây không phải web bán hàng thông thường.  
> AI là trung tâm vận hành — không phải tính năng bổ sung.

---

## 📌 Mục lục

1. [Core Value & Định vị hệ thống](#-core-value)
2. [Kiến trúc tổng thể](#-kiến-trúc-tổng-thể)
3. [AI Features](#-ai-features)
   - Intelligent Sales Agent
   - Automated Order Handling Agent
   - Recommendation System
   - AI Product Description Generation
4. [Solution Structure](#-solution-structure)
5. [ERD & Domain Design](#-erd--domain-design)
6. [Hướng dẫn cài đặt](#-hướng-dẫn-cài-đặt)
7. [API Reference](#-api-reference)
8. [Lộ trình phát triển](#-lộ-trình-phát-triển)

---

## 🎯 Core Value

| | Shopee / Tiki / Lazada | **eShop AI** |
|---|---|---|
| Tìm kiếm | Keyword matching | 🆕 **NLP intent parsing** |
| Mô tả sản phẩm | Nhập tay | 🆕 **AI auto-generate** |
| Xử lý đơn hàng | Nhân viên CSKH | 🆕 **AI Agent tự động** |
| Gợi ý sản phẩm | Rule-based | 🆕 **Vector similarity search** |
| Kiến trúc | Monolith CRUD | 🆕 **AI Service Layer tách biệt** |

---

## 🏗 Kiến trúc tổng thể

```
┌─────────────────────────────────────────────┐
│              Frontend (Angular)              │
└────────────────────┬────────────────────────┘
                     │ HTTP / WebSocket
┌────────────────────▼────────────────────────┐
│            Backend API (.NET 9)              │
│   ┌─────────────┐     ┌──────────────────┐  │
│   │  Core API   │     │    AI API        │  │
│   │  /products  │     │  /ai/search      │  │
│   │  /orders    │     │  /ai/generate    │  │
│   └──────┬──────┘     └────────┬─────────┘  │
└──────────┼──────────────────────┼────────────┘
           │                      │
┌──────────▼──────────────────────▼────────────┐
│                  Database                     │
│    CoreDbContext        AIDbContext           │
│    (Products/Orders)    (Logs/Vectors)        │
└───────────────────────────────────────────────┘
```

> 💡 **Thay đổi so với tài liệu gốc:** Tách rõ `Core API` và `AI API` thành 2 service độc lập ngay từ sơ đồ kiến trúc, giúp scale và deploy riêng biệt.

---

## 🤖 AI Features

### 1. Intelligent Sales Agent

**Mục tiêu:** Thay thế tìm kiếm keyword bằng hiểu ngôn ngữ tự nhiên.

#### Luồng xử lý

```
User: "Tôi cần áo đi tiệc cưới mùa hè dưới 2 triệu"
         │
         ▼
 ┌───────────────────┐
 │   NLP Parsing     │  → category: "áo", event: "wedding",
 │  (LLM / Rules)    │    season: "summer", budget: < 2_000_000
 └────────┬──────────┘
          │
          ▼
 ┌───────────────────┐
 │  Query Builder    │  → WHERE category='shirt'
 │                   │    AND price < 2000000
 └────────┬──────────┘
          │
          ▼
 ┌───────────────────┐
 │   Product API     │  → Ranked result list
 └───────────────────┘
```

#### 🆕 Nâng cấp đề xuất

- **Thêm confidence score** cho mỗi kết quả NLP parsing — nếu score thấp, hỏi lại người dùng thay vì trả về sai
- **Fallback strategy:** NLP parse thất bại → tự động chuyển về keyword search thay vì báo lỗi
- **SearchLogs analytics:** Lưu toàn bộ query để cải thiện model theo thời gian

```csharp
// 🆕 Interface nâng cấp — thêm confidence + fallback
public interface IAISearchService
{
    Task<SearchParseResult> ParseQueryAsync(string naturalLanguageQuery);
    // SearchParseResult chứa: Filters, ConfidenceScore, FallbackToKeyword
}
```

---

### 2. Automated Order Handling Agent

**Mục tiêu:** Tự động hóa các tác vụ CSKH lặp lại.

#### Use Case: Hủy đơn hàng

```
User: "Hủy đơn hàng #123"
         │
         ▼
  Kiểm tra Order Status
         │
    ┌────┴────┐
 Pending   Shipped / Delivered
    │            │
 Cancel()    Return message:
 Email()     "Không thể hủy — đơn đã giao"
```

#### Logic xử lý

```csharp
public async Task<AgentResult> HandleCancelAsync(int orderId, int userId)
{
    var order = await _orderRepo.GetByIdAsync(orderId);

    // 🆕 Kiểm tra ownership — tránh user A hủy đơn user B
    if (order.UserId != userId)
        return AgentResult.Unauthorized();

    if (order.Status != OrderStatus.Pending)
        return AgentResult.Rejected("Đơn hàng không thể hủy ở trạng thái hiện tại.");

    await _orderRepo.CancelAsync(orderId);
    await _emailService.SendCancellationEmailAsync(order);

    return AgentResult.Success();
}
```

#### 🆕 Nâng cấp đề xuất

- **Thêm ownership check** — bảo vệ endpoint khỏi truy cập trái phép (thiếu trong tài liệu gốc)
- **AgentResult pattern** — response thống nhất thay vì throw exception tự do
- **Mở rộng use case:** Đổi địa chỉ giao hàng, Yêu cầu hoàn tiền, Tra cứu trạng thái đơn

---

### 3. Recommendation System

**Mục tiêu:** Cá nhân hóa trải nghiệm mua sắm dựa trên hành vi.

#### Dữ liệu đầu vào

| Nguồn | Tín hiệu |
|---|---|
| Lịch sử xem | View count, thời gian xem |
| Lịch sử mua | Purchased items, frequency |
| Hành vi realtime | Click, Add to cart, Wishlist |

#### Phương pháp kết hợp (Hybrid)

```
User Behavior Logs
       │
       ▼
 Feature Extraction
  ┌────┴────────────────────────────┐
  │                                 │
  ▼                                 ▼
Content-Based                Collaborative
Filtering                    Filtering
(thuộc tính sản phẩm)        (user similarity)
  │                                 │
  └──────────────┬──────────────────┘
                 │
                 ▼
          Vector Search
       (cosine similarity)
                 │
                 ▼
        Top-K Recommendation
```

#### 🆕 Nâng cấp đề xuất

- **Cold start handling:** User mới chưa có lịch sử → dùng global trending thay vì trả về rỗng
- **Phân loại output rõ ràng:**

```
"Sản phẩm tương tự"     → Content-based (cùng danh mục)
"Có thể bạn thích"      → Collaborative (user như bạn đã mua)
"Mua cùng nhau"         → Association rules (frequently bought together)
"Đang hot hôm nay"      → 🆕 Trending (cold-start fallback)
```

---

### 4. AI Product Description Generation

**Mục tiêu:** Admin nhập thông tin cơ bản → AI sinh mô tả chuẩn SEO tự động.

#### Luồng xử lý

```
Admin nhập: tên, chất liệu, màu, phong cách
         │
         ▼
  Prompt Engineering
  ┌─────────────────────────────────────┐
  │ Viết mô tả sản phẩm thời trang:    │
  │  - Tên: {name}                      │
  │  - Chất liệu: {material}            │
  │  - Màu sắc: {color}                 │
  │  - Phong cách: {style}              │
  │ Yêu cầu: ngắn gọn, hấp dẫn, SEO   │
  └──────────────┬──────────────────────┘
                 │
                 ▼
         AI Model (Claude / GPT)
                 │
                 ▼
    Save → Products.AI_Description
    Log  → AI_GenerationLogs
```

#### Ví dụ

**Input:**
```
Tên: Áo sơ mi nam  |  Chất liệu: Cotton  |  Màu: Trắng  |  Phong cách: Công sở
```

**Output:**
```
Áo sơ mi nam chất liệu cotton cao cấp, thoáng mát, mang lại cảm giác dễ chịu 
suốt ngày dài làm việc. Thiết kế tinh tế, tối giản — phù hợp cho môi trường 
công sở và các dịp trang trọng.
```

#### Schema Database

**Bảng `Products` — bổ sung fields AI:**

| Field | Type | Mô tả |
|---|---|---|
| Description | text | Mô tả gốc (nhập tay) |
| AI_Description | text | Mô tả do AI sinh |
| AI_Generated | bool | Flag kiểm tra |
| 🆕 AI_GeneratedAt | datetime | Thời điểm sinh |
| 🆕 AI_Provider | varchar | Claude / GPT / Local LLM |

**Bảng `AI_GenerationLogs`:**

| Field | Type | Mô tả |
|---|---|---|
| Id | int | PK |
| ProductId | int | FK → Products |
| Prompt | text | Prompt đã gửi |
| Response | text | Phản hồi nhận về |
| CreatedAt | datetime | Timestamp |
| 🆕 TokensUsed | int | Theo dõi chi phí API |
| 🆕 LatencyMs | int | Hiệu năng |
| 🆕 ModelVersion | varchar | Phiên bản model |

#### 🆕 Nâng cấp đề xuất

- **Retry logic** khi AI API timeout hoặc trả về lỗi
- **Rate limiting** — tránh vượt quota API
- **Token tracking** — kiểm soát chi phí theo từng sản phẩm

---

## 📁 Solution Structure

```
eShop/
│
├── 📚 Library/                          # Shared across solution
│   ├── CR.EntitiesBase/
│   │   ├── BaseEntity.cs                # Id, CreatedAt, UpdatedAt
│   │   └── BaseAuditEntity.cs           # + CreatedBy, UpdatedBy
│   │
│   ├── CR.ApplicationBase/
│   │   ├── ServiceBase.cs               # Logger + DbContext inject sẵn
│   │   └── Localization/
│   │       └── IMapErrorCode.cs         # Error code → message (vi/en)
│   │
│   ├── CR.InfrastructureBase/
│   │   └── Persistence/
│   │       └── ApplicationDbContext.cs  # Base DbContext<TUser>
│   │
│   ├── CR.WebAPIBase/
│   │   ├── Responses/
│   │   │   └── ApiResponse.cs           # { Success, Data, Message, Errors }
│   │   ├── Filters/
│   │   │   └── ValidationFilter.cs
│   │   └── Middlewares/
│   │       └── GlobalExceptionMiddleware.cs
│   │
│   └── CR.Utils/
│       ├── StringExtensions.cs
│       └── DateTimeUtils.cs
│
├── 🎯 Services/
│   │
│   ├── 💼 Core/
│   │   ├── CR.Core.Domain/
│   │   │   ├── Product/
│   │   │   │   ├── Product.cs
│   │   │   │   └── ProductCategory.cs
│   │   │   ├── Order/
│   │   │   │   ├── Order.cs
│   │   │   │   └── OrderItem.cs
│   │   │   ├── Customer/
│   │   │   │   └── Customer.cs
│   │   │   ├── Inventory/
│   │   │   │   └── StockLedger.cs
│   │   │   └── Payment/
│   │   │       └── PaymentRecord.cs
│   │   │
│   │   ├── CR.Core.Dtos/
│   │   │   ├── Product/
│   │   │   │   ├── ProductRequestDto.cs
│   │   │   │   └── ProductResponseDto.cs
│   │   │   ├── Order/
│   │   │   │   ├── CreateOrderDto.cs
│   │   │   │   └── OrderResponseDto.cs
│   │   │   └── Search/
│   │   │       └── SearchRequestDto.cs  # query, filters, 🆕 confidence threshold
│   │   │
│   │   ├── CR.Core.ApplicationServices/
│   │   │   ├── ProductModule/
│   │   │   │   └── IProductService.cs
│   │   │   ├── OrderModule/
│   │   │   │   ├── IOrderService.cs
│   │   │   │   └── IOrderAgentService.cs
│   │   │   ├── CustomerModule/
│   │   │   │   └── ICustomerService.cs
│   │   │   ├── AuthenticationModule/
│   │   │   │   └── IAuthService.cs
│   │   │   └── Common/ServiceImplementations/
│   │   │       ├── ProductService.cs
│   │   │       ├── OrderService.cs
│   │   │       └── OrderAgentService.cs
│   │   │
│   │   ├── CR.Core.Infrastructure/
│   │   │   ├── Persistence/
│   │   │   │   ├── CoreDbContext.cs
│   │   │   │   ├── Configurations/
│   │   │   │   │   ├── ProductConfiguration.cs
│   │   │   │   │   └── OrderConfiguration.cs
│   │   │   │   ├── Repositories/
│   │   │   │   │   ├── ProductRepository.cs
│   │   │   │   │   └── OrderRepository.cs
│   │   │   │   └── Migrations/
│   │   │   └── Exceptions/
│   │   │       └── CoreExceptionHandler.cs
│   │   │
│   │   └── CR.Core.API/
│   │       ├── Program.cs
│   │       ├── Controllers/
│   │       │   ├── ProductsController.cs
│   │       │   ├── OrdersController.cs
│   │       │   └── AuthController.cs
│   │       └── appsettings.json
│   │
│   ├── 🤖 AI/
│   │   ├── CR.AI.Domain/
│   │   │   ├── SearchFilter.cs           # Kết quả NLP parsing
│   │   │   ├── GenerationLog.cs
│   │   │   └── RecommendationResult.cs
│   │   │
│   │   ├── CR.AI.Dtos/
│   │   │   ├── GenerateDescriptionRequestDto.cs
│   │   │   └── SearchParseResponseDto.cs # 🆕 thêm ConfidenceScore
│   │   │
│   │   ├── CR.AI.ApplicationServices/
│   │   │   ├── IAISearchService.cs       # ParseQueryAsync()
│   │   │   ├── IAIGenerationService.cs   # GenerateDescriptionAsync()
│   │   │   ├── IRecommendationService.cs # GetTopKAsync()
│   │   │   └── Common/
│   │   │       ├── AISearchService.cs
│   │   │       ├── AIGenerationService.cs
│   │   │       └── RecommendationService.cs
│   │   │
│   │   ├── CR.AI.Infrastructure/
│   │   │   ├── Persistence/
│   │   │   │   ├── AIDbContext.cs
│   │   │   │   └── Repositories/
│   │   │   │       └── GenerationLogRepository.cs
│   │   │   ├── External/
│   │   │   │   ├── AnthropicClient.cs        # Gọi Claude API
│   │   │   │   └── RateLimitedAIClient.cs    # Decorator: giới hạn quota
│   │   │   └── Migrations/
│   │   │
│   │   └── CR.AI.API/
│   │       ├── Program.cs
│   │       ├── Controllers/
│   │       │   ├── AISearchController.cs
│   │       │   └── AIGenerationController.cs
│   │       └── appsettings.json
│   │
│   └── 📦 Shared/
│       ├── CR.Constants/
│       │   ├── OrderStatus.cs            # enum: Pending, Shipped, Delivered...
│       │   └── AIProviders.cs
│       ├── CR.Common/
│       │   └── PaginatedResult.cs
│       └── CR.UserRolePermission/
│           └── PermissionConstants.cs
│
├── 🛠️ Tools/
│   ├── CR.Migrations/
│   ├── CR.HostConsole/                   # CLI: seed data, scheduled jobs
│   └── CR.Project.AppHost/              # Aspire orchestration
│
├── 🌐 WebApps/
│   └── CR.WebApp/                       # Angular frontend
│
└── eShop.sln
```

---

## 🗄 ERD & Domain Design

### Entities cốt lõi (Core)

```
Users ──────────────< Orders >──────────── OrderItems >──── Products
  │                     │                                        │
  │              OrderStatus enum                          Categories
  │           (Pending/Shipped/Delivered/Cancelled)
  │
  └──────────────< SearchLogs
                 (query, parsed_filter, result_count)
```

### Entities AI

```
Products ──────────< AI_GenerationLogs
                    (prompt, response, tokens_used, latency_ms)

Users ─────────────< UserBehaviorLogs
                    (product_id, action: view/click/cart, timestamp)

Products ──────────  ProductVectors
                    (embedding vector cho similarity search)
```

> 🆕 **Bổ sung so với tài liệu gốc:** `UserBehaviorLogs` và `ProductVectors` là nền tảng bắt buộc cho Recommendation System — thiếu hai bảng này thì feature recommendation không thể triển khai.

---

## ⚙️ Hướng dẫn cài đặt

### Yêu cầu

- .NET 9 SDK
- Node.js 18+ & Angular CLI v17+
- SQL Server (LocalDB, Express, hoặc Docker)
- API key: Claude (Anthropic) hoặc OpenAI

### Backend

```bash
cd eShop/Services/Core/CR.Core.API

dotnet restore
dotnet ef database update
dotnet run
```

API: `http://localhost:5000` | Swagger: `http://localhost:5000/swagger`

### AI Service

```bash
cd eShop/Services/AI/CR.AI.API

# Cấu hình API key trong appsettings.Development.json
dotnet run
```

AI Service: `http://localhost:5001`

### Frontend

```bash
cd eShop/WebApps/CR.WebApp

npm install
ng serve
```

Frontend: `http://localhost:4200`

### Cấu hình

**`appsettings.Development.json` (AI Service):**
```json
{
  "AIProviders": {
    "Anthropic": {
      "ApiKey": "YOUR_API_KEY",
      "Model": "claude-sonnet-4-20250514",
      "MaxTokens": 1000
    }
  },
  "RateLimiting": {
    "RequestsPerMinute": 20
  }
}
```

**Connection string mặc định (LocalDB):**
```
Server=(localdb)\mssqllocaldb;Database=eShop_Db;Trusted_Connection=true;Encrypt=false
```

**Docker SQL Server (tuỳ chọn):**
```bash
docker run -e 'ACCEPT_EULA=Y' -e 'SA_PASSWORD=YourPassword123' \
  -p 1433:1433 mcr.microsoft.com/mssql/server:2022-latest
```

---

## 📡 API Reference

### Authentication
| Method | Endpoint | Mô tả |
|---|---|---|
| POST | `/api/auth/register` | Đăng ký tài khoản |
| POST | `/api/auth/login` | Đăng nhập, nhận JWT |

### Products
| Method | Endpoint | Auth | Mô tả |
|---|---|---|---|
| GET | `/api/products` | — | Lấy danh sách sản phẩm |
| GET | `/api/products/{id}` | — | Chi tiết sản phẩm |
| POST | `/api/products` | ✅ | Tạo sản phẩm mới |
| PUT | `/api/products/{id}` | ✅ | Cập nhật sản phẩm |
| DELETE | `/api/products/{id}` | ✅ | Xoá sản phẩm |

### AI Endpoints
| Method | Endpoint | Auth | Mô tả |
|---|---|---|---|
| POST | `/ai/search/parse` | — | Parse natural language query |
| POST | `/ai/generate/description` | ✅ | Sinh mô tả sản phẩm |
| GET | `/ai/recommend/{userId}` | ✅ | Gợi ý cá nhân hoá |

### Orders
| Method | Endpoint | Auth | Mô tả |
|---|---|---|---|
| GET | `/api/orders` | ✅ | Lịch sử đơn hàng |
| POST | `/api/orders` | ✅ | Tạo đơn hàng |
| DELETE | `/api/orders/{id}` | ✅ | Yêu cầu hủy đơn |

---

## 🚀 Lộ trình phát triển

### Phase 1 — Nền tảng ✅
- [x] Clean Architecture setup (.NET 9)
- [x] JWT Authentication
- [x] CRUD Products / Orders
- [x] Angular frontend cơ bản

### Phase 2 — AI Core (Ưu tiên cao)
- [ ] **AI Search** — NLP parsing với confidence score
- [ ] **AI Description Generator** — tích hợp Claude API + log tokens
- [ ] `SearchLogs` + `AI_GenerationLogs` schema

### Phase 3 — Personalization
- [ ] `UserBehaviorLogs` tracking
- [ ] Recommendation engine (Content-based + Collaborative)
- [ ] `ProductVectors` table + vector similarity

### Phase 4 — Automation & Scale
- [ ] Order Agent — hủy đơn, hoàn tiền tự động
- [ ] RateLimitedAIClient — kiểm soát quota
- [ ] Admin dashboard: AI usage analytics

---

## 🔥 Điểm khác biệt so với hệ thống CRUD thông thường

| Tiêu chí | CRUD thuần | **eShop AI** |
|---|---|---|
| Tìm kiếm | SQL `LIKE '%keyword%'` | NLP intent extraction |
| Nội dung | Nhập tay 100% | AI-assisted generation |
| Chăm sóc KH | Con người xử lý | AI Agent tự động hóa |
| Cá nhân hóa | Không có | Behavior-based recommendation |
| Kiến trúc | 1 service monolith | Tách biệt Core / AI service |
| Khả năng mở rộng | Giới hạn | Scale AI service độc lập |

---

*Tài liệu được cập nhật lần cuối: 2025*

### Các nhóm người dùng chính:
#### 1. Khách hàng (Customers)
#### Khách vãng lai (Guest):

- Xem sản phẩm, danh mục
- Tìm kiếm sản phẩm
- Xem giỏ hàng
- Đặt hàng (cần nhập thông tin cơ bản)

#### Khách hàng đã đăng ký:

- Tất cả chức năng của khách vãng lai
- Quản lý tài khoản cá nhân
- Xem lịch sử đơn hàng
- Theo dõi trạng thái đơn hàng
- Lưu địa chỉ giao hàng
- Wishlist/danh sách yêu thích
- Đánh giá, nhận xét sản phẩm
- Tích điểm thành viên

#### 2. Quản trị viên (Admin)
#### Super Admin:

- Quản lý toàn bộ hệ thống
- Quản lý người dùng và phân quyền
- Cấu hình hệ thống
- Quản lý danh mục sản phẩm
- Quản lý sản phẩm (CRUD)
- Quản lý đơn hàng
- Quản lý kho
- Báo cáo và thống kê
- Quản lý khuyến mãi, voucher
- Quản lý nội dung website

#### 3. Nhân viên vận hành
#### Nhân viên bán hàng:

- Xem và xử lý đơn hàng
- Tư vấn khách hàng
- Tạo đơn hàng thay khách

#### Nhân viên kho:

- Quản lý tồn kho
- Xuất/nhập kho
- Đóng gói đơn hàng
#### Kiểm kê

#### Nhân viên marketing:

- Quản lý khuyến mãi
- Quản lý banner, nội dung
- Xem báo cáo analytics

#### Nhân viên CSKH:

- Xem thông tin khách hàng
- Xử lý khiếu nại, hoàn trả
- Chat hỗ trợ khách hàng

#### 4. Đối tác (nếu là marketplace)
#### Nhà cung cấp/Người bán:

- Quản lý sản phẩm của mình
- Xem đơn hàng
- Quản lý kho riêng
- Rút tiền, xem doanh thu
- Báo cáo bán hàng

#### Đối tác vận chuyển:

- Nhận thông tin đơn hàng
- Cập nhật trạng thái giao hàng
- API tích hợp
