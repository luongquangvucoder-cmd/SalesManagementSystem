# Sales Management System API - Quick Start Guide

## ?? Getting Started

### Prerequisites
- .NET 8 SDK installed
- SQL Server installed and running
- Visual Studio or VS Code
- Postman or similar API testing tool

### Setup Instructions

#### 1. **Clone the Repository**
```bash
git clone https://github.com/luongquangvucoder-cmd/SalesManagementSystem
cd SalesManagementSystem/"Sales Management System API"
```

#### 2. **Database Configuration**
Update `appsettings.json` with your SQL Server connection string:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=SalesManagementSystemDB;Trusted_Connection=true;TrustServerCertificate=true;"
  }
}
```

#### 3. **Apply Database Migrations**
```bash
dotnet ef database update
```

#### 4. **Configure Secrets**
Set up user secrets for sensitive data:
```bash
dotnet user-secrets set "JWT:Secret" "your-super-secret-key-min-32-chars"
dotnet user-secrets set "JWT:Issuer" "your-issuer"
dotnet user-secrets set "JWT:Audience" "your-audience"
dotnet user-secrets set "Email:SmtpServer" "smtp.gmail.com"
dotnet user-secrets set "Email:SmtpPort" "587"
dotnet user-secrets set "Email:SenderEmail" "your-email@gmail.com"
dotnet user-secrets set "Email:SenderPassword" "your-app-password"
```

#### 5. **Run the Application**
```bash
dotnet run
```

The API will be available at: `https://localhost:7245`
Swagger documentation: `https://localhost:7245/swagger/index.html`

---

## ?? Authentication Flow

### 1. **Register New User**
```http
POST /api/authentication/register
Content-Type: application/json

{
  "userName": "john_doe",
  "email": "john@example.com",
  "password": "SecurePassword123!",
  "fullName": "John Doe"
}
```

**Response:**
```json
{
  "success": true,
  "message": "User registered successfully. Please confirm your email.",
  "data": {
    "userId": "user-id-here",
    "email": "john@example.com"
  }
}
```

### 2. **Confirm Email**
After receiving confirmation email:
```http
POST /api/authentication/confirm-email
Content-Type: application/json

{
  "email": "john@example.com",
  "token": "email-confirmation-token-from-email"
}
```

### 3. **Login**
```http
POST /api/authentication/login
Content-Type: application/json

{
  "email": "john@example.com",
  "password": "SecurePassword123!"
}
```

**Response:**
```json
{
  "success": true,
  "message": "Login successful",
  "data": {
    "accessToken": "eyJhbGciOiJIUzI1NiIs...",
    "refreshToken": "refresh-token-here",
    "expiresIn": 600
  }
}
```

### 4. **Use Access Token**
Include in all authenticated requests:
```http
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...
```

### 5. **Refresh Access Token**
```http
POST /api/authentication/refresh-token
Content-Type: application/json

{
  "accessToken": "old-access-token",
  "refreshToken": "refresh-token"
}
```

---

## ?? Common API Workflows

### Create and Manage Products

#### 1. **Create Category**
```http
POST /api/category
Content-Type: application/json
Authorization: Bearer {token}

{
  "name": "Electronics",
  "parentId": null
}
```

#### 2. **Create Product**
```http
POST /api/product
Content-Type: application/json
Authorization: Bearer {token}

{
  "name": "iPhone 14",
  "description": "Latest Apple iPhone",
  "brand": "Apple",
  "categoryId": 1
}
```

#### 3. **Create Product Variant**
```http
POST /api/product/1/variants
Content-Type: application/json
Authorization: Bearer {token}

{
  "sku": "IPHONE-14-BLACK-128GB",
  "price": 999.99,
  "stockQuantity": 50
}
```

#### 4. **Add Product Image**
```http
POST /api/product/1/images
Content-Type: application/json
Authorization: Bearer {token}

{
  "imageUrl": "https://example.com/images/iphone-14.jpg",
  "isPrimary": true
}
```

### Shopping & Order Flow

#### 1. **Add to Cart**
```http
POST /api/cart/items
Content-Type: application/json
Authorization: Bearer {token}

{
  "productVariantId": 1,
  "quantity": 2
}
```

#### 2. **View Cart**
```http
GET /api/cart
Authorization: Bearer {token}
```

#### 3. **Create Address**
```http
POST /api/address
Content-Type: application/json
Authorization: Bearer {token}

{
  "fullName": "John Doe",
  "phoneNumber": "0912345678",
  "city": "Ho Chi Minh",
  "district": "District 1",
  "ward": "Ward 1",
  "detailAddress": "123 Main Street",
  "isDefault": true
}
```

