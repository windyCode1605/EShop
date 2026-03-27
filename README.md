# 🤖 LỚP TRẢI NGHIỆM NGƯỜI DÙNG (AI-POWERED FEATURES)

## 🎯 Mục tiêu

Lớp này nhằm nâng cấp hệ thống E-commerce truyền thống bằng cách tích hợp AI Agent Framework, giúp:

- Hiểu yêu cầu người dùng bằng ngôn ngữ tự nhiên  
- Tự động hóa quy trình mua sắm  
- Cá nhân hóa trải nghiệm người dùng  
- Tự động tạo nội dung sản phẩm  

---

# 🧠 1. Intelligent Sales Agent (AI bán hàng thông minh)

## 📌 Mô tả

Thay vì sử dụng tìm kiếm keyword truyền thống, hệ thống triển khai một AI Agent có khả năng:

- Hiểu ngôn ngữ tự nhiên (Natural Language Understanding)  
- Phân tích intent (ý định người dùng)  
- Truy vấn dữ liệu sản phẩm qua API  
- Trả về danh sách sản phẩm phù hợp  

---

## 🔄 Luồng xử lý

```plaintext
User Input 
   ↓
NLP Processing (Extract: category, price, context)
   ↓
Convert → Query Filter
   ↓
Call Product API
   ↓
Return Result
```

---

## 💡 Ví dụ

### Input:
```
“Tôi cần áo đi tiệc cưới mùa hè dưới 2 triệu”
```

### AI hiểu:
- Category: áo (shirt)  
- Event: wedding  
- Season: summer  
- Budget: < 2M  

### Output:
- Danh sách sản phẩm phù hợp + sắp xếp theo độ liên quan  

---

## 🧪 Kỹ thuật sử dụng

- NLP parsing (rule-based hoặc LLM)  
- Mapping intent → filter query  
- Ranking sản phẩm  

---

# ⚙️ 2. Automated Order Handling Agent

## 📌 Mô tả

AI Agent hỗ trợ xử lý các yêu cầu liên quan đến đơn hàng mà không cần nhân viên.

---

## 🔄 Use Case: Hủy đơn hàng

```plaintext
User request: "Hủy đơn hàng #123"
        ↓
Agent kiểm tra Order Status
        ↓
IF status = "Pending"
    → Call Cancel API
    → Update DB
    → Send Email
ELSE
    → Return error
```

---

## 💡 Logic xử lý

```pseudo
IF order.status != "Shipped":
    cancelOrder()
    sendEmail()
ELSE:
    reject()
```

---

## 🎯 Lợi ích

- Giảm tải CSKH  
- Xử lý real-time  
- Tự động hóa quy trình  

---

# 🎯 3. Recommendation System (Cá nhân hóa)

## 📌 Mô tả

Hệ thống gợi ý sản phẩm dựa trên hành vi người dùng.

---

## 📊 Dữ liệu đầu vào

- Lịch sử xem sản phẩm  
- Lịch sử mua hàng  
- Hành vi (click, add to cart)  

---

## 🧠 Phương pháp

### 🔹 Content-Based Filtering
- So sánh thuộc tính sản phẩm  

### 🔹 Collaborative Filtering
- User A giống User B → gợi ý giống nhau  

### 🔹 Vector Search
- Encode sản phẩm thành vector  
- Tìm độ tương đồng  

---

## 🔄 Flow

```plaintext
User Behavior Logs
        ↓
Feature Extraction
        ↓
Similarity Calculation
        ↓
Top-K Recommendation
```

---

## 📈 Output

- Sản phẩm tương tự  
- “Có thể bạn thích”  
- “Mua cùng”  

---

# ✍️ 4. AI Product Description Generation

## 📌 Mô tả

Hệ thống sử dụng AI để tự động tạo mô tả sản phẩm dựa trên thông tin đầu vào.

---

## 🎯 Mục tiêu

- Tiết kiệm thời gian cho admin/seller  
- Tạo nội dung hấp dẫn, chuẩn SEO  
- Tăng khả năng chuyển đổi  

---

## 🔄 Luồng xử lý

