import { AuthService } from './../../shared/services/auth.service';
import { Component } from '@angular/core';
import { LucideAngularModule } from 'lucide-angular';
import { Icons } from '../../shared/icons';
import { RequestsService } from '../../shared/services/requests.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-my-purchase',
  imports: [LucideAngularModule, CommonModule],
  templateUrl: './my-purchase.component.html',
  styleUrl: './my-purchase.component.scss'
})
export class MyPurchaseComponent {
  readonly Icons = Icons;

  open: boolean = false;
  loans: any[] = [];
  errorMsg: any;

  activeTab: 'active' | 'history' = 'active';

  userId: string = '';

  // Pagination
  currentPage: number = 1;
  pageSize: number = 10;
  totalCount: number = 0;
  totalPages: number = 0;
  hasPreviousPage: boolean = false;
  hasNextPage: boolean = false;

  constructor(private authService: AuthService, private requestsService: RequestsService) { }

  ngOnInit(): void {
    const user = this.authService.getUserInformation();
    if (user?.userId) {
      this.userId = user.userId;
      this.initData();
    }
  }

  initData() {
    this.open = false;
    const url = `/api/Loan?UserId=${this.userId}&PageNumber=${this.currentPage}&PageSize=${this.pageSize}`;
    this.requestsService.get(url).subscribe({
      next: (res) => {
        this.loans = res.items;
        this.totalCount = res.totalCount;
        this.totalPages = res.totalPages;
        this.hasPreviousPage = res.hasPreviousPage;
        this.hasNextPage = res.hasNextPage;
        this.currentPage = res.pageNumber;
      },
      error: (err) => { this.errorMsg = err; },
      complete: () => { this.open = true; }
    });
  }

  get activeLoans() {
    return this.loans.filter(l => l.returnDate === null);
  }

  get historyLoans() {
    return this.loans.filter(l => l.returnDate !== null);
  }

  get overdueCount() {
    return this.loans.filter(l => l.isOverdue).length;
  }

  get displayedLoans() {
    return this.activeTab === 'active' ? this.activeLoans : this.historyLoans;
  }

  getBorderColor(loan: any): string {
    if (loan.status === 'Returned') return 'bg-stone-300';
    if (loan.isOverdue) return 'bg-red-500';
    if (loan.daysRemaining <= 3) return 'bg-orange-400';
    return 'bg-green-500';
  }

  getBadgeClass(loan: any): string {
    if (loan.status === 'Returned') return 'bg-stone-100 text-stone-600';
    if (loan.isOverdue) return 'bg-red-100 text-red-700';
    if (loan.daysRemaining <= 3) return 'bg-orange-100 text-orange-700';
    return 'bg-green-100 text-green-700';
  }

  getStatusLabel(loan: any): string {
    if (loan.status === 'Returned') return 'Returned';
    if (loan.isOverdue) return 'Overdue';
    if (loan.daysRemaining <= 3) return 'Due Soon';
    return 'On Track';
  }

  getDueDateText(loan: any): string {
    if (loan.status === 'Returned') {
      const date = new Date(loan.returnDate);
      return `Returned on ${date.toLocaleDateString('en-US', { month: 'short', day: 'numeric', year: 'numeric' })}`;
    }
    const dueDate = new Date(loan.dueDate);
    const formatted = dueDate.toLocaleDateString('en-US', { month: 'short', day: 'numeric' });
    if (loan.isOverdue) {
      const days = Math.abs(loan.daysRemaining);
      return `Due ${days} day${days > 1 ? 's' : ''} ago (${formatted})`;
    }
    if (loan.daysRemaining === 0) return `Due today (${formatted})`;
    return `Due in ${loan.daysRemaining} day${loan.daysRemaining > 1 ? 's' : ''} (${formatted})`;
  }

  getDueDateClass(loan: any): string {
    if (loan.status === 'Returned') return 'text-stone-500';
    if (loan.isOverdue) return 'text-red-600';
    if (loan.daysRemaining <= 3) return 'text-orange-600';
    return 'text-stone-500';
  }

  getDueDateIcon(loan: any) {
    if (loan.status === 'Returned') return this.Icons.CheckCircle2;
    if (loan.isOverdue) return this.Icons.AlertCircle;
    if (loan.daysRemaining <= 3) return this.Icons.Clock;
    return this.Icons.Calendar;
  }

  goToPage(page: number) {
    if (page < 1 || page > this.totalPages || page === this.currentPage) return;
    this.currentPage = page;
    this.initData();
  }

  getPageNumbers(): (number | '...')[] {
    const pages: (number | '...')[] = [];
    const total = this.totalPages;
    const current = this.currentPage;
    if (total <= 7) {
      for (let i = 1; i <= total; i++) pages.push(i);
      return pages;
    }
    pages.push(1);
    if (current > 4) pages.push('...');
    const start = Math.max(2, current - 1);
    const end = Math.min(total - 1, current + 1);
    for (let i = start; i <= end; i++) pages.push(i);
    if (current < total - 3) pages.push('...');
    pages.push(total);
    return pages;
  }
}
