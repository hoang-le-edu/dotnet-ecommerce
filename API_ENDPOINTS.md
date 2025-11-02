# 📡 SIMPLCOMMERCE - API ENDPOINTS DOCUMENTATION

**Generated:** November 2025  
**Base URL (Local):** `https://localhost:49206`  
**Base URL (Azure):** `https://your-app.azurewebsites.net`

---

## 🔐 AUTHENTICATION

Hầu hết APIs yêu cầu authentication với roles:
- **admin** - Quản trị viên hệ thống
- **vendor** - Nhà cung cấp/người bán
- **customer** - Khách hàng (user thường)

**Headers cần thiết:**
```
Authorization: Bearer {JWT_TOKEN}
Content-Type: application/json
```

---

## 📋 MODULE: CATALOG (Products & Categories)

### 🛍️ **Products API**
**Base Route:** `/api/products`  
**Role Required:** `admin`, `vendor`

| Method | Endpoint | Description | Request Body |
|--------|----------|-------------|--------------|
| GET | `/api/products` | List all products | - |
| GET | `/api/products/{id}` | Get product by ID | - |
| POST | `/api/products` | Create new product | ProductForm |
| PUT | `/api/products/{id}` | Update product | ProductForm |
| DELETE | `/api/products/{id}` | Delete product | - |
| POST | `/api/products/{id}/media` | Upload product images | FormData |

**ProductForm Example:**
```json
{
  "name": "Product Name",
  "slug": "product-slug",
  "sku": "SKU-001",
  "price": 99.99,
  "oldPrice": 129.99,
  "shortDescription": "Short desc",
  "description": "Full description",
  "specification": "Tech specs",
  "isPublished": true,
  "isFeatured": false,
  "categoryIds": [1, 2],
  "brandId": 1
}
```

---

### 🏷️ **Categories API**
**Base Route:** `/api/categories`  
**Role Required:** `admin`, `vendor`

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/categories` | List all categories |
| GET | `/api/categories/{id}` | Get category by ID |
| POST | `/api/categories` | Create category |
| PUT | `/api/categories/{id}` | Update category |
| DELETE | `/api/categories/{id}` | Delete category |

**Category Example:**
```json
{
  "name": "Electronics",
  "slug": "electronics",
  "description": "Electronic products",
  "displayOrder": 1,
  "isPublished": true,
  "includeInMenu": true,
  "parentId": null
}
```

---

### 🔖 **Brands API**
**Base Route:** `/api/brands`  
**Role Required:** `admin`, `vendor`

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/brands` | List all brands |
| GET | `/api/brands/{id}` | Get brand by ID |
| POST | `/api/brands` | Create brand |
| PUT | `/api/brands/{id}` | Update brand |
| DELETE | `/api/brands/{id}` | Delete brand |

---

### 🎨 **Product Attributes API**
**Base Route:** `/api/product-attributes`  
**Role Required:** `admin`

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/product-attributes` | List attributes |
| GET | `/api/product-attributes/{id}` | Get attribute |
| POST | `/api/product-attributes` | Create attribute |
| PUT | `/api/product-attributes/{id}` | Update attribute |
| DELETE | `/api/product-attributes/{id}` | Delete attribute |

---

## 📦 MODULE: ORDERS

### 🛒 **Orders API**
**Base Route:** `/api/orders`  
**Role Required:** `admin`, `vendor`

| Method | Endpoint | Description | Query Params |
|--------|----------|-------------|--------------|
| GET | `/api/orders` | List orders | `status`, `numRecords` |
| GET | `/api/orders/{id}` | Get order details | - |
| PUT | `/api/orders/{id}/status` | Update order status | - |
| PUT | `/api/orders/{id}/cancel` | Cancel order | - |

**Query Examples:**
```
GET /api/orders?status=1&numRecords=20
GET /api/orders?status=2  (status: 1=New, 2=Processing, 3=Shipped, 4=Complete, 5=Canceled)
```

---

### 🧾 **Checkout API**
**Base Route:** `/api/checkout`  
**Role Required:** Authenticated user

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/checkout` | Create order from cart |
| GET | `/api/checkout/shipping-methods` | Get shipping options |
| GET | `/api/checkout/payment-methods` | Get payment options |

**Checkout Request:**
```json
{
  "shippingMethodId": 1,
  "paymentMethodId": 1,
  "shippingAddress": {
    "contactName": "John Doe",
    "phone": "+84912345678",
    "addressLine1": "123 Street",
    "city": "Ho Chi Minh",
    "stateOrProvinceId": 1,
    "countryId": 1,
    "zipCode": "700000"
  },
  "billingAddress": { /* same structure */ },
  "orderNote": "Please deliver before 5pm"
}
```

---