```plaintext
Admin nhập thông tin sản phẩm
        ↓
Call AI Service (Prompt)
        ↓
AI Generate Description
        ↓
Save vào DB (AI_Description)
        ↓
Hiển thị trên UI
```

---

## 💡 Ví dụ

### Input:

```
Tên: Áo sơ mi nam
Chất liệu: Cotton
Màu: Trắng
Phong cách: Công sở
```

---

### Output:

```
Áo sơ mi nam chất liệu cotton cao cấp, thoáng mát, mang lại cảm giác dễ chịu suốt ngày dài. 
Thiết kế tối giản, phù hợp cho môi trường công sở và các dịp trang trọng.
```

---

## 🧪 Kỹ thuật sử dụng

### 🔹 Prompt Engineering

```plaintext
Viết mô tả sản phẩm thời trang với thông tin sau:
- Tên: {name}
- Chất liệu: {material}
- Màu sắc: {color}
- Phong cách: {style}

Yêu cầu:
- Ngắn gọn
- Hấp dẫn
- Chuẩn SEO
```

---

### 🔹 AI Model

- OpenAI API (GPT)  
- Hoặc Local LLM  

---

## 🗄 Cập nhật Database

### Products

| Field | Type |
|------|------|
| Description | text |
| AI_Description | text |
| AI_Generated | bool |

---

### AI_GenerationLogs

| Field | Type |
|------|------|
| Id | int |
| ProductId | int |
| Prompt | text |
| Response | text |
| CreatedAt | datetime |

---

# 🏗 5. Kiến trúc AI Layer

```plaintext
Frontend (Angular)
   ↓
Backend API (.NET)
   ↓
AI Service Layer 🤖
   ├── NLP Module
   ├── Recommendation Engine
   ├── Order Agent
   ├── Text Generation (Description)
   ↓
Database
```

---

# 🔥 Điểm mạnh hệ thống

- Không chỉ CRUD mà có AI hỗ trợ ra quyết định  
- AI có khả năng tự thực hiện action thông qua API  
- Cá nhân hóa dựa trên hành vi người dùng  
- Tự động hóa nội dung sản phẩm  

---

# 💥 Tổng kết

> Hệ thống chuyển đổi từ mô hình “bán hàng truyền thống” sang “AI-powered commerce”, trong đó AI đóng vai trò trung tâm trong việc hỗ trợ người dùng và tối ưu vận hành.









```


===================================================================================================================


```






```


===================================================================================================================


```

# 🚀 BƯỚC 0 — CHỌN CORE VALUE (CỰC QUAN TRỌNG)

## ❓ Câu hỏi bắt buộc:
Hệ thống KHÁC gì so với Shopee / Tiki / Lazada?

- ❌ Không phải: Web bán hàng thông thường  
- ✅ Phải là: **AI-powered Commerce System**

> 👉 AI là trung tâm (NOT optional feature)  
> 👉 CRUD chỉ là nền tảng hỗ trợ  

---

# 🧭 BƯỚC 1 — DEFINE SCOPE

## 🎯 Chọn feature AI cốt lõi (khuyến nghị):

- Intelligent Search (AI tìm sản phẩm)
- AI Product Description

## 💡 Lý do:
- Dễ demo
- Có AI thực tế
- Không cần infra quá nặng

---

# 🧱 BƯỚC 2 — THIẾT KẾ DOMAIN (ERD + USE CASE)

## 📌 Entities tối thiểu:

- Users
- Products
- Categories
- Orders
- OrderItems

## 📌 Entities mở rộng (AI):

- AI_GenerationLogs
- SearchLogs

## 📌 Use Case quan trọng:

- User search bằng text tự nhiên
- User xem sản phẩm
- Admin tạo sản phẩm + AI generate description

> ⚠️ Nếu bước này làm không kỹ → AI không integrate được → fail

---

# 🏗 BƯỚC 3 — SETUP BACKEND

## 🧩 Tech Stack:

- .NET Web API
- EF Core
- MySQL

## 🎯 Tasks:

### 1. Setup Clean Architecture:

