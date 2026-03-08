import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { LucideAngularModule } from 'lucide-angular';
import { Subject, debounceTime, distinctUntilChanged } from 'rxjs';
import { PagedResult } from '../../shared/dtos/paged-result.dto';
import { UserDto } from '../../shared/dtos/user.dto';
import { Icons } from '../../shared/icons';
import { RequestsService } from '../../shared/services/requests.service';

@Component({
  selector: 'app-users',
  imports: [CommonModule, FormsModule, RouterLink, LucideAngularModule],
  templateUrl: './users.component.html',
  styleUrl: './users.component.scss'
})

export class UsersComponent implements OnInit {
  readonly Icons = Icons;

  users: UserDto[] = [];
  totalCount = 0;
  pageNumber = 1;
  pageSize = 10;
  totalPages = 0;
  hasPreviousPage = false;
  hasNextPage = false;

  filterUserName = '';
  filterRole = '';
  showFilters = false;

  private userNameSubject = new Subject<string>();

  loading = false;
  deleteModalOpen = false;
  userToDeleteId: string | null = null;

  private readonly BASE = '/api/User';

  constructor(private requests: RequestsService, private router: Router) {}

  ngOnInit(): void {
    this.loadUsers();
    this.userNameSubject.pipe(debounceTime(400), distinctUntilChanged()).subscribe((val) => {
      this.filterUserName = val;
      this.pageNumber = 1;
      this.loadUsers();
    });
  }

  loadUsers(): void {
    this.loading = true;
    let params = `?PageNumber=${this.pageNumber}&PageSize=${this.pageSize}`;
    if (this.filterUserName) params += `&UserName=${encodeURIComponent(this.filterUserName)}`;
    if (this.filterRole) params += `&Role=${encodeURIComponent(this.filterRole)}`;

    this.requests.get(`${this.BASE}${params}`).subscribe({
      next: (result: PagedResult<UserDto>) => {
        this.users = result.items;
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

  onRoleChange(): void {
    this.pageNumber = 1;
    this.loadUsers();
  }

  clearFilters(): void {
    this.filterUserName = '';
    this.filterRole = '';
    this.pageNumber = 1;
    this.loadUsers();
  }

  editUser(id: string): void {
    this.router.navigate(['/admin/users/edit', id]);
  }

  openDeleteModal(id: string): void {
    this.userToDeleteId = id;
    this.deleteModalOpen = true;
  }

  cancelDelete(): void {
    this.deleteModalOpen = false;
    this.userToDeleteId = null;
  }

  confirmDelete(): void {
    if (!this.userToDeleteId) return;
    this.requests.delete(`${this.BASE}/${this.userToDeleteId}`).subscribe({
      next: () => {
        this.deleteModalOpen = false;
        this.userToDeleteId = null;
        this.loadUsers();
      },
      error: () => {}
    });
  }

  goToPage(page: number): void {
    if (page < 1 || page > this.totalPages) return;
    this.pageNumber = page;
    this.loadUsers();
  }

  nextPage(): void {
    if (this.hasNextPage) { this.pageNumber++; this.loadUsers(); }
  }

  previousPage(): void {
    if (this.hasPreviousPage) { this.pageNumber--; this.loadUsers(); }
  }

  get pages(): number[] {
    const range: number[] = [];
    const start = Math.max(1, this.pageNumber - 2);
    const end = Math.min(this.totalPages, this.pageNumber + 2);
    for (let i = start; i <= end; i++) range.push(i);
    return range;
  }

  roleClass(role: string): string {
    switch (role?.toLowerCase()) {
      case 'admin':  return 'bg-indigo-100 text-indigo-700';
      case 'client': return 'bg-emerald-100 text-emerald-700';
      default:       return 'bg-slate-100 text-slate-500';
    }
  }

  avatarUrl(userName: string): string {
    return `https://ui-avatars.com/api/?name=${encodeURIComponent(userName)}&background=6366f1&color=fff&size=64`;
  }
}