### 📄 **Invoice API**
**Base Route:** `/api/invoices`  
**Role Required:** `admin`, `vendor`

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/invoices/{orderId}` | Get invoice for order |
| POST | `/api/invoices/{orderId}/send-email` | Send invoice via email |

---

## 🛒 MODULE: SHOPPING CART

### 🛍️ **Cart API**
**Base Route:** `/api/cart`  
**Role Required:** Any (user or guest)

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/cart` | Get current cart |
| POST | `/api/cart/items` | Add item to cart |
| PUT | `/api/cart/items/{id}` | Update item quantity |
| DELETE | `/api/cart/items/{id}` | Remove item from cart |
| POST | `/api/cart/apply-coupon` | Apply coupon code |

**Add to Cart:**
```json
{
  "productId": 1,
  "quantity": 2,
  "productOptions": [
    { "key": "Size", "value": "L" },
    { "key": "Color", "value": "Red" }
  ]
}
```

---

## 👤 MODULE: CORE (Users, Roles, Settings)

### 👥 **Users API**
**Base Route:** `/api/users`  
**Role Required:** `admin`

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/users` | List all users |
| GET | `/api/users/{id}` | Get user by ID |
| POST | `/api/users` | Create user |
| PUT | `/api/users/{id}` | Update user |
| DELETE | `/api/users/{id}` | Delete user |
| PUT | `/api/users/{id}/change-password` | Change password |
| PUT | `/api/users/{id}/roles` | Update user roles |

**User Form:**
```json
{
  "email": "user@example.com",
  "fullName": "John Doe",
  "phoneNumber": "+84912345678",
  "roles": ["customer"],
  "password": "SecurePass123!"
}
```

---

### 🔐 **Roles API**
**Base Route:** `/api/roles`  
**Role Required:** `admin`

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/roles` | List all roles |
| GET | `/api/roles/{id}` | Get role by ID |
| POST | `/api/roles` | Create role |
| PUT | `/api/roles/{id}` | Update role |
| DELETE | `/api/roles/{id}` | Delete role |

---

### ⚙️ **App Settings API**
**Base Route:** `/api/app-settings`  
**Role Required:** `admin`

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/app-settings` | List all settings |
| GET | `/api/app-settings/{key}` | Get setting by key |
| PUT | `/api/app-settings/{key}` | Update setting |

**Common Settings:**
- `Global.DefaultCultureUI`
- `Theme.DefaultTheme`
- `Catalog.ProductPageSize`
- `Shipping.FreeShippingOrderAmount`

---

### 🌍 **Countries & States API**

**Countries:**
- GET `/api/countries`
- GET `/api/countries/{id}`
- POST `/api/countries`
- PUT `/api/countries/{id}`

**States/Provinces:**
- GET `/api/states` (query: `?countryId=1`)
- GET `/api/states/{id}`
- POST `/api/states`
- PUT `/api/states/{id}`

**Districts:**
- GET `/api/districts` (query: `?stateId=1`)
- GET `/api/districts/{id}`

---

## 📰 MODULE: CMS (Content Management)

### 📄 **Pages API**
**Base Route:** `/api/pages`  
**Role Required:** `admin`

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/pages` | List all pages |
| GET | `/api/pages/{id}` | Get page by ID |
| POST | `/api/pages` | Create page |
| PUT | `/api/pages/{id}` | Update page |
| DELETE | `/api/pages/{id}` | Delete page |

**Page Form:**
```json
{
  "name": "About Us",
  "slug": "about-us",
  "body": "<h1>About Us</h1><p>Content...</p>",
  "metaTitle": "About Us",
  "metaDescription": "About our company",
  "isPublished": true
}
```

---

### 📰 **News API**
**Base Route:** `/api/news-items`  
**Role Required:** `admin`

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/news-items` | List news |
| GET | `/api/news-items/{id}` | Get news by ID |
| POST | `/api/news-items` | Create news |
| PUT | `/api/news-items/{id}` | Update news |
| DELETE | `/api/news-items/{id}` | Delete news |

---

### 📝 **Menus API**
**Base Route:** `/api/menus`  
**Role Required:** `admin`

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/menus` | List menus |
| GET | `/api/menus/{id}` | Get menu by ID |
| POST | `/api/menus` | Create menu |
| PUT | `/api/menus/{id}` | Update menu |
| DELETE | `/api/menus/{id}` | Delete menu |

---

## 💳 MODULE: PAYMENTS

