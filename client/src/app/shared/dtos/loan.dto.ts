export interface LoanDto {
  id: number;
  bookId: number;
  bookTitle: string;
  userId: string;
  userName: string;
  loanDate: string;
  dueDate: string;
  returnDate: string | null;
  status: string; // 'Active' | 'Returned' | 'Overdue'
  daysRemaining: number;
  isOverdue: boolean;
}

export interface CreateLoanDto {
  bookId: number;
  userId: string;
  customDueDate?: string;
}

export interface LoanFilterDto {
  userId?: string;
  bookId?: number;
  status?: string;
  isOverdue?: boolean;
  pageNumber: number;
  pageSize: number;
}
