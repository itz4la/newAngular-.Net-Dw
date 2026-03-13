# Project Documentation — Libris Digital Library System + Data Warehouse

> **Workspace analysis** of [`project backlog.md`](./project%20backlog.md), the [`api/`](./api) backend, and the [`client/`](./client) frontend.

---

## Table of Contents

1. [Project Overview](#1-project-overview)
2. [System Architecture](#2-system-architecture)
3. [Backend — `api/`](#3-backend--api)
   - 3.1 [Technology Stack](#31-technology-stack)
   - 3.2 [Data Models](#32-data-models)
   - 3.3 [DTOs](#33-dtos)
   - 3.4 [Repository Pattern](#34-repository-pattern)
   - 3.5 [REST API Endpoints](#35-rest-api-endpoints)
   - 3.6 [Authentication & Security](#36-authentication--security)
   - 3.7 [Database Configuration](#37-database-configuration)
4. [Frontend — `client/`](#4-frontend--client)
   - 4.1 [Technology Stack](#41-technology-stack)
   - 4.2 [Project Structure](#42-project-structure)
   - 4.3 [Routing & Guards](#43-routing--guards)
   - 4.4 [Admin Module](#44-admin-module)
   - 4.5 [Client (Student) Module](#45-client-student-module)
   - 4.6 [Authentication Module](#46-authentication-module)
   - 4.7 [Shared Services & DTOs](#47-shared-services--dtos)
5. [Data Warehouse — `api/models/DW/`](#5-data-warehouse--apimodelsdw)
   - 5.1 [Star Schema Design](#51-star-schema-design)
   - 5.2 [ETL Pipeline (SSIS)](#52-etl-pipeline-ssis)
   - 5.3 [Analytics API — 14 KPI Endpoints](#53-analytics-api--14-kpi-endpoints)
   - 5.4 [Dashboard Charts (Frontend)](#54-dashboard-charts-frontend)
6. [Backlog Implementation Status](#6-backlog-implementation-status)
7. [Setup & Configuration Guide](#7-setup--configuration-guide)
8. [API Quick Reference](#8-api-quick-reference)

---

## 1. Project Overview

**Libris** is a full-stack web application built with **Angular 19** and **.NET 9** that manages a digital library — book inventory, borrowing cycles, user accounts, and administrative reporting.

A second, integrated component provides a **Data Warehouse** layer powered by the WideWorldImporters (WWI) sample database, exposed through a Star Schema queried via a dedicated analytics API and rendered on the admin dashboard as interactive charts.

| Attribute | Value |
|-----------|-------|
| **Project name** | Libris Digital Library System |
| **Frontend** | Angular 19 (standalone components, lazy-loaded modules) |
| **Backend** | ASP.NET Core 9 Web API |
| **ORM** | Entity Framework Core 9 |
| **Auth** | ASP.NET Identity + JWT Bearer tokens |
| **Operational DB** | SQL Server — `DigitalLibrary` |
| **Data Warehouse DB** | SQL Server — `WWI_DataWarehouse` |
| **ETL tool** | SQL Server Integration Services (SSIS) |
| **Deployment target** | Firebase (frontend), localhost/IIS (backend) |

---

## 2. System Architecture

```
┌─────────────────────────────────────────────────────────────────────┐
│                         Angular 19 SPA                              │
│  ┌───────────────┐  ┌──────────────────┐  ┌──────────────────────┐ │
│  │ /authentication│  │ /client (student)│  │ /admin (admin panel) │ │
│  │ login/register│  │ browse, details  │  │ dashboard, CRUD      │ │
│  │ PublicGuard   │  │ UserGuard        │  │ AdminGuard           │ │
│  └───────────────┘  └──────────────────┘  └──────────────────────┘ │
└────────────────────────────────┬────────────────────────────────────┘
                                 │  HTTP (JWT Bearer)
┌────────────────────────────────▼────────────────────────────────────┐
│                     ASP.NET Core 9 Web API                          │
│  UserController  BookController  GenreController  LoanController   │
│  AnalyticsController  SeedDataController                            │
│  ──────────────────────────────────────────────────────────────     │
│  Repository Pattern:                                                │
│  IUserRepository  IBookRepository  IGenreRepository                 │
│  ILoanRepository  IAnalyticsRepository                              │
└────────────┬───────────────────────────────────┬────────────────────┘
             │ EF Core (ApplicationContext)       │ EF Core (DWContext)
┌────────────▼───────────┐           ┌────────────▼───────────────────┐
│   SQL Server           │           │   SQL Server                   │
│   DigitalLibrary       │           │   WWI_DataWarehouse            │
│   (operational DB)     │  ← SSIS → │   (star schema)                │
│   Users, Books,        │           │   Fact_Sales + 5 Dimensions    │
│   Genres, Loans        │           │                                │
└────────────────────────┘           └────────────────────────────────┘
```

---

## 3. Backend — `api/`

### 3.1 Technology Stack

| Package | Purpose |
|---------|---------|
| `Microsoft.AspNetCore` (.NET 9) | Web API host |
| `Microsoft.EntityFrameworkCore.SqlServer` | ORM + SQL Server provider |
| `Microsoft.AspNetCore.Identity.EntityFrameworkCore` | User/role management |
| `Microsoft.AspNetCore.Authentication.JwtBearer` | JWT authentication middleware |
| `Microsoft.IdentityModel.Tokens` | Token generation & validation |
| `Swashbuckle.AspNetCore` | Swagger / OpenAPI docs |

### 3.2 Data Models

#### `ApplicationUser` (`models/ApplicationUser.cs`)
Extends `IdentityUser` — all Identity fields (Id, UserName, Email, PasswordHash, etc.) are inherited.

#### `Genre` (`models/Genre.cs`)
| Field | Type | Constraints |
|-------|------|-------------|
| `Id` | `int` | PK, auto-increment |
| `Name` | `string` | Required, max 50 chars |
| `Books` | `ICollection<Book>` | Navigation — one-to-many |

#### `Book` (`models/Book.cs`)
| Field | Type | Constraints |
|-------|------|-------------|
| `Id` | `int` | PK |
| `Title` | `string` | Required, max 150 |
| `Author` | `string` | Required, max 100 |
| `Description` | `string` | `text` column |
| `GenreId` | `int` | FK → Genre |
| `Genre` | `Genre` | Navigation property |
| `CoverImageUrl` | `string` | max 255 |
| `PublishedDate` | `DateTime` | Required |

#### `Loan` (`models/Loan.cs`)
| Field | Type | Constraints |
|-------|------|-------------|
| `Id` | `int` | PK |
| `BookId` | `int` | FK → Book |
| `UserId` | `string` | FK → ApplicationUser |
| `LoanDate` | `DateTime` | Default: `DateTime.Now` |
| `DueDate` | `DateTime` | Default: now + 14 days |
| `ReturnDate` | `DateTime?` | Nullable until returned |
| `Status` | `LoanStatus` | Enum: `Active=1`, `Returned=2`, `Overdue=3` |

#### Data Warehouse Models (`models/DW/`)
| Model | Maps to |
|-------|---------|
| `DimDate` | `Dim_Date` |
| `DimCustomer` | `Dim_Customer` |
| `DimStockItem` | `Dim_StockItem` |
| `DimEmployee` | `Dim_Employee` |
| `DimCity` | `Dim_City` |
| `FactSales` | `Fact_Sales` |

### 3.3 DTOs

#### Book DTOs (`DTOs/Book/`)
| DTO | Used for |
|-----|----------|
| `BookDto` | Read — includes `GenreName`, `IsAvailable` |
| `CreateBookDto` | POST request body |
| `UpdateBookDto` | PUT request body |
| `BookFilterDto` | Query params: `Title`, `Author`, `GenreId`, `PageNumber`, `PageSize` |
| `PagedResultDto<T>` | Paginated response wrapper |

#### Loan DTOs (`DTOs/Loan/`)
| DTO | Used for |
|-----|----------|
| `LoanDto` | Read loan details |
| `CreateLoanDto` | POST — requires `UserId` + `BookId` |
| `ReturnLoanDto` | POST return — optional `ReturnDate` |
| `LoanFilterDto` | Pagination + status filter |
| `LoanValidationResult` | Validation outcome (max 5 active loans, book availability) |

#### User DTOs (`DTOs/User/`)
| DTO | Used for |
|-----|----------|
| `UserDto` | Read user info + role |
| `RegisterDTO` | Public registration |
| `LoginDTO` | Auth request → JWT response |
| `CreateAdminUserDto` | Admin-created accounts |
| `UpdateUserDto` | PUT user update |
| `UserFilterDto` | Pagination |

#### Analytics DTOs (`DTOs/Analytics/AnalyticsDtos.cs`)
14 strongly-typed DTOs matching each KPI endpoint (see §5.3).

### 3.4 Repository Pattern

All data access lives behind interfaces, injected via ASP.NET Core DI:

| Interface | Implementation | Registered in `Program.cs` |
|-----------|---------------|----------------------------|
| `IBookRepository` | `BookRepository` | `AddScoped` |
| `IGenreRepository` | `GenreRepository` | `AddScoped` |
| `ILoanRepository` | `LoanRepository` | `AddScoped` |
| `IUserRepository` | `UserRepository` | `AddScoped` |
| `IAnalyticsRepository` | `AnalyticsRepository` | `AddScoped` |

**`ILoanRepository` key methods:**
```csharp
Task<LoanValidationResult> ValidateLoanCreationAsync(string userId, int bookId);
// Rules: max 5 active loans per user, book must be available
Task<LoanDto> ReturnLoanAsync(int loanId, DateTime? returnDate = null);
Task UpdateOverdueLoansAsync();  // bulk-marks past-due loans as Overdue
```

### 3.5 REST API Endpoints

#### `UserController` — `/api/user`

| Method | Route | Description |
|--------|-------|-------------|
| `POST` | `/register` | Public — create a new `Client`-role account |
| `POST` | `/login` | Public — returns JWT token |
| `GET` | `/` | List all users (paginated) |
| `GET` | `/{id}` | Get user by ID |
| `GET` | `/non-admin` | List non-admin users |
| `POST` | `/admin` | Create an admin user |
| `PUT` | `/{id}` | Update user |
| `DELETE` | `/{id}` | Delete user |

#### `BookController` — `/api/book`

| Method | Route | Description |
|--------|-------|-------------|
| `GET` | `/` | List all books (filtered & paginated) |
| `GET` | `/{id}` | Get book by ID |
| `GET` | `/available` | List books where `IsAvailable = true` |
| `POST` | `/` | Create a book |
| `PUT` | `/{id}` | Update a book |
| `DELETE` | `/{id}` | Delete a book |

#### `GenreController` — `/api/genre`

| Method | Route | Description |
|--------|-------|-------------|
| `GET` | `/` | List all genres |
| `GET` | `/{id}` | Get genre by ID |
| `POST` | `/` | Create a genre |
| `PUT` | `/{id}` | Update a genre |
| `DELETE` | `/{id}` | Delete a genre |

#### `LoanController` — `/api/loan`

| Method | Route | Description |
|--------|-------|-------------|
| `GET` | `/` | List all loans (filtered & paginated) |
| `GET` | `/{id}` | Get loan by ID |
| `GET` | `/user/{userId}/active` | All active loans for a user |
| `GET` | `/user/{userId}/loan-count` | Active loan count + max allowed (5) |
| `GET` | `/book/{bookId}/availability` | Check if book can be borrowed |
| `POST` | `/` | Create a loan (validates rules) |
| `POST` | `/return/{loanId}` | Return a borrowed book |
| `POST` | `/update-overdue` | Bulk-update overdue status |

#### `AnalyticsController` — `/api/analytics`

See [§5.3](#53-analytics-api--14-kpi-endpoints) for the full DW analytics endpoint table.

### 3.6 Authentication & Security

- **Identity store:** `ApplicationContext` (EF Core, SQL Server)
- **Password hashing:** ASP.NET Identity default (PBKDF2)
- **Token format:** JWT — signed with `HMAC-SHA256`
- **Token lifetime:** 1 hour (`expires: DateTime.Now.AddHours(1)`)
- **Claims issued at login:** `username`, `userId`, `jti`, `role(s)`
- **Token storage (frontend):** `localStorage` key `PR_TK` — stored encrypted via `SecurityService`
- **CORS policy:** `AllowAll` (any origin/header/method — for development)

JWT configuration keys in `appsettings.json`:
```json
"JWT": {
  "Audience": "http://localhost:4200",
  "Issuer": "http://localhost:4200",
  "SecretKey": "<secret>"
}
```

> ⚠️ **Security note:** Rotate `SecretKey` before production deployment. Consider `[Authorize]` decorators on controllers for full enforcement.

### 3.7 Database Configuration

Two `DbContext` instances are registered:

| Context | Connection string key | Purpose |
|---------|-----------------------|---------|
| `ApplicationContext` | `cnx` | Operational DB — users, books, genres, loans |
| `DWContext` | `DataWarehouseDb` | Read-only analytics against WWI_DataWarehouse |

Default connection strings in `appsettings.json`:
```json
"ConnectionStrings": {
  "cnx":            "Server=(localdb)\\ProjectModels;Database=DigitalLibrary;...",
  "DataWarehouseDb": "Server=localhost;Database=WWI_DataWarehouse;..."
}
```

EF Core migrations are located in `api/Migrations/`.

---

## 4. Frontend — `client/`

### 4.1 Technology Stack

| Package | Purpose |
|---------|---------|
| Angular 19 | SPA framework |
| `@auth0/angular-jwt` | JWT decode / expiry check |
| `rxjs` | Reactive state & HTTP |
| Firebase | Hosting target (`.firebaserc`) |
| Bootstrap / custom SCSS | Styling (`assets/template/`) |

### 4.2 Project Structure

```
client/src/app/
├── app.component.*         Root component
├── app.config.ts           Bootstrap config (providers, HttpClient, router)
├── app.routes.ts           Top-level lazy-loaded routes
│
├── authentication/         Public auth pages
│   ├── login/
│   └── register/
│
├── client/                 Student-facing portal
│   ├── home/
│   ├── browse/             Book catalog with search/filter
│   ├── product-details/    Book detail + borrow action
│   ├── my-purchase/        Active & past loans
│   └── profile/            Profile view/edit
│
├── admin/                  Admin panel (protected)
│   ├── dashboard/          DW analytics charts
│   ├── products/           Book CRUD
│   ├── categories/         Genre CRUD
│   ├── orders/             Loan management
│   └── users/              User CRUD
│
└── shared/
    ├── guards/             Route guards
    ├── services/           Auth, HTTP, Security services
    ├── dtos/               TypeScript interfaces mirroring backend DTOs
    └── validators/         Custom form validators
```

### 4.3 Routing & Guards

**Top-level routes** (`app.routes.ts`):

| Path | Module | Guard | Description |
|------|--------|-------|-------------|
| `/authentication` | `AuthenticationRoutingModule` | `PublicGuard` | Redirects logged-in users away from login/register |
| `/client` | `ClientRoutingModule` | `UserGuard` | Requires valid JWT (any role) |
| `/admin` | `AdminRoutingModule` | `AdminGuard` | Requires `Admin` role claim |

**`AuthService`** (`shared/services/auth.service.ts`):
- Reads encrypted token from `localStorage` (`PR_TK`)
- Decodes JWT payload with `@auth0/angular-jwt`
- Exposes `authStatus: Observable<boolean>` and `activeUser: Observable<any>`
- Methods: `signin()`, `logout()`, `getToken()`, `getUserInformation()`

**`SecurityService`** encrypts/decrypts the stored token before writing to `localStorage`.

### 4.4 Admin Module

Located at `/admin` — protected by `AdminGuard`.

#### Dashboard (`/admin/dashboard`)
Interactive analytics dashboard pulling from the DW analytics API.  
Contains sub-components (each a standalone chart):

| Component | KPI visualised |
|-----------|----------------|
| `kpi-cards` | Dashboard summary (revenue, orders, customers, products) |
| `revenue-yearly-chart` | Total revenue per year (line/bar chart) |
| `revenue-monthly-chart` | Month-over-month comparison |
| `revenue-quarterly-chart` | Quarterly breakdown |
| `revenue-territory-chart` | Revenue by sales territory (map/bar) |
| `revenue-state-chart` | Revenue by state/province |
| `top-products-chart` | Top 10 best-selling products |
| `brand-performance-chart` | Revenue by product brand |
| `color-revenue-chart` | Revenue by product colour |
| `salesperson-chart` | Salesperson leaderboard |
| `customer-category-chart` | Revenue by customer category |
| `top-customers-chart` | Top VIP customers |
| `sales-day-chart` | Sales by day of week |
| `city-avg-order-chart` | Average order value by city |
| `date-filter` | Shared date-range filter input |

#### Products (`/admin/products`)
Book CRUD — list, create (`/products/create`), edit (`/products/edit/:id`).

#### Categories (`/admin/categories`)
Genre CRUD — list, create, edit.

#### Orders (`/admin/orders`)
Loan management — list all loans, create a new loan.

#### Users (`/admin/users`)
User CRUD — list, create (`/users/create`), edit (`/users/edit/:id`).

### 4.5 Client (Student) Module

Located at `/client` — protected by `UserGuard`.

| Route | Component | Description |
|-------|-----------|-------------|
| `/client/home` | `HomeComponent` | Landing page |
| `/client/browse` | `BrowseComponent` | Search/filter book catalog |
| `/client/product-details/:id` | `ProductDetailsComponent` | Book detail + borrow button |
| `/client/my-purchase` | `MyPurchaseComponent` | Active & returned loans |
| `/client/profile` | `ProfileComponent` | View and edit profile |

### 4.6 Authentication Module

Located at `/authentication` — protected by `PublicGuard` (redirects already-logged-in users).

| Route | Component |
|-------|-----------|
| `/authentication/login` | `LoginComponent` |
| `/authentication/register` | `RegisterComponent` |

### 4.7 Shared Services & DTOs

#### Services

| Service | Responsibility |
|---------|----------------|
| `AuthService` | JWT read/write, auth status stream |
| `SecurityService` | Token encryption/decryption (AES or similar) |
| `RequestsService` | Generic typed HTTP wrapper (uses `HttpClient`) |

#### TypeScript DTOs (`shared/dtos/`)

| File | Mirrors backend DTO |
|------|---------------------|
| `book.dto.ts` | `BookDto`, `CreateBookDto`, `UpdateBookDto`, `BookFilterDto` |
| `genre.dto.ts` | `GenreDto` |
| `loan.dto.ts` | `LoanDto`, `CreateLoanDto`, `ReturnLoanDto` |
| `user.dto.ts` | `UserDto`, `LoginDTO`, `RegisterDTO` |
| `paged-result.dto.ts` | `PagedResultDto<T>` |

---

## 5. Data Warehouse — `api/models/DW/`

### 5.1 Star Schema Design

Source system: **WideWorldImporters (WWI)** standard SQL Server sample database.  
Target: **`WWI_DataWarehouse`** — a 5-dimension Star Schema.

```
               ┌─────────────┐
               │  Dim_Date   │
               │ DateSK (PK) │
               └──────┬──────┘
                      │
┌────────────┐  ┌─────▼──────────┐  ┌────────────────┐
│ Dim_Customer│  │   Fact_Sales   │  │ Dim_StockItem  │
│CustomerSK  ├──┤ SalesSK (PK)   ├──┤ StockItemSK    │
│CustomerID  │  │ InvoiceLineID  │  │ StockItemID    │
│CustomerName│  │ DateSK         │  │ StockItemName  │
│Category    │  │ CustomerSK     │  │ Brand/Size     │
│BuyingGroup │  │ StockItemSK    │  │ ColorName      │
└────────────┘  │ EmployeeSK     │  │ UnitPrice      │
                │ CitySK         │  └────────────────┘
┌────────────┐  │ Quantity       │  ┌────────────────┐
│ Dim_Employee│  │ TotalAmount    │  │   Dim_City     │
│EmployeeSK  ├──┤                ├──┤ CitySK         │
│ PersonID   │  └────────────────┘  │ CityID         │
│EmployeeName│                      │ CityName       │
│IsSalesperson                      │ StateProvince  │
└────────────┘                      │ SalesTerritory │
                                    └────────────────┘
```

### 5.2 ETL Pipeline (SSIS)

The ETL is split into two SSIS packages:

#### Package 1 — `Load_Dimensions.dtsx`

Five Data Flow Tasks, one per dimension, each using **Upsert** logic:

| Dimension | Strategy | Key |
|-----------|----------|-----|
| `Dim_Date` | Insert-only + Lookup filter (skip existing dates) | `FullDate` |
| `Dim_Customer` | SCD Type 1 Wizard — overwrites changed attributes | `CustomerID` |
| `Dim_StockItem` | SCD Type 1 Wizard | `StockItemID` |
| `Dim_Employee` | SCD Type 1 Wizard | `PersonID` |
| `Dim_City` | SCD Type 1 Wizard | `CityID` |

#### Package 2 — `Load_Fact_Sales.dtsx`

1. OLE DB Source queries `Sales.InvoiceLines` joined to `Sales.Invoices`, `Sales.Customers`.
2. Five sequential **Lookup** blocks resolve dimension surrogate keys: `DateSK`, `CustomerSK`, `StockItemSK`, `EmployeeSK`, `CitySK`.
3. Final **anti-duplication Lookup** against `Fact_Sales.InvoiceLineID` — only new rows (no-match output) are inserted.

> **Run order:** Execute `Load_Dimensions.dtsx` first, then `Load_Fact_Sales.dtsx`.

### 5.3 Analytics API — 14 KPI Endpoints

All endpoints are on `/api/analytics`. All accept optional `?from=` and `?to=` date-range query parameters.

| # | Method | Route | DTO returned | Description |
|---|--------|-------|-------------|-------------|
| — | `GET` | `/date-range` | `DateRangeDto` | Min/max date available in `Dim_Date` |
| 12 | `GET` | `/dashboard` | `DashboardSummaryDto` | High-level KPIs (revenue, orders, customers, products, salespersons, AOV) |
| 1 | `GET` | `/revenue/yearly` | `List<RevenueByYearDto>` | Total revenue + orders per year |
| 10 | `GET` | `/revenue/monthly` | `List<MonthlyRevenueDto>` | Month-over-month YoY comparison |
| 11 | `GET` | `/revenue/quarterly` | `List<QuarterlyRevenueDto>` | Quarterly breakdown |
| 4 | `GET` | `/revenue/territory` | `List<RevenueByTerritoryDto>` | Revenue by sales territory |
| 13 | `GET` | `/revenue/state` | `List<RevenueByStateDto>` | Revenue by state/province |
| 2 | `GET` | `/products/top?top=10` | `List<TopProductDto>` | Top N best-selling products |
| 8 | `GET` | `/products/brands` | `List<BrandPerformanceDto>` | Revenue + units per brand |
| 14 | `GET` | `/products/colors` | `List<ColorRevenueDto>` | Revenue by product colour |
| 3 | `GET` | `/employees/leaderboard` | `List<SalespersonDto>` | Salesperson revenue ranking |
| 5 | `GET` | `/customers/categories` | `List<CustomerCategoryRevenueDto>` | Revenue by customer category |
| 7 | `GET` | `/customers/top-vip?top=5` | `List<TopCustomerDto>` | Top N VIP customers by lifetime spend |
| 6 | `GET` | `/sales/day-of-week` | `List<SalesByDayDto>` | Units & revenue per day of week |
| 9 | `GET` | `/cities/avg-order` | `List<CityAvgOrderDto>` | Average order value per city |

### 5.4 Dashboard Charts (Frontend)

The admin dashboard (`/admin/dashboard`) renders all 14 KPIs through dedicated standalone Angular components housed under `client/src/app/admin/dashboard/`. A shared `date-filter` component emits date-range changes that all charts subscribe to via the dashboard's analytics service.

---

## 6. Backlog Implementation Status

### Epic 1 — Authentication & Security

| ID | User Story | Status | Notes |
|----|------------|--------|-------|
| US-01 | User Registration & Login | ✅ **Done** | `POST /api/user/register` + `POST /api/user/login` → JWT. `RegisterDTO` validates unique username. |
| US-02 | Role-Based Access Control | ✅ **Done** | Roles: `Client` (auto-assigned on register), `Admin`. Angular `AdminGuard` / `UserGuard`. Backend `[Authorize]` setup in `Program.cs`. |

### Epic 2 — Catalog & Discovery

| ID | User Story | Status | Notes |
|----|------------|--------|-------|
| US-03 | Book Search & Filtering | ✅ **Done** | `GET /api/book?Title=...&GenreId=...&PageNumber=1&PageSize=10` via `BookFilterDto`. Client `BrowseComponent` renders filtered results. |
| US-04 | Digital Book Preview | ✅ **Done** | `GET /api/book/{id}` returns `BookDto` with `CoverImageUrl`, genre, availability. `ProductDetailsComponent` displays all fields. |

### Epic 3 — Borrowing Logic

| ID | User Story | Status | Notes |
|----|------------|--------|-------|
| US-05 | Physical Book Borrowing | ✅ **Done** | `POST /api/loan` with validation (max 5 active loans, availability check). `LoanRepository.ValidateLoanCreationAsync`. |
| US-06 | E-Book Access | ⚠️ **Partial** | `Book` model has `CoverImageUrl` but no `PdfUrl` / `IsDigital` / `DownloadCount` fields yet (PascalCase, consistent with C# conventions). PDF delivery & download tracking not implemented. |

### Epic 4 — Administration

| ID | User Story | Status | Notes |
|----|------------|--------|-------|
| US-07 | Inventory Management (CRUD) | ✅ **Done** | Full Book + Genre CRUD in `BookController` / `GenreController`. Admin UI at `/admin/products` and `/admin/categories`. |
| US-08 | Dashboard Analytics | ✅ **Done** (Bonus) | 14-endpoint DW analytics API + 14 chart components on the admin dashboard. |

### Epic 5 — User Management

| ID | User Story | Status | Notes |
|----|------------|--------|-------|
| US-09 | Borrowing History | ✅ **Done** | `GET /api/loan/user/{userId}/active` + paginated loan list. Client `/my-purchase` page. |
| US-10 | Profile Customisation | ⚠️ **Partial** | `ProfileComponent` exists. `UpdateUserDto` present. `ProfilePictureUrl` not in `ApplicationUser` model; form validation partially implemented. |

---

## 7. Setup & Configuration Guide

### Prerequisites

- .NET 9 SDK
- Node.js ≥ 20 + npm
- SQL Server (local or remote)
- Visual Studio 2022 / VS Code
- (Optional) SQL Server Integration Services — for ETL only

### Backend Setup (`api/`)

```bash
# 1. Update connection strings in api/appsettings.json
#    "cnx": pointing to your SQL Server instance
#    "DataWarehouseDb": pointing to WWI_DataWarehouse (optional — needed for analytics)

# 2. Apply EF Core migrations to create the DigitalLibrary database
cd api
dotnet ef database update

# 3. (Optional) Seed initial data
#    Call POST /api/seeddata endpoints or add seed logic in Program.cs

# 4. Run the API
dotnet run
# Swagger UI: https://localhost:{port}/swagger
```

### Data Warehouse Setup (`api/DW.md`)

1. In SSMS, create a blank database named `WWI_DataWarehouse`.
2. Run the DDL scripts from `api/DW.md` (Phase 2) to create the star schema tables.
3. Open Visual Studio → create a new SSIS project.
4. Configure two OLE DB connection managers: `Source_WWI` → `WideWorldImporters`, `Dest_DW` → `WWI_DataWarehouse`.
5. Build and execute `Load_Dimensions.dtsx` first, then `Load_Fact_Sales.dtsx`.

### Frontend Setup (`client/`)

```bash
cd client
npm install

# Development server (proxies API calls to localhost)
ng serve
# App runs at http://localhost:4200

# Production build
ng build
```

### Firebase Deployment (Frontend)

```bash
npm install -g firebase-tools
firebase login
cd client
ng build
firebase deploy
```

---

## 8. API Quick Reference

Base URL: `http://localhost:{port}/api`

### Authentication

```
POST /user/register    { username, email, password }
POST /user/login       { username, password }   → { token, expiration, username }
```

### Books

```
GET    /book                    ?Title&Author&GenreId&PageNumber&PageSize
GET    /book/{id}
GET    /book/available
POST   /book                    { title, author, description, genreId, coverImageUrl, publishedDate }
PUT    /book/{id}               { same fields }
DELETE /book/{id}
```

### Genres

```
GET    /genre
GET    /genre/{id}
POST   /genre                   { name }
PUT    /genre/{id}              { name }
DELETE /genre/{id}
```

### Loans

```
GET    /loan                    ?PageNumber&PageSize&Status
GET    /loan/{id}
GET    /loan/user/{userId}/active
GET    /loan/user/{userId}/loan-count
GET    /loan/book/{bookId}/availability
POST   /loan                    { userId, bookId }
POST   /loan/return/{loanId}    { returnDate? }
POST   /loan/update-overdue
```

### Users (Admin)

```
GET    /user                    ?PageNumber&PageSize
GET    /user/{id}
GET    /user/non-admin
POST   /user/admin              { username, email, password }
PUT    /user/{id}               { username, email, ... }
DELETE /user/{id}
```

### Analytics (Data Warehouse)

```
GET    /analytics/date-range
GET    /analytics/dashboard             ?from&to
GET    /analytics/revenue/yearly        ?from&to
GET    /analytics/revenue/monthly       ?from&to
GET    /analytics/revenue/quarterly     ?from&to
GET    /analytics/revenue/territory     ?from&to
GET    /analytics/revenue/state         ?from&to
GET    /analytics/products/top          ?top=10&from&to
GET    /analytics/products/brands       ?from&to
GET    /analytics/products/colors       ?from&to
GET    /analytics/employees/leaderboard ?from&to
GET    /analytics/customers/categories  ?from&to
GET    /analytics/customers/top-vip     ?top=5&from&to
GET    /analytics/sales/day-of-week     ?from&to
GET    /analytics/cities/avg-order      ?from&to
```

---

*Generated by workspace analysis — `project backlog.md` · `api/` · `client/`*