#### 4. **Create Order**
```http
POST /api/order
Content-Type: application/json
Authorization: Bearer {token}

{
  "shippingAddress": "123 Main Street, District 1, HCMC",
  "receiverName": "John Doe",
  "receiverPhone": "0912345678"
}
```

#### 5. **Create Payment**
```http
POST /api/payment
Content-Type: application/json
Authorization: Bearer {token}

{
  "orderId": 1,
  "amount": 1999.98,
  "paymentMethod": "COD",
  "transactionCode": null
}
```

#### 6. **Complete Payment**
```http
PUT /api/payment/1/complete
Authorization: Bearer {token}
```

---

## ?? Dashboard & Analytics

### Get Dashboard Statistics
```http
GET /api/dashboard/stats
Authorization: Bearer {token}
```

### Get Category Sales
```http
GET /api/dashboard/category-sales
Authorization: Bearer {token}
```

### Get Period Statistics
```http
GET /api/dashboard/period-stats?period=monthly
Authorization: Bearer {token}
```

---

## ?? Inventory Management

### Import Stock
```http
POST /api/inventory/import?variantId=1&quantity=100&note=New shipment
Authorization: Bearer {token}
```

### Check Current Stock
```http
GET /api/inventory/stock/1
Authorization: Bearer {token}
```

### Get Low Stock Products
```http
GET /api/inventory/low-stock?threshold=20
Authorization: Bearer {token}
```

---

## ?? Filtering and Pagination

### Product Filtering Example
```http
GET /api/product?search=iPhone&categoryId=1&minPrice=500&maxPrice=1500&page=1&pageSize=10&sortBy=price&isDescending=true
Authorization: Bearer {token}
```

### Order Filtering Example
```http
GET /api/order?orderStatus=Completed&paymentStatus=Paid&fromDate=2024-01-01&toDate=2024-01-31&page=1&pageSize=20
Authorization: Bearer {token}
```

### Inventory Filtering Example
```http
GET /api/inventory/transactions?type=Export&fromDate=2024-01-01&toDate=2024-01-31&page=1&pageSize=10
Authorization: Bearer {token}
```

---

## ??? Admin Operations

### Manage User Roles
```http
POST /api/role/assign
Content-Type: application/json
Authorization: Bearer {admin-token}

{
  "userId": "user-id",
  "roleId": "admin-role-id"
}
```

### Update Order Status
```http
PUT /api/order/1/status
Content-Type: application/json
Authorization: Bearer {admin-token}

{
  "orderStatus": "Shipping"
}
```

### Adjust Inventory
```http
POST /api/inventory/adjust?variantId=1&quantity=-5&note=Damage report
Authorization: Bearer {admin-token}
```

---

## ?? Error Handling

### Standard Error Response Format
```json
{
  "success": false,
  "message": "Validation failed",
  "errors": [
    "Email already exists",
    "Password must contain uppercase letter"
  ]
}
```

### HTTP Status Codes
- `200 OK` - Success
- `201 Created` - Resource created
- `400 Bad Request` - Invalid input
- `401 Unauthorized` - Authentication required
- `403 Forbidden` - Permission denied
- `404 Not Found` - Resource not found
- `409 Conflict` - Duplicate data
- `500 Internal Server Error` - Server error

---

## ?? Testing

### Using Postman
1. Create a new collection
2. Import all endpoints
3. Set `{{base_url}}` = `https://localhost:7245`
4. Set `{{token}}` from login response
5. Use in Authorization header: `Bearer {{token}}`

### Using cURL
```bash
# Login
curl -X POST https://localhost:7245/api/authentication/login \
  -H "Content-Type: application/json" \
  -d '{"email":"john@example.com","password":"SecurePassword123!"}'

# Get Products
curl -X GET https://localhost:7245/api/product \
  -H "Authorization: Bearer YOUR_ACCESS_TOKEN"
```

---

## ?? API Documentation

Full Swagger documentation available at:
```
https://localhost:7245/swagger/index.html
```

---

## ?? Security Best Practices

1. **Never commit secrets** - Use dotnet user-secrets
2. **Always use HTTPS** - Enforce in production
3. **Validate all inputs** - Both client and server-side
4. **Use environment variables** - For configuration
5. **Implement rate limiting** - To prevent abuse
6. **Keep dependencies updated** - Regular security patches
7. **Use strong passwords** - Enforce password policy
8. **Enable CORS carefully** - Only allow trusted origins

---

## ?? Support & Resources

- **GitHub Repository:** https://github.com/luongquangvucoder-cmd/SalesManagementSystem
- **Project Skills Document:** See PROJECT_SKILL.md
- **Features Documentation:** See FEATURES_COMPLETED.md

---

## ?? Next Steps

1. ? Set up development environment
2. ? Create test data
3. ? Implement frontend integration
4. ? Add payment gateway integration
5. ? Deploy to production

Happy coding! ??
