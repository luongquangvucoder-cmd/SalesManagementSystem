# Sales Management System API

A modern **Sales Management System API** built with **ASP.NET Core Web API**, **Entity Framework Core**, and **SQL Server** following **Clean Architecture**, **Repository Pattern**, and **JWT Authentication**.

---

# 🚀 Features

## Authentication & Authorization

* JWT Authentication
* Refresh Token Authentication
* Email Confirmation
* Forgot Password
* Reset Password
* Resend Email Confirmation
* Role-Based Authorization
* Secure Password Validation
* Custom Exception Middleware

---

## Category Management

* Create Category
* Update Category
* Soft Delete Category
* Restore Category
* Category Tree Structure
* Parent / Child Categories
* Search & Filter
* Pagination

---

## Product Management

* Product CRUD
* Product Variant Management
* Product Images
* Stock Management
* Product Search
* Product Filtering
* Product Sorting
* Pagination

---

## Shopping Features

* Cart
* Cart Items
* Orders
* Order Items
* Payment Management
* Shipping Address

---

# 🛠 Technologies

* ASP.NET Core Web API
* Entity Framework Core
* SQL Server
* ASP.NET Identity
* JWT Authentication
* MailKit
* Swagger / OpenAPI
* LINQ
* Repository Pattern
* RESTful API

---

# 📂 Project Structure

```bash
SalesManagementSystem/
│
├── Controllers/
├── Services/
├── Repositories/
├── Models/
├── DTO/
├── Middleware/
├── Data/
├── Migrations/
├── Interfaces/
└── Program.cs
```

---

# 🔐 Authentication Flow

## Register

1. User registers account
2. System sends confirmation email
3. User confirms email
4. User can login

---

## Login

1. Validate Email / Username
2. Validate Password
3. Generate JWT Token
4. Generate Refresh Token

---

## Refresh Token

* Access Token expires quickly
* Refresh Token used to generate new Access Token
* Supports persistent login like Facebook

---

# 🗄 Database Design

## Main Tables

* AspNetUsers
* RefreshTokens
* Categories
* Products
* ProductVariants
* ProductImages
* Carts
* CartItems
* Orders
* OrderItems
* Payments
* Addresses

---

# ⚡ Performance Optimization

* AsNoTracking()
* LINQ Projection
* Database Indexing
* Pagination
* Soft Delete
* Query Optimization
* Composite Indexes

---

# 📧 Email Features

* Email Confirmation
* Forgot Password
* Reset Password
* Resend Confirmation Email

Using:

* Gmail SMTP
* MailKit

---

# 📖 API Documentation

Swagger UI:

```bash
https://localhost:{port}/swagger
```

---

# ⚙️ Setup Instructions

## 1. Clone Repository

```bash
git clone https://github.com/luongquangvucoder-cmd/SalesManagementSystem.git
```

---

## 2. Configure appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "YOUR_CONNECTION_STRING"
  },

  "JWT": {
    "Secret": "YOUR_SECRET_KEY",
    "Issuer": "YOUR_ISSUER",
    "Audience": "YOUR_AUDIENCE"
  },

  "EmailSettings": {
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": 587,
    "SenderEmail": "YOUR_EMAIL",
    "SenderPassword": "YOUR_APP_PASSWORD"
  }
}
```

---

## 3. Run Migration

```bash
add-migration InitialCreate
update-database
```

---

## 4. Run Project

```bash
dotnet run
```

---

# 🔒 Security

* JWT Authentication
* Refresh Token Rotation
* Password Hashing
* Email Verification
* Identity Validation
* Exception Middleware
* Role Authorization

---

# 📌 Future Improvements

* Redis Caching
* Docker
* CI/CD
* VNPay Integration
* MoMo Integration
* Product Reviews
* Wishlist
* Dashboard Analytics
* SignalR Notifications
* Cloud Image Storage
* ElasticSearch

---

# 👨‍💻 Author

Developed by Luong Quang Vu

---

# 📄 License

This project is for learning and portfolio purposes.