- Controllers
- Services
- Repositories
- DTOs

### 2. Build API cơ bản:

- `GET /products`
- `GET /products/{id}`
- `POST /products`
- `GET /orders`

> 👉 Đây là nền để AI gọi vào

---

# 🤖 BƯỚC 4 — AI SERVICE LAYER (QUAN TRỌNG NHẤT)

## ❌ Sai:
Nhét AI trực tiếp vào Controller

## ✅ Đúng:


```


===================================================================================================================


```
```
eShop/
│
├── 📚 Library/                              # Tái sử dụng toàn solution
│   ├── CR.ApplicationBase/
│   │   ├── ServiceBase.cs
│   │   ├── ServiceBaseSingleton.cs
│   │   └── Localization/
│   │       └── IMapErrorCode.cs
│   │
│   ├── CR.EntitiesBase/
│   │   └── BaseEntity.cs                   # Id, CreatedAt, UpdatedAt
│   │
│   ├── CR.DtoBase/
│   │   ├── BaseRequestDto.cs
│   │   └── BaseResponseDto.cs
│   │
│   ├── CR.InfrastructureBase/
│   │   ├── Persistence/
│   │   │   └── ApplicationDbContext.cs     # DbContext<TUser> base
│   │   └── RabbitMQ/
│   │       └── IRabbitMQPublisher.cs
│   │
│   ├── CR.WebAPIBase/
│   │   ├── Filters/
│   │   │   └── ValidationFilter.cs
│   │   ├── Middlewares/
│   │   │   └── GlobalExceptionMiddleware.cs
│   │   └── Responses/
│   │       └── ApiResponse.cs              # { Success, Data, Message, Errors }
│   │
│   └── CR.Utils/
│       ├── StringExtensions.cs
│       └── DateTimeUtils.cs
│
├── 🎯 Services/
│   │
│   ├── 💼 Core/                            # Domain chính: sản phẩm, đơn hàng
│   │   │
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
│   │   │       └── SearchRequestDto.cs     # query, filters
│   │   │
│   │   ├── CR.Core.ApplicationServices/
│   │   │   ├── ProductModule/
│   │   │   │   └── IProductService.cs
│   │   │   ├── OrderModule/
│   │   │   │   ├── IOrderService.cs
│   │   │   │   └── IOrderAgentService.cs   # AI agent hủy đơn
│   │   │   ├── CustomerModule/
│   │   │   │   └── ICustomerService.cs
│   │   │   ├── AuthenticationModule/
│   │   │   │   └── IAuthService.cs
│   │   │   ├── Configs/
│   │   │   │   └── ApplicationServicesConfig.cs
│   │   │   └── Common/
│   │   │       └── ServiceImplementations/
│   │   │           ├── ProductService.cs
│   │   │           ├── OrderService.cs
│   │   │           └── OrderAgentService.cs
│   │   │
│   │   ├── CR.Core.Infrastructure/
│   │   │   ├── Persistence/
│   │   │   │   ├── CoreDbContext.cs        # : ApplicationDbContext<User>
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
│   │   └── CR.Core.API/                    # Composition Root
│   │       ├── Program.cs
│   │       ├── Controllers/
│   │       │   ├── ProductsController.cs
│   │       │   ├── OrdersController.cs
│   │       │   └── AuthController.cs
│   │       ├── appsettings.json
│   │       ├── appsettings.Development.json
│   │       └── appsettings.Production.json
│   │
│   ├── 🤖 AI/                              # AI service — microservice riêng
│   │   │
│   │   ├── CR.AI.Domain/
│   │   │   ├── SearchFilter.cs             # Kết quả NLP parsing
│   │   │   ├── GenerationLog.cs
│   │   │   └── RecommendationResult.cs
│   │   │
│   │   ├── CR.AI.Dtos/
│   │   │   ├── GenerateDescriptionRequestDto.cs
│   │   │   └── SearchParseResponseDto.cs
│   │   │
│   │   ├── CR.AI.ApplicationServices/
│   │   │   ├── IAISearchService.cs         # ParseQueryAsync()
│   │   │   ├── IAIGenerationService.cs     # GenerateDescriptionAsync()
│   │   │   ├── IRecommendationService.cs   # GetTopKAsync()
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
│   │   │   │   ├── AnthropicClient.cs      # Gọi Claude API
│   │   │   │   └── RateLimitedAIClient.cs  # Decorator: giới hạn quota
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
│       │   ├── OrderStatus.cs              # enum: Pending, Shipped...
│       │   └── AIProviders.cs
│       ├── CR.Common/
│       │   └── PaginatedResult.cs
│       └── CR.UserRolePermission/
│           └── PermissionConstants.cs
│
├── 🛠️ Tools/
│   ├── CR.Migrations/                      # Chạy migration độc lập
│   ├── CR.HostConsole/                     # CLI: seed data, jobs
│   └── CR.Project.AppHost/                 # Aspire orchestration
│
├── 🌐 WebApps/
│   └── CR.WebApp/                          # Angular (frontend)
│
└── eShop.sln
```
## Thiết kế ERD 