### 💰 **Payment Providers API**
**Base Route:** `/api/payment-providers`  
**Role Required:** `admin`

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/payment-providers` | List providers |
| GET | `/api/payment-providers/{id}` | Get provider config |
| PUT | `/api/payment-providers/{id}` | Update config |

**Available Providers:**
- Stripe
- PayPal Express
- Braintree
- Cash on Delivery (CoD)
- Momo Payment
- Ngan Luong
- Cashfree

---

### 💳 **Payment Processing**

**Stripe:**
- POST `/api/stripe/charge`
- POST `/api/stripe/webhook`

**PayPal:**
- POST `/api/paypal/create-payment`
- GET `/api/paypal/execute-payment`

**Momo:**
- POST `/api/momo/create-payment`
- POST `/api/momo/ipn` (callback)

---

## 🚚 MODULE: SHIPPING

### 📦 **Shipping Providers API**
**Base Route:** `/api/shipping-providers`  
**Role Required:** `admin`

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/shipping-providers` | List providers |
| PUT | `/api/shipping-providers/{id}` | Update config |

---

### 📊 **Table Rate API**
**Base Route:** `/api/shipping-table-rates`  
**Role Required:** `admin`

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/shipping-table-rates` | List rates |
| POST | `/api/shipping-table-rates` | Create rate |
| PUT | `/api/shipping-table-rates/{id}` | Update rate |
| DELETE | `/api/shipping-table-rates/{id}` | Delete rate |

---

## 📦 MODULE: INVENTORY

### 🏭 **Warehouses API**
**Base Route:** `/api/warehouses`  
**Role Required:** `admin`, `vendor`

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/warehouses` | List warehouses |
| GET | `/api/warehouses/{id}` | Get warehouse |
| POST | `/api/warehouses` | Create warehouse |
| PUT | `/api/warehouses/{id}` | Update warehouse |

---

### 📊 **Stock API**
**Base Route:** `/api/stock`  
**Role Required:** `admin`, `vendor`

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/stock` | List stock by warehouse |
| GET | `/api/stock/{productId}` | Get stock for product |
| PUT | `/api/stock/{productId}` | Update stock quantity |

---

## ⭐ MODULE: REVIEWS

### 🌟 **Reviews API**
**Base Route:** `/api/reviews`  
**Role Required:** Authenticated

| Method | Endpoint | Description | Role |
|--------|----------|-------------|------|
| GET | `/api/reviews` | List reviews | admin |
| GET | `/api/reviews/{productId}` | Get product reviews | any |
| POST | `/api/reviews` | Create review | customer |
| PUT | `/api/reviews/{id}` | Update review | admin |
| DELETE | `/api/reviews/{id}` | Delete review | admin |
| PUT | `/api/reviews/{id}/approve` | Approve review | admin |

**Review Form:**
```json
{
  "productId": 1,
  "rating": 5,
  "title": "Great product!",
  "comment": "I love this product because..."
}
```

---

## 🔍 MODULE: SEARCH

### 🔎 **Search API**
**Base Route:** `/api/search`  
**Role Required:** Public

| Method | Endpoint | Description | Query Params |
|--------|----------|-------------|--------------|
| GET | `/api/search` | Search products | `query`, `page`, `pageSize` |

**Example:**
```
GET /api/search?query=laptop&page=1&pageSize=20
```

**Response:**
```json
{
  "items": [
    {
      "id": 1,
      "name": "Laptop Dell",
      "slug": "laptop-dell",
      "price": 15000000,
      "thumbnail": "/media/products/laptop.jpg",
      "rating": 4.5
    }
  ],
  "totalItems": 50,
  "totalPages": 3
}
```

---

## 💬 MODULE: COMMENTS

### 💭 **Comments API**
**Base Route:** `/api/comments`  
**Role Required:** Authenticated

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/comments/{entityId}` | Get comments for entity |
| POST | `/api/comments` | Create comment |
| PUT | `/api/comments/{id}` | Update comment |
| DELETE | `/api/comments/{id}` | Delete comment |

---

## 💝 MODULE: WISHLIST

### ❤️ **Wishlist API**
**Base Route:** `/api/wishlist`  
**Role Required:** Authenticated

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/wishlist` | Get user's wishlist |
| POST | `/api/wishlist/items` | Add item |
| DELETE | `/api/wishlist/items/{productId}` | Remove item |

---

## 🏪 MODULE: VENDORS

### 🏬 **Vendors API**
**Base Route:** `/api/vendors`  
**Role Required:** `admin`

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/vendors` | List vendors |
| GET | `/api/vendors/{id}` | Get vendor |
| POST | `/api/vendors` | Create vendor |
| PUT | `/api/vendors/{id}` | Update vendor |
| DELETE | `/api/vendors/{id}` | Delete vendor |

---

## 📧 MODULE: CONTACTS

### 📬 **Contacts API**
**Base Route:** `/api/contacts`  
**Role Required:** `admin`

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/contacts` | List contact messages |
| GET | `/api/contacts/{id}` | Get contact message |
| DELETE | `/api/contacts/{id}` | Delete message |

---

## 💰 MODULE: PRICING (Promotions)

### 🎁 **Cart Rules API**
**Base Route:** `/api/cart-rules`  
**Role Required:** `admin`

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/cart-rules` | List cart rules (coupons) |
| GET | `/api/cart-rules/{id}` | Get cart rule |
| POST | `/api/cart-rules` | Create cart rule |
| PUT | `/api/cart-rules/{id}` | Update cart rule |
| DELETE | `/api/cart-rules/{id}` | Delete cart rule |

