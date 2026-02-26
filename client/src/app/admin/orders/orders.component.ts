import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { LucideAngularModule } from 'lucide-angular';
import { Icons } from '../../shared/icons';
import { RequestsService } from '../../shared/services/requests.service';
import { LoanDto } from '../../shared/dtos/loan.dto';
import { PagedResult } from '../../shared/dtos/paged-result.dto';
import { Subject, debounceTime, distinctUntilChanged } from 'rxjs';

@Component({
  selector: 'app-orders',
  imports: [CommonModule, FormsModule, RouterLink, LucideAngularModule],
  templateUrl: './orders.component.html',
  styleUrl: './orders.component.scss'
})
export class OrdersComponent implements OnInit {
  readonly Icons = Icons;

  loans: LoanDto[] = [];
  totalCount = 0;
  pageNumber = 1;
  pageSize = 10;
  totalPages = 0;
  hasPreviousPage = false;
  hasNextPage = false;

  filterUserName = '';
  filterStatus = '';
  showFilters = false;

  private userNameSubject = new Subject<string>();

  loading = false;

  returnModalOpen = false;
  loanToReturnId: number | null = null;
  returnDate = '';
  returning = false;

  private readonly BASE = '/api/Loan';

  constructor(private requests: RequestsService, private router: Router) {}

  ngOnInit(): void {
    this.loadLoans();

    this.userNameSubject.pipe(debounceTime(400), distinctUntilChanged()).subscribe((val) => {
      this.filterUserName = val;
      this.pageNumber = 1;
      this.loadLoans();
    });
  }

  loadLoans(): void {
    this.loading = true;
    let params = `?PageNumber=${this.pageNumber}&PageSize=${this.pageSize}`;
    if (this.filterUserName) params += `&UserName=${encodeURIComponent(this.filterUserName)}`;
    if (this.filterStatus) params += `&Status=${encodeURIComponent(this.filterStatus)}`;

    this.requests.get(`${this.BASE}${params}`).subscribe({
      next: (result: PagedResult<LoanDto>) => {
        this.loans = result.items;
        this.totalCount = result.totalCount;
        this.pageNumber = result.pageNumber;
        this.totalPages = result.totalPages;
        this.hasPreviousPage = result.hasPreviousPage;
        this.hasNextPage = result.hasNextPage;
        this.loading = false;
      },
      error: () => { this.loading = false; }
    });
  }

  onUserNameChange(value: string): void { this.userNameSubject.next(value); }

  onStatusChange(): void {
    this.pageNumber = 1;
    this.loadLoans();
  }

  clearFilters(): void {
    this.filterUserName = '';
    this.filterStatus = '';
    this.pageNumber = 1;
    this.loadLoans();
  }

  openReturnModal(loanId: number): void {
    this.loanToReturnId = loanId;
    this.returnDate = '';
    this.returnModalOpen = true;
  }

  cancelReturn(): void {
    this.returnModalOpen = false;
    this.loanToReturnId = null;
    this.returnDate = '';
  }

  confirmReturn(): void {
    if (!this.loanToReturnId) return;
    this.returning = true;
    const body: any = { loanId: this.loanToReturnId };
    if (this.returnDate) body['returnDate'] = this.returnDate;
    this.requests.post(`${this.BASE}/return/${this.loanToReturnId}`, body).subscribe({
      next: () => {
        this.returning = false;
        this.returnModalOpen = false;
        this.loanToReturnId = null;
        this.returnDate = '';
        this.loadLoans();
      },
      error: () => { this.returning = false; }
    });
  }

  goToPage(page: number): void {
    if (page < 1 || page > this.totalPages) return;
    this.pageNumber = page;
    this.loadLoans();
  }

  nextPage(): void {
    if (this.hasNextPage) { this.pageNumber++; this.loadLoans(); }
  }

  previousPage(): void {
    if (this.hasPreviousPage) { this.pageNumber--; this.loadLoans(); }
  }

  get pages(): number[] {
    const range: number[] = [];
    const start = Math.max(1, this.pageNumber - 2);
    const end = Math.min(this.totalPages, this.pageNumber + 2);
    for (let i = start; i <= end; i++) range.push(i);
    return range;
  }

  statusClass(status: string): string {
    switch (status?.toLowerCase()) {
      case 'active':   return 'bg-blue-100 text-blue-600';
      case 'returned': return 'bg-emerald-100 text-emerald-600';
      case 'overdue':  return 'bg-rose-100 text-rose-600';
      default:         return 'bg-slate-100 text-slate-500';
    }
  }
}