### Clean Architecture Design (Backend)
### Prerequisites

- **.NET 9 SDK** (or later)
- **SQL Server** (Express or LocalDB) or use Docker
- **Angular CLI** v17+

### Installation

#### 1. Backend Setup (ASP.NET Core)

```bash
cd eShop/eShop.API

# Restore NuGet packages (already done, but you can re-run if needed)
dotnet restore

# Create and apply database migrations
dotnet ef database update

# Run the backend server
dotnet run
```

Backend will be available at: `http://localhost:5000` (or the port shown in console)

**API Documentation (Swagger UI):** `http://localhost:5000/swagger`

#### 2. Frontend Setup (Angular)

```bash
cd eShop/eShop.Web

# Install npm packages
npm install

# Start the development server
npm start
# or
ng serve
```

Frontend will be available at: `http://localhost:4200`

### Configuration

#### Backend Configuration (`appsettings.json`)

- **Database Connection String:** Update if using different SQL Server instance
- **JWT Secret Key:** Change `your_super_secret_key_...` to a secure key in production
- **CORS Settings:** Already configured for `http://localhost:4200`

#### Frontend Configuration (`src/environments/environment.ts`)

- **API URL:** Set to your backend URL (default: `http://localhost:5000/api`)

### Database

#### Using SQL Server LocalDB

The project is configured to use LocalDB by default. The connection string is:
```
Server=(localdb)\mssqllocaldb;Database=eShop_Db;Trusted_Connection=true;Encrypt=false
```

#### Using Docker (Optional)

```bash
docker run -e 'ACCEPT_EULA=Y' -e 'SA_PASSWORD=YourPassword123' -p 1433:1433 mcr.microsoft.com/mssql/server:2022-latest
```

Update connection string in `appsettings.json`:
```json
"DefaultConnection": "Server=localhost,1433;Database=eShop_Db;User Id=sa;Password=YourPassword123;Encrypt=false"
```

### API Endpoints

#### Authentication
- `POST /api/auth/register` - Register new user
- `POST /api/auth/login` - Login user

#### Products
- `GET /api/products` - Get all products
- `GET /api/products/{id}` - Get product by ID
- `POST /api/products` - Create product (requires authentication)
- `PUT /api/products/{id}` - Update product (requires authentication)
- `DELETE /api/products/{id}` - Delete product (requires authentication)

### Features Implemented

✅ **Authentication**
- JWT-based authentication
- Register and login endpoints
- Secure password hashing (SHA256)

✅ **Authorization**
- Protected API endpoints with `[Authorize]` attribute
- Role-based access control (can be extended)

✅ **Database**
- Entity Framework Core with SQL Server
- Auto-migrations on startup (development)
- Seed data with sample products

✅ **Frontend**
- Angular v17 standalone components
- Login/Register pages
- Product listing
- Responsive navbar with authentication status
- Bootstrap 5 styling

✅ **HTTP Interceptor**
- Automatic JWT token attachment to requests
- 401 error handling with redirect to login

### Development Workflow

1. **Start Backend**
   ```bash
   cd eShop/eShop.API
   dotnet run
   ```

