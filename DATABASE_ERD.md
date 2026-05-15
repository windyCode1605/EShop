```mermaid
erDiagram
    %% ===== CATALOG (Thương mại) =====
    CATEGORY ||--o{ CATEGORY : "parent_id"
    CATEGORY ||--o{ PRODUCT : "contains"
    PRODUCT ||--o{ PRODUCTVARIANT : "has"
    PRODUCT ||--o{ PRODUCTIMAGE : "has"
    PRODUCT ||--o{ REVIEWS : "reviewed_by"

    %% ===== CART & ORDER =====
    USERS ||--|| CART : "owns"
    CART ||--o{ CARTITEM : "contains"
    CARTITEM }o--|| PRODUCTVARIANT : "for"
    
    USERS ||--o{ ORDER : "creates"
    ORDER ||--o{ ORDERITEM : "contains"
    ORDERITEM }o--|| PRODUCTVARIANT : "for"
    ORDERITEM ||--o{ ORDERREFUND : "has"
    
    ADDRESSES }o--|| USERS : "belongs_to"
    ORDER }o--|| ADDRESSES : "ships_to"

    %% ===== PAYMENT & LOGISTICS =====
    ORDER ||--o{ PAYMENTS : "has"
    ORDER ||--o{ SHIPMENT : "has_many"
    
    %% ===== PROMOTION =====
    COUPONS ||--o{ COUPONUSAGE : "used_in"
    ORDER ||--o{ COUPONUSAGE : "applies"
    USERS ||--o{ COUPONUSAGE : "uses"

    %% ===== USER & AUTHENTICATION =====
    USERS ||--|| USERPROFILE : "has"
    USERS ||--o{ AUTHOTP : "generates"
    USERS ||--o{ NOTIFICATIONTOKEN : "has"
    USERS ||--o{ REVIEWS : "writes"

    %% ===== PERMISSION =====
    USERS ||--o{ USERROLE : "assigned"
    USERROLE }o--|| ROLE : "grants"
    ROLE ||--o{ ROLEPERMISSION : "has"
    
    USERS ||--o{ CUSTOMERUSERROLE : "has_customer_role"
    CUSTOMERUSERROLE }o--|| CUSTOMERROLE : "grants"
    CUSTOMERROLE ||--o{ CUSTOMERROLEPERMISSION : "has"

    %% ===== SYSTEM CONFIG =====
    %% SYSVAR standalone

    %% ===== ENTITY DEFINITIONS =====
    CATEGORY {
        int id PK
        int parent_id FK
        string name
        string slug UK
        datetime created_date
        datetime modified_date
        bit deleted
    }

    PRODUCT {
        int id PK
        int category_id FK
        string name
        string slug UK
        decimal base_price
        string description
        datetime created_date
        datetime modified_date
        bit deleted
    }

    PRODUCTVARIANT {
        int id PK
        int product_id FK
        string sku UK
        string size
        string color
        decimal price_adjustment
        int stock_quantity
        datetime created_date
        datetime modified_date
        bit deleted
    }

    PRODUCTIMAGE {
        int id PK
        int product_id FK
        string url
        int sort_order
        bit is_primary
        datetime created_date
        datetime modified_date
        bit deleted
    }

    CART {
        int id PK
        int user_id FK UK
        datetime last_updated_at
    }

    CARTITEM {
        int id PK
        int cart_id FK
        int product_variant_id FK
        int quantity
    }

    USERS {
        int id PK
        string username UK
        string email UK
        string phone
        string password_hash
        int user_type
        int status
        int login_fail_count
        datetime last_login
        datetime created_date
        datetime modified_date
        bit deleted
    }

    USERPROFILE {
        int id PK
        int user_id FK
        string full_name
        string phone_number
        datetime date_of_birth
        int gender
        string avatar_url
    }

    ADDRESSES {
        int id PK
        int user_id FK
        string street
        string city
        string province
        bit is_default
        datetime created_date
        datetime modified_date
        bit deleted
    }

    ORDER {
        int id PK
        string order_code UK
        int user_id FK
        string shipping_address
        decimal total_amount
        string status
        string payment_method
        datetime created_date
        datetime modified_date
        bit deleted
    }

    ORDERITEM {
        int id PK
        int order_id FK
        int product_variant_id FK
        int quantity
        string product_name
        string variant_sku
        decimal unit_price
    }

    ORDERREFUND {
        int id PK
        int order_item_id FK
        int refund_quantity
        string reason
        string status
        datetime created_date
        datetime modified_date
        bit deleted
    }

    PAYMENTS {
        int id PK
        int order_id FK
        string method
        string status
        decimal amount
        datetime paid_at
    }

    SHIPMENT {
        int id PK
        int order_id FK
        string shipping_provider
        string tracking_number
        decimal shipping_fee
        string receiver_name
        string receiver_phone
        string shipping_address
        datetime estimated_delivery
        datetime actual_delivery
        string status
        datetime created_date
        datetime modified_date
        bit deleted
    }

    COUPONS {
        int id PK
        string code UK
        int discount_type
        decimal discount_value
        decimal min_order_value
        decimal max_discount_value
        datetime start_date
        datetime expiry_date
        int usage_limit
        int used_count
        int usage_limit_per_user
        bit is_active
        datetime created_date
        datetime modified_date
        bit deleted
    }

    COUPONUSAGE {
        int id PK
        int coupon_id FK
        int user_id FK
        int order_id FK
        decimal discount_amount
        datetime used_at
    }

    AUTHOTP {
        int id PK
        string otp_code
        datetime expire_time
        bit is_used
        int user_id FK
        int verify_time
    }

    NOTIFICATIONTOKEN {
        int id PK
        string fcm_token
        string apns_token
        int user_id FK
    }

    REVIEWS {
        int id PK
        int user_id FK
        int product_id FK
        int rating
        string comment
        datetime created_at
    }

    ROLE {
        int id PK
        string name
        string description
        int status
        datetime created_date
        datetime modified_date
        bit deleted
    }

    ROLEPERMISSION {
        int id PK
        int role_id FK
        string permission_key
        datetime created_date
        int created_by
    }

    USERROLE {
        int id PK
        int user_id FK
        int role_id FK
        datetime created_date
        datetime modified_date
        bit deleted
    }

    CUSTOMERROLE {
        int id PK
        string name
        int user_type
        int customer_id
        string description
        int status
        datetime created_date
        datetime modified_date
        bit deleted
    }

    CUSTOMERROLEPERMISSION {
        int id PK
        int customer_role_id FK
        string permission_key
        datetime created_date
        int created_by
    }

    CUSTOMERUSERROLE {
        int id PK
        int user_id FK
        int customer_role_id FK
        datetime created_date
        datetime modified_date
        bit deleted
    }

    SYSVAR {
        int id PK
        string gr_name
        string var_name
        string var_value
        string var_desc
    }

    SENDOTP {
        int id PK
        string username
        int send_count
        datetime last_sent_date_time
        datetime time_limit_can_verify_otp
    }
```

**Ghi chú:**
- `PK`: Primary Key
- `FK`: Foreign Key
- `UK`: Unique Key
- `||--o{`: 1-to-Many (1 bảng này có nhiều bảng kia)
- `||--||`: 1-to-1 (1 bảng này có 1 bảng kia)
- `}o--||`: Many-to-1 (từ bảng này có nhiều liên hệ tới bảng kia)
