# Project Backlog — Libris Digital Library System

## Project Objective
Build a full-stack web application (Angular + .NET) to manage library inventory, digital e-books, and user borrowing cycles.

---

## 1. Authentication & Security (EPIC)

- **US-01: User Registration & Login**
  - **User story:** As a user, I want to create an account and log in so that I can access personalized library features.
  - **Acceptance Criteria:**
    - System must validate unique email addresses.
    - Passwords must be hashed in the database (.NET Identity or BCrypt).
    - Login returns a valid JWT token.
  - **Priority:** Critical

- **US-02: Role-Based Access Control (RBAC)**
  - **User story:** As a system, I want to distinguish between Admins and Students so that sensitive data remains secure.
  - **Acceptance Criteria:**
    - Angular routes for `/admin` must be protected by an `AuthGuard`.
    - API endpoints must use `[Authorize(Roles = "Admin")]` decorators.
  - **Priority:** Critical

---

## 2. Catalog & Discovery (EPIC)

- **US-03: Book Search & Filtering**
  - **User story:** As a student, I want to search for books by title or genre so that I can find specific study materials quickly.
  - **Acceptance Criteria:**
    - Search must be case-insensitive.
    - Filters must work dynamically without page reloads (use Angular RxJS).
  - **Priority:** High

- **US-04: Digital Book Preview**
  - **User story:** As a student, I want to see book details and cover images so that I can decide if I want to borrow it.
  - **Acceptance Criteria:**
    - Display Title, Author, ISBN, Description, and Stock Level.
    - Show an "Out of Stock" badge if the count is 0.
  - **Priority:** Medium

---

## 3. Borrowing Logic (EPIC)

- **US-05: Physical Book Borrowing**
  - **User story:** As a student, I want to borrow a physical book so that I can use it for my studies.
  - **Acceptance Criteria:**
    - System must decrement `StockCount` by 1 upon success.
    - A `Loan` record must be created with a default 14-day return window.
    - Users cannot borrow more than 5 books at once.
  - **Priority:** High

- **US-06: E-Book Access**
  - **User story:** As a student, I want to download/view E-books so that I can study digitally.
  - **Acceptance Criteria:**
    - Only authorized users can access the PDF link.
    - The system tracks the number of downloads per book.
  - **Priority:** Medium

---

## 4. Administration (EPIC)

- **US-07: Inventory Management (CRUD)**
  - **User story:** As an admin, I want to add new books to the system so that the library collection stays updated.
  - **Acceptance Criteria:**
    - Admin can upload an image (cover) and a file (PDF).
    - Admin can edit existing book details or delete obsolete items.
  - **Priority:** High

- **US-08: Dashboard Analytics**
  - **User story:** As an admin, I want to see library statistics so that I can monitor system usage.
  - **Acceptance Criteria:**
    - Display total active loans and most borrowed genres.
    - List users with overdue books in a dedicated table.
  - **Priority:** Low (Bonus)

---

## 5. User Management (EPIC)

- **US-09: Borrowing History**
  - **User story:** As a student, I want to see my past loans so that I can keep track of my reading.
  - **Acceptance Criteria:**
    - Table view showing Book Title, Date Borrowed, and Date Returned.
  - **Priority:** Low

- **US-10: Profile Customization**
  - **User story:** As a user, I want to update my profile picture and contact info.
  - **Acceptance Criteria:**
    - Form validation for phone numbers and names.
  - **Priority:** Low

---

## 6. Technical Data Models (Schema)
Define models in the .NET backend and reflect DTOs/interfaces in the Angular frontend.

### A. User Model (Identity)
- `Id`: Guid/Int (Primary Key)
- `Email`: String (Unique)
- `PasswordHash`: String
- `FullName`: String
- `Role`: Enum (Admin, Student)
- `ProfilePictureUrl`: String
- `CreatedAt`: DateTime

### B. Book Model
- `Id`: Guid/Int (Primary Key)
- `Title`: String
- `Author`: String
- `ISBN`: String (Unique)
- `Description`: Text
- `Genre`: String
- `CoverImageUrl`: String
- `PdfUrl`: String (Optional, for E-books)
- `IsDigital`: Boolean
- `StockCount`: Integer (For physical copies)
- `DownloadCount`: Integer
- `PublishedDate`: DateTime

### C. Loan Model (Transaction)
- `Id`: Guid/Int (Primary Key)
- `BookId`: Int (Foreign Key -> Book)
- `UserId`: Guid (Foreign Key -> User)
- `LoanDate`: DateTime (Default: Now)
- `DueDate`: DateTime (Default: Now + 14 days)
- `ReturnDate`: DateTime? (Nullable until returned)
- `Status`: Enum (Active, Returned, Overdue)

### D. Review Model (Optional)
- `Id`: Guid/Int
- `BookId`: Int
- `UserId`: Guid
- `Rating`: Integer (1-5)
- `Comment`: Text
- `CreatedAt`: DateTime

---

## Notes & Next Steps
- Implement authentication using ASP.NET Identity and JWTs.
- Create Angular services for search, borrowing, and file downloads with proper guards.
- Define EF Core migrations for the models above.