2. **Start Frontend** (in a new terminal)
   ```bash
   cd eShop/eShop.Web
   npm start
   ```

3. **Access Application**
   - Web: http://localhost:4200
   - API: http://localhost:5000
   - Swagger: http://localhost:5000/swagger

### Default Test Credentials

After running database migrations, seed data is available. You can register a new user or use the following flow:

1. Go to http://localhost:4200
2. Click "Register"
3. Create an account
4. Login with your credentials
5. Browse products

### Build for Production

#### Backend
```bash
cd eShop/eShop.API
dotnet publish -c Release
```

#### Frontend
```bash
cd eShop/eShop.Web
npm run build
# Output: dist/
```

### Troubleshooting

**CORS Errors**
- Ensure backend is running on port 5000
- Check CORS policy in `Program.cs`

**Database Connection Issues**
- Verify SQL Server is running
- Check connection string in `appsettings.json`
- For LocalDB: `(localdb)\mssqllocaldb` should exist

**Angular Build Errors**
- Delete `node_modules` folder
- Run `npm install` again
- Clear Angular cache: `ng cache clean`

**Port Already in Use**
- Backend: `dotnet run --urls http://localhost:5001`
- Frontend: `ng serve --port 4201`

### Next Steps

1. **Add Product Management Pages**
   - Create detailed product page
   - Edit/Delete product forms

2. **Add Shopping Cart**
   - Cart service and storage
   - Order creation and checkout

3. **Add User Profile**
   - User profile management
   - Order history

4. **Add Admin Panel**
   - User management
   - Product management
   - Order management

5. **Add Notifications**
   - Toast/snackbar for user feedback
   - ngx-toastr integration

6. **Deployment**
   - Deploy backend to Azure App Service
   - Deploy frontend to Azure Static Web Apps
   - Setup CI/CD pipeline

### License

MIT

### Support

For issues or questions, please create an issue in the repository.



## Tiến hành
#### 1. CR.Entities - thực thể dùng chung 
- ##### ` Library/CR.EntitiesBase/BaseEntity.cs`
   -  Mọi entity trong toàn solution đều kế thừa class này
   -  Không bao giờ tạo entity không có Id và audit fields
- ##### `Library/CR.EntitiesBase/BaseAuditEntity.cs`
   +  Dùng cho entity cần track người tạo/sửa (Orders, Products...)
#### 2. CR.Application - Service dùng chung 
- ######  `Library/CR.ApplicationBase/ServiceBase.cs`
   -  Base class cho TẤT CẢ service trong solution
   -  Cung cấp Logger và DbContext sẵn — không phải inject riêng từng service
- #####  `Library/CR.ApplicationBase/Localization/IMapErrorCode.cs`
   -  Contract để map error code → message theo ngôn ngữ
   -  Dùng khi hệ thống hỗ trợ đa ngôn ngữ (vi/en)
#### 3. CR.InfrastructureBase - Cơ sở DbContext
- ##### `Library/CR.InfrastructureBase/Persistence/ApplicationDbContext.cs`
   -  Base DbContext — mỗi service tạo DbContext riêng kế thừa từ đây
   -  TUser là entity User của service đó (không chia sẻ User table giữa services)
#### 4. CR.WebApiBase - Lớp bao bọc phản hồi và phần mềm trung gian xử lý ngoại lệ.
- ##### `Library/CR.WebAPIBase/Responses/ApiResponse.cs`
   -  MỌI API trong solution trả về format này — nhất quán cho frontend
- ##### `Library/CR.WebAPIBase/Middlewares/GlobalExceptionMiddleware.cs`
#### Layer CR.Core.* - chi tiết từng phần
- #### lớp miền 
   -  `Services/Core/CR.Core.Domain/Product/Product.cs`
   -  Entity thuần túy — KHÔNG import namespace nào từ Infrastructure hay Application
- #### Lớp ApplicationService - Giao diện trước triển khai sau

- #### CR.Constants & CR.Common
- #### CR.Core.Domain
- #### CR.Core.Dtos
- #### CR.Core.Application 
- 