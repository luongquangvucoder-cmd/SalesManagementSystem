# PROJECT_SKILL.md — Sales Management System API

# Sales Management System API
## Complete Development Skill & Architecture Guide

---

# 1. PROJECT OVERVIEW

## Project Type

Backend RESTful API for:

- Sales Management
- Ecommerce
- Inventory Management
- Order Processing
- Authentication & Authorization

---

# 2. TECHNOLOGY STACK

## Backend

- ASP.NET Core Web API (.NET 8)
- Entity Framework Core
- SQL Server
- ASP.NET Identity
- JWT Authentication
- Refresh Token
- API Versioning
- Swagger / OpenAPI

---

## Architecture

Project follows:

```txt
Clean Architecture + Service Repository Pattern
````

Flow:

```txt
Controller
    ↓
Service
    ↓
Repository
    ↓
DbContext
    ↓
SQL Server
```

---

# 3. CORE DEVELOPMENT PRINCIPLES

## Always prioritize:

* Clean code
* Scalability
* Maintainability
* Performance
* Security
* Readability
* Consistency

---

# 4. PROJECT STRUCTURE

```txt
SalesManagementSystem/
│
├── Controllers/
│   └── v1/
│
├── Services/
│   ├── Interfaces/
│   └── Implementations
│
├── Repositories/
│   ├── Interfaces/
│   └── Implementations
│
├── Models/
│
├── DTO/
│
├── Data/
│
├── Middleware/
│
├── Exceptions/
│
├── Helpers/
│
├── Configurations/
│
├── Extensions/
│
├── Mappings/
│
├── Validators/
│
├── Constants/
│
└── Program.cs
```

---

# 5. ARCHITECTURE RULES

# Controller Responsibilities

Controllers ONLY:

* Receive request
* Validate ModelState
* Call service
* Return response

Controllers MUST NOT:

* Access DbContext
* Write LINQ query
* Write business logic
* Handle transactions
* Hash passwords
* Generate JWT manually

---

# Service Responsibilities

Services contain:

* Business logic
* Validation logic
* Authorization rules
* Transaction logic
* Token generation
* Workflow processing

Services MUST NOT:

* Return Entity directly
* Return IActionResult
* Access HTTP context directly unless necessary

---

# Repository Responsibilities

Repositories ONLY:

* Query database
* CRUD operations
* Pagination
* Filtering
* Sorting

Repositories MUST NOT:

* Throw business exceptions
* Return HTTP responses
* Handle JWT
* Handle Email

---

# DbContext Responsibilities

DbContext ONLY:

* Database mapping
* Relationships
* Indexes
* Constraints
* Fluent API configuration

---

# 6. CODING STANDARDS

# Naming Convention

## PascalCase

Use for:

* Class
* Method
* Property
* DTO
* Enum

Example:

```csharp
public class ProductService
```

---

## camelCase

Use for:

```csharp
private readonly IProductRepository _productRepository;
```

---

## Interface Naming

Always prefix with:

```txt
I
```

Example:

```csharp
IProductService
```

---

# Async Naming

All async methods MUST end with:

```txt
Async
```

Example:

```csharp
GetByIdAsync()
```

---

# 7. API RESPONSE STANDARD

All APIs MUST return:

```json
{
  "success": true,
  "message": "string",
  "data": {}
}
```

---

# ApiResponse Format

```csharp
public class ApiResponse<T>
{
    public bool Success { get; set; }

    public string Message { get; set; }

    public T? Data { get; set; }

