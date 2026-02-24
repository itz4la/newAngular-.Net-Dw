import { Component, OnDestroy } from '@angular/core';
import { Icons } from '../../shared/icons';
import { LucideAngularModule } from 'lucide-angular';
import { RequestsService } from '../../shared/services/requests.service';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ProductCardComponent } from '../component/product-card/product-card.component';
import { Subject, debounceTime, distinctUntilChanged, takeUntil } from 'rxjs';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-browse',
  imports: [CommonModule, RouterModule, LucideAngularModule, ProductCardComponent, FormsModule],
  templateUrl: './browse.component.html',
  styleUrl: './browse.component.scss'
})
export class BrowseComponent implements OnDestroy {
  readonly Icons = Icons;

  open: boolean = false;
  data: any = [];
  genre: any = [];
  errorMsg: any;

  titleFilter: string = '';
  selectedGenreId: number | null = null;

  // Pagination
  currentPage: number = 1;
  pageSize: number = 6;
  totalCount: number = 0;
  totalPages: number = 0;
  hasPreviousPage: boolean = false;
  hasNextPage: boolean = false;

  private titleSearch$ = new Subject<string>();
  private destroy$ = new Subject<void>();

  constructor(private requestsService: RequestsService) { }


  ngOnInit(): void {
    this.initGenre();
    this.initData();

    this.titleSearch$.pipe(
      debounceTime(400),
      distinctUntilChanged(),
      takeUntil(this.destroy$)
    ).subscribe(() => {
      this.currentPage = 1;
      this.initData();
    });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  initData() {
    this.open = false;

    let url = `/api/Book?PageNumber=${this.currentPage}&PageSize=${this.pageSize}`;
    if (this.titleFilter.trim()) {
      url += `&Title=${encodeURIComponent(this.titleFilter.trim())}`;
    }
    if (this.selectedGenreId !== null) {
      url += `&GenreId=${this.selectedGenreId}`;
    }

    this.requestsService.get(url).subscribe({
      next: (res) => {
        this.data = res.items;
        this.totalCount = res.totalCount;
        this.totalPages = res.totalPages;
        this.hasPreviousPage = res.hasPreviousPage;
        this.hasNextPage = res.hasNextPage;
        this.currentPage = res.pageNumber;
        console.log('🚀 ~ BrowseComponent ~ initData ~ this.data:', this.data);
      },
      error: (err) => {
        this.errorMsg = err;
      },
      complete: () => {
        this.open = true;
      }
    });
  };

  initGenre() {
    this.requestsService.get(`/api/Genre`).subscribe({
      next: (res) => {
        this.genre = res;
        console.log('🚀 ~ BrowseComponent ~ initGenre ~ this.genre:', this.genre);
      },
      error: (err) => {
        this.errorMsg = err;
      }
    });
  };

  onTitleInput(value: string): void {
    this.titleFilter = value;
    this.titleSearch$.next(value);
  }

  onGenreSelect(id: number): void {
    this.selectedGenreId = this.selectedGenreId === id ? null : id;
    this.currentPage = 1;
    this.initData();
  }

  goToPage(page: number): void {
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

  get showingFrom(): number {
    return this.totalCount === 0 ? 0 : (this.currentPage - 1) * this.pageSize + 1;
  }

  get showingTo(): number {
    return Math.min(this.currentPage * this.pageSize, this.totalCount);
  }


}
