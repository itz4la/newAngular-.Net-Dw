# Libris Digital Library System - Product Backlog

## Project Overview
Libris is a full-stack web application built with Angular and .NET designed to manage library inventory, digital e-books, and user borrowing cycles. The system supports user authentication, role-based access control (Admin/Student), book catalog management, borrowing/returning functionality, and administrative oversight.

## Epics and User Stories

---

### EPIC 1: Authentication & Security

#### US-01: User Registration & Login
**User Story:** As a user, I want to create an account and log in so that I can access personalized library features.

**Acceptance Criteria:**
- System must validate unique email addresses
- Passwords must be hashed in the database (.NET Identity or BCrypt)
- Login returns a valid JWT token

**Test Cases:**
- **Positive:** User submits valid email (user@example.com) and matching passwords (≥8 characters); account is created successfully; user receives confirmation email; login with same credentials returns valid JWT token
- **Negative:** User submits invalid email format (e.g., userexample.com); system displays "Invalid email format" error; account not created
- **Negative:** User submits password with insufficient strength (<8 characters); error message "Password must be at least 8 characters" shown; account not created
- **Negative:** User attempts to register with existing email; system displays "Email already registered" error
- **Edge:** User submits registration with empty fields; appropriate field-specific validation errors shown
- **Edge:** User logs in with correct credentials; redirected to dashboard with valid session maintained until token expiry
- **Edge:** User logs in with incorrect password; error message "Invalid credentials" displayed; no redirect occurs

#### US-02: Role-Based Access Control (RBAC)
**User Story:** As a system, I want to distinguish between Admins and Students so that sensitive data remains secure.

**Acceptance Criteria:**
- Angular routes for `/admin` must be protected by an `AuthGuard`
- API endpoints must use `[Authorize(Roles = "Admin")]` decorators

**Test Cases:**
- **Positive:** Admin user logs in; can access `/admin/*` routes without redirection to login
- **Positive:** Admin user attempts to access Admin API endpoints (e.g., `/api/User/admin`); request succeeds with 200 OK
- **Negative:** Student user logs in; attempts to access `/admin/*` routes; redirected to login page with unauthorized message
- **Negative:** Student user attempts to access Admin API endpoints; receives 403 Forbidden response
- **Edge:** Unauthenticated user attempts to access admin routes; redirected to login page
- **Edge:** User token expires while on admin page; system detects expired token, logs user out, redirects to login

---

### EPIC 2: Catalog & Discovery

#### US-03: Book Search & Filtering
**User Story:** As a student, I want to search for books by title or genre so that I can find specific study materials quickly.

**Acceptance Criteria:**
- Search must be case-insensitive
- Filters must work dynamically without page reloads (use Angular RxJS)