    public List<string>? Errors { get; set; }
}
```

---

# Success Response Example

```json
{
  "success": true,
  "message": "Product created successfully",
  "data": {
    "productId": 1
  }
}
```

---

# Error Response Example

```json
{
  "success": false,
  "message": "Validation failed",
  "errors": [
    "Email already exists"
  ]
}
```

---

# 8. EXCEPTION HANDLING

# Use Global Exception Middleware

All exceptions handled by:

```txt
ExceptionMiddleware
```

---

# Allowed Custom Exceptions

```txt
BadRequestException
UnauthorizedException
ForbiddenException
NotFoundException
ConflictException
```

---

# NEVER USE

```csharp
throw new Exception(...)
```

unless unexpected system failure.

---

# Exception Mapping

| Exception             | Status Code |
| --------------------- | ----------- |
| BadRequestException   | 400         |
| UnauthorizedException | 401         |
| ForbiddenException    | 403         |
| NotFoundException     | 404         |
| ConflictException     | 409         |
| Unhandled Exception   | 500         |

---

# 9. VALIDATION RULES

# DTO Validation

Use:

* Required
* StringLength
* Range
* Compare
* EmailAddress
* RegularExpression

Example:

```csharp
[Required]
[StringLength(100)]
public string Name { get; set; }
```

---

# Validation Layers

## DTO Validation

For:

* null
* empty
* format
* length

---

## Service Validation

For:

* duplicate data
* business rules
* relationship validation
* authorization

---

# 10. DATABASE DESIGN RULES

# Every table MUST have:

## Primary Key

```txt
Id
```

or:

```txt
ProductId
CategoryId
```

---

## CreatedAt

```csharp
DateTime CreatedAt
```

---

## Soft Delete

Use:

```csharp
bool IsActive
```

instead of hard delete.

---

# NEVER HARD DELETE IMPORTANT DATA

Examples:

* Users
* Orders
* Products

---

# 11. ENTITY FRAMEWORK RULES

# Always Use AsNoTracking()

For read-only queries:

```csharp
_context.Products
    .AsNoTracking()
```

---

# Avoid N+1 Query

BAD:

```csharp
foreach(var product in products)
{
    product.Category.Name
}
```

---

GOOD:

```csharp
.Include(p => p.Category)
```

or projection.

---

# Prefer Projection

GOOD:

```csharp
.Select(x => new ProductDto())
```

BETTER than:

```csharp
.Include()
```

when only few fields needed.

---

# Never Call ToList Early

BAD:

```csharp
.ToList()
.Where()
```

GOOD:

```csharp
.Where()
.Select()
.ToListAsync()
```

---

# Pagination Required

For large list:

```csharp
.Skip((page - 1) * pageSize)
.Take(pageSize)
```

---

# 12. INDEXING RULES

# Must Index

## Foreign Keys

Examples:

* CategoryId
* ProductId
* UserId

---

## Search Fields

Examples:

* Email
* UserName
* SKU
* Product Name

---

## Filter Fields

Examples:

* CreatedAt
* Status
* IsActive

---

# Composite Index Examples

```csharp
.HasIndex(x => new
{
    x.CategoryId,
    x.Status
});
```

---

# 13. IDENTITY RULES

# ASP.NET Identity Handles

* Password hashing
* SecurityStamp
* Lockout
* EmailConfirmed
* User normalize
* Password validation

---

# NEVER HASH PASSWORD MANUALLY

Always use:

```csharp
_userManager.CreateAsync()
```

---

# Require Unique Email

```csharp
options.User.RequireUniqueEmail = true;
```

---

# 14. AUTHENTICATION RULES

# Login Methods

Allow:

* Email login
* Username login

---

# JWT Rules

## Access Token

Lifetime:

```txt
10 minutes
```

---

## Refresh Token

Lifetime:

```txt
7 days
```

Stored in database.

---

# Never Store Sensitive Data in JWT

DO NOT store:

* password
* refresh token
* personal secrets

---

# Email Confirmation Required

User MUST verify email before login.

---

# 15. REFRESH TOKEN RULES

# Refresh Token Table

Must contain:

* Token
* JwtId
* UserId
* DateExpire
* IsRevoked

---

# Refresh Flow

```txt
Access Token expired
    ↓
Client sends Refresh Token
    ↓
Server validates Refresh Token
    ↓