**Cart Rule Example:**
```json
{
  "name": "Summer Sale",
  "couponCode": "SUMMER2025",
  "discountAmount": 100000,
  "discountType": "fixed", // or "percentage"
  "startDate": "2025-06-01",
  "endDate": "2025-08-31",
  "isActive": true,
  "minimumOrderAmount": 500000
}
```

---

## 💵 MODULE: TAX

### 🧾 **Tax Classes API**
**Base Route:** `/api/tax-classes`  
**Role Required:** `admin`

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/tax-classes` | List tax classes |
| POST | `/api/tax-classes` | Create tax class |
| PUT | `/api/tax-classes/{id}` | Update tax class |

---

### 📊 **Tax Rates API**
**Base Route:** `/api/tax-rates`  
**Role Required:** `admin`

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/tax-rates` | List tax rates |
| POST | `/api/tax-rates` | Create tax rate |
| PUT | `/api/tax-rates/{id}` | Update tax rate |

---

## 🌐 MODULE: LOCALIZATION

### 🗣️ **Localization API**
**Base Route:** `/api/localizations`  
**Role Required:** `admin`

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/localizations` | List translations |
| GET | `/api/localizations/{culture}` | Get translations for culture |
| PUT | `/api/localizations` | Update translations |

**Supported Cultures:**
- `en-US` - English
- `vi-VN` - Tiếng Việt

---

## 🔔 MODULE: NOTIFICATIONS

### 📢 **Notifications API**
**Base Route:** `/api/notifications`  
**Role Required:** Authenticated

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/notifications` | Get user notifications |
| PUT | `/api/notifications/{id}/read` | Mark as read |
| DELETE | `/api/notifications/{id}` | Delete notification |

---

## 🎨 MODULE: WIDGETS

### 🧩 **Widgets API**
**Base Route:** `/api/widgets`  
**Role Required:** `admin`

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/widgets` | List available widgets |
| GET | `/api/widget-instances` | List widget instances |
| POST | `/api/widget-instances` | Create widget instance |
| PUT | `/api/widget-instances/{id}` | Update widget |
| DELETE | `/api/widget-instances/{id}` | Delete widget |

---

## 📊 COMMON RESPONSE FORMATS

### ✅ Success Response
```json
{
  "success": true,
  "data": { /* result data */ },
  "message": "Operation completed successfully"
}
```

### ❌ Error Response
```json
{
  "success": false,
  "errors": [
    {
      "field": "email",
      "message": "Email is required"
    }
  ],
  "message": "Validation failed"
}
```

### 📄 Paginated Response
```json
{
  "items": [ /* array of items */ ],
  "currentPage": 1,
  "pageSize": 20,
  "totalItems": 150,
  "totalPages": 8
}
```

---

## 🔐 AUTHENTICATION FLOW

### 1. Login
```
POST /api/account/login
```
**Request:**
```json
{
  "email": "admin@simplcommerce.com",
  "password": "your-password"
}
```

**Response:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "...",
  "expiresIn": 3600,
  "user": {
    "id": 10,
    "email": "admin@simplcommerce.com",
    "fullName": "Shop Admin",
    "roles": ["admin"]
  }
}
```

### 2. Use Token in Requests
```
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

---

## 📝 TESTING APIs

### Using cURL:
```bash
# Login
curl -X POST https://localhost:49206/api/account/login \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@simplcommerce.com","password":"1qazZAQ!"}'

# Get products (with token)
curl -X GET https://localhost:49206/api/products \
  -H "Authorization: Bearer YOUR_TOKEN_HERE"

# Create product
curl -X POST https://localhost:49206/api/products \
  -H "Authorization: Bearer YOUR_TOKEN_HERE" \
  -H "Content-Type: application/json" \
  -d '{"name":"New Product","price":99.99,...}'
```

### Using Postman:
1. Import collection (có thể tạo từ Swagger)
2. Set base URL: `{{baseUrl}}`
3. Set Authorization: Bearer Token
4. Test endpoints

---

## 🚀 SWAGGER / OpenAPI

Access API documentation UI:
```
https://localhost:49206/swagger
https://your-app.azurewebsites.net/swagger
```

---

## 📞 SUPPORT

- **GitHub:** https://github.com/simplcommerce/SimplCommerce
- **Documentation:** https://docs.simplcommerce.com
- **Issues:** https://github.com/simplcommerce/SimplCommerce/issues

---

**Last Updated:** November 2025