**Test Cases:**
- **Positive:** User searches for "harry potter"; system returns books containing "harry potter" in title (case-insensitive match)
- **Positive:** User selects "Science Fiction" genre filter; system displays only books with that genre
- **Positive:** User combines search ("java") and filter ("Programming"); system returns books matching both criteria
- **Positive:** User clears search/filter; system returns to full book list
- **Negative:** User searches for nonexistent title ("xyz123"); system displays "No books found" message
- **Edge:** User searches with special characters ("C++"); system handles escaping correctly and returns relevant results
- **Edge:** User types rapidly in search field; debouncing prevents excessive API calls (verified via network tab)
- **Edge:** Filter updates immediately when selection changes without full page reload (verified by checking URL doesn't change and no browser refresh occurs)

#### US-04: Digital Book Preview
**User Story:** As a student, I want to see book details and cover images so that I can decide if I want to borrow it.

**Acceptance Criteria:**
- Display Title, Author, ISBN, Description, and Stock Level
- Show an "Out of Stock" badge if the count is 0

**Test Cases:**
- **Positive:** User views book details page; system displays Title, Author, ISBN, Description, and Stock Level accurately
- **Positive:** Book with StockLevel > 0 shows "In Stock" badge; book with StockLevel = 0 shows "Out of Stock" badge
- **Positive:** Book cover image loads correctly when CoverImageUrl is provided
- **Positive:** For e-books (IsDigital = true), system indicates availability for download/view
- **Negative:** Book with missing cover image; system displays placeholder image or appropriate fallback
- **Negative:** Book with empty description; system handles gracefully (shows "No description available" or similar)
- **Edge:** Very long description text; system truncates with "Read more" expandable option
- **Edge:** ISBN validation; system displays properly formatted ISBN (with hyphens if applicable)

---

### EPIC 3: Borrowing Logic

#### US-05: Physical Book Borrowing
**User Story:** As a student, I want to borrow a physical book so that I can use it for my studies.

**Acceptance Criteria:**
- System must decrement `StockCount` by 1 upon success
- A `Loan` record must be created with a default 14-day return window
- Users cannot borrow more than 5 books at once

**Test Cases:**
- **Positive:** User borrows available book; system decrements StockCount by 1; creates Loan record with DueDate = LoanDate + 14 days; user sees book in active loans
- **Positive:** User attempts to borrow 6th book when already has 5 active loans; system blocks action and displays "Maximum 5 active loans allowed" error
- **Positive:** User returns book; system increments StockCount by 1; updates Loan ReturnDate and Status to "Returned"
- **Negative:** User attempts to borrow book with StockCount = 0; system displays "Book currently out of stock" error; Loan not created
- **Negative:** User tries to borrow same book copy twice simultaneously; second attempt fails with "Book unavailable" message
- **Edge:** User borrows book near system date change; LoanDate and DueDate calculated correctly using server time
- **Edge:** System clock tested with timezone differences; all date calculations use UTC consistently

#### US-06: E-Book Access
**User Story:** As a student, I want to download/view E-books so that I can study digitally.

**Acceptance Criteria:**
- Only authorized users can access the PDF link
- The system tracks the number of downloads per book

**Test Cases:**
- **Positive:** Authorized user accesses e-book; system increments DownloadCount by 1; provides secure PDF access link
- **Positive:** Unauthorized user (not logged in) attempts to access e-book PDF; redirected to login page
- **Negative:** Student user attempts to access e-book PDF for book they haven't borrowed (if applicable); access denied based on library policy
- **Positive:** DownloadCount persists correctly across multiple downloads by same/different users
- **Edge:** Concurrent download requests; system handles race conditions correctly with atomic increment
- **Edge:** PDF file missing from server; system returns appropriate error (404) and logs incident
- **Edge:** Very large PDF file; system streams download efficiently without memory issues

---

### EPIC 4: Administration

#### US-07: Inventory Management (CRUD)
**User Story:** As an admin, I want to add new books to the system so that the library collection stays updated.

**Acceptance Criteria:**
- Admin can upload an image (cover) and a file (PDF)
- Admin can edit existing book details or delete obsolete items

**Test Cases:**
- **Positive:** Admin uploads valid book cover image (JPG/PNG <5MB); image stored and CoverImageUrl updated
- **Positive:** Admin uploads valid PDF file for e-book; file stored and PdfUrl updated; IsDigital set to true
- **Positive:** Admin creates new book with all required fields; book appears in catalog search results
- **Positive:** Admin edits existing book title/description; changes reflected immediately in catalog
- **Positive:** Admin deletes book with zero active loans; book removed from system; associated loans unaffected
- **Negative:** Admin attempts to delete book with active loans; system prevents deletion and displays "Cannot delete book with active loans"
- **Negative:** Admin uploads invalid file type (exe, etc.); system rejects with "Invalid file type" error
- **Negative:** Admin uploads file exceeding size limit; system displays "File too large" error
- **Edge:** Admin uploads cover image with special characters in filename; system sanitizes filename correctly
- **Edge:** Admin edits book to change IsDigital from false to true; system validates PDF upload requirement

#### US-08: Dashboard Analytics
**User Story:** As an admin, I want to see library statistics so that I can monitor system usage.

**Acceptance Criteria:**
- Display total active loans and most borrowed genres
- List users with overdue books in a dedicated table

**Test Cases:**
- **Positive:** Admin dashboard loads; shows accurate count of total active loans across all users
- **Positive:** Dashboard displays ranked list of most borrowed genres (based on Loan history)
- **Positive:** Overdue users table shows Student users with books past DueDate; includes book title, days overdue, user contact
- **Positive:** Dashboard statistics update in real-time (or near real-time) after borrowing/returning actions
- **Negative:** When no overdue books exist; overdue table shows "No overdue books" message
- **Negative:** When no loans exist; active loans count shows 0; genres table shows "No borrowing history"
- **Edge:** System handles large datasets efficiently (tested with 10k+ loans); dashboard loads within 3 seconds
- **Edge:** Date range filters on dashboard work correctly for analytics views (if implemented)

---

### EPIC 5: User Management

#### US-09: Borrowing History
**User Story:** As a student, I want to see my past loans so that I can keep track of my reading.

**Acceptance Criteria:**
- Table view showing Book Title, Date Borrowed, and Date Returned

**Test Cases:**
- **Positive:** User navigates to borrowing history; sees table with past loans including Book Title, LoanDate, ReturnDate
- **Positive:** Active loans (ReturnDate null) are excluded from history view (separate active loans tab exists)
- **Positive:** History table sorted by LoanDate descending (most recent first)
- **Positive:** User with no borrowing history sees "No borrowing history found" message
- **Negative:** User cannot see other users' borrowing history; data properly scoped to current user
- **Edge:** History table paginated correctly for users with extensive borrowing history (>20 loans)
- **Edge:** Date formatting consistent and localized based on user preferences (if applicable)
- **Edge:** ReturnDate correctly null for active loans; populated date for returned loans

#### US-10: Profile Customization
**User Story:** As a user, I want to update my profile picture and contact info.

**Acceptance Criteria:**
- Form validation for phone numbers and names

**Test Cases:**
- **Positive:** User updates profile picture; new image displayed immediately after upload
- **Positive:** User updates phone number with valid format; change saved and displayed correctly
- **Positive:** User updates FullName; change reflected in header/profile across application
- **Negative:** User submits invalid phone number (letters, wrong length); field shows validation error
- **Negative:** User submits empty FullName; form prevents submission with "Name is required" error
- **Negative:** User attempts to upload non-image file as profile picture; system rejects with "Invalid image file" error
- **Edge:** Profile picture upload respects size and dimension limits; provides feedback if exceeded
- **Edge:** User removes profile picture; system reverts to default avatar
- **Edge:** Concurrent profile updates handled correctly; last write wins with appropriate conflict resolution

---

## Technical Data Models (Schema)

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

## Implementation Status
Based on codebase analysis:

✅ **Fully Implemented:**
- User registration and login with JWT authentication
- Role-based access control (Admin/Client roles)
- Book CRUD operations (create, read, update, delete)
- Loan creation, return, and tracking
- User profile and borrowing history views
- Basic admin dashboard (though currently sales-focused)
- Form validations (email, password, required fields)

🔄 **Partially Implemented/Needs Alignment:**
- Admin dashboard shows sales metrics rather than library statistics (active loans, overdue books, genre popularity)
- E-book access tracking (DownloadCount) needs verification
- Book search/filtering exists but needs confirmation of case-insensitivity and dynamic updates
- Role terminology uses "Client" instead of "Student" in codebase

📝 **Not Found in Codebase:**
- Email confirmation upon registration
- Advanced password strength validation beyond length
- Account lockout after failed login attempts
- Notification system for task updates (not applicable to library system)
- Task sharing/collaboration features (not applicable to library system)

## Recommended Next Steps
1. Update Admin dashboard to display library-specific metrics (active loans, overdue books, popular genres)
2. Verify and enhance e-book access controls and download tracking
3. Confirm search functionality meets case-insensitive and dynamic filter requirements
4. Align role terminology between codebase (Client) and documentation (Student) as appropriate
5. Implement email verification for new registrations
6. Add password strength requirements (special characters, numbers, etc.)
7. Implement login attempt limiting and account lockout protection
8. Add borrowing history table view with detailed loan information