Generate new Access Token
```

---

# Logout Flow

Logout should:

```txt
Set RefreshToken.IsRevoked = true
```

---

# 16. EMAIL SYSTEM RULES

# Use MailKit

Never use deprecated SMTP libraries.

---

# NEVER COMMIT

* SMTP password
* JWT secret
* SQL password

---

# Use Environment Variables

OR:

```bash
dotnet user-secrets
```

---

# Email Features

Implemented:

* Email confirmation
* Forgot password
* Reset password
* Resend email confirmation

---

# 17. CATEGORY RULES

# Category Structure

Tree structure:

```txt
ParentId
```

---

# Prevent Invalid Tree

Cannot:

* self parent
* recursive loop

---

# Category Delete Rule

Soft delete only.

Cannot deactivate category if:

```txt
Has active children
```

---

# 18. PRODUCT RULES

# Product

Contains:

* Name
* Brand
* Description
* Category

---

# ProductVariant

Contains:

* SKU
* Price
* Stock
* Variant combinations

---

# SKU MUST BE UNIQUE

```csharp
.HasIndex(x => x.SKU)
.IsUnique()
```

---

# 19. CART RULES

# One User = One Cart

```csharp
.HasIndex(x => x.UserId)
.IsUnique()
```

---

# CartItem Rules

Composite unique:

```txt
CartId + ProductVariantId
```

---

# 20. ORDER RULES

# Order Status

Use enum:

```txt
Pending
Confirmed
Shipping
Completed
Cancelled
```

---

# Order Never Deleted

Only update status.

---

# 21. PAYMENT RULES

# Payment TransactionCode

Must be unique.

---

# Payment Methods

Examples:

* COD
* VNPay
* MoMo
* Stripe

---

# 22. SECURITY RULES

# Never Trust Client Data

Always validate server-side.

---

# Protect Against

* SQL Injection
* XSS
* Brute force
* Token replay
* Duplicate request

---

# Enable HTTPS

Production MUST use HTTPS.

---

# 23. PERFORMANCE RULES

# Priority Order

1. Database optimization
2. Index optimization
3. Projection query
4. Pagination
5. Caching
6. Reduced Includes
7. Async everywhere

---

# Future Optimization

Planned:

* Redis cache
* Distributed cache
* CQRS
* Background jobs
* Queue system

---

# 24. SWAGGER RULES

All APIs must:

* have summary
* have response type
* support JWT
* support versioning

---

# 25. API VERSIONING RULES

Use:

```txt
/api/v1/
/api/v2/
```

Never break old client version.

---

# 26. LOGGING RULES

Need future logging for:

* Authentication
* Payment
* Order
* Exception
* Admin actions

---

# 27. GIT RULES

# Branch Naming

```txt
feature/auth
feature/product
feature/category
bugfix/login
refactor/category-query
```

---

# Commit Naming

```txt
feat: add refresh token
fix: validate duplicate email
refactor: optimize product query
```

---

# NEVER PUSH

* bin/
* obj/
* secrets
* production config

---

# 28. CLEAN CODE RULES

# Method Size

Prefer:

```txt
< 50 lines
```

---

# One Responsibility

One method should do:

```txt
ONE THING
```

---

# Avoid Duplicate Logic

Shared logic → Helper / Service.

---

# 29. FUTURE FEATURES

Planned:

* Product review
* Wishlist
* Coupons
* VNPay
* MoMo
* Dashboard analytics
* Inventory management
* Redis caching
* Admin dashboard
* Role management
* Real-time notifications
* Search engine
* Elasticsearch
* Audit logging

---

# 30. FINAL DEVELOPMENT RULES

Every new feature MUST:

* use async/await
* use DTO
* validate properly
* optimize query
* avoid duplicate logic
* follow architecture
* support scalability
* support maintainability
* support security
* support performance

---

# 31. IMPORTANT PROJECT PHILOSOPHY

Priority:

```txt
Readable > Clever
Maintainable > Complex
Consistent > Fancy
Secure > Fast
Optimized > Premature optimization
```

---

# 32. AI CODING INSTRUCTION

When generating code for this project:

ALWAYS:

* follow current architecture
* use repository pattern
* use DTO mapping
* use async EF Core
* optimize LINQ
* add indexes if necessary
* use custom exceptions
* avoid business logic in controller
* follow naming conventions
* keep scalability in mind

NEVER:

* return Entity directly
* use synchronous database calls
* use raw SQL unnecessarily
* use Exception directly
* hardcode secrets
* create fat controllers
* duplicate validation logic
