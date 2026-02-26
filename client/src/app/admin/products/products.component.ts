import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { LucideAngularModule } from 'lucide-angular';
import { Icons } from '../../shared/icons';
import { RequestsService } from '../../shared/services/requests.service';
import { BookDto } from '../../shared/dtos/book.dto';
import { GenreDto } from '../../shared/dtos/genre.dto';
import { PagedResult } from '../../shared/dtos/paged-result.dto';
import { Subject, debounceTime, distinctUntilChanged } from 'rxjs';

@Component({
  selector: 'app-products',
  imports: [CommonModule, FormsModule, RouterLink, LucideAngularModule],
  templateUrl: './products.component.html',
  styleUrl: './products.component.scss'
})
export class ProductsComponent implements OnInit {
  readonly Icons = Icons;

  books: BookDto[] = [];
  genres: GenreDto[] = [];
  totalCount = 0;
  pageNumber = 1;
  pageSize = 10;
  totalPages = 0;
  hasPreviousPage = false;
  hasNextPage = false;

  filterTitle = '';
  filterAuthor = '';
  filterGenreId: number | null = null;
  showFilters = false;

  private titleSubject = new Subject<string>();
  private authorSubject = new Subject<string>();

  loading = false;
  deleteModalOpen = false;
  bookToDeleteId: number | null = null;

  private readonly BASE = '/api/Book';
  private readonly GENRE_BASE = '/api/Genre';

  constructor(private requests: RequestsService, private router: Router) {}

  ngOnInit(): void {
    this.loadBooks();
    this.loadGenres();

    this.titleSubject.pipe(debounceTime(400), distinctUntilChanged()).subscribe((val) => {
      this.filterTitle = val;
      this.pageNumber = 1;
      this.loadBooks();
    });

    this.authorSubject.pipe(debounceTime(400), distinctUntilChanged()).subscribe((val) => {
      this.filterAuthor = val;
      this.pageNumber = 1;
      this.loadBooks();
    });
  }

  loadBooks(): void {
    this.loading = true;
    let params = `?PageNumber=${this.pageNumber}&PageSize=${this.pageSize}`;
    if (this.filterTitle) params += `&Title=${encodeURIComponent(this.filterTitle)}`;
    if (this.filterAuthor) params += `&Author=${encodeURIComponent(this.filterAuthor)}`;
    if (this.filterGenreId) params += `&GenreId=${this.filterGenreId}`;

    this.requests.get(`${this.BASE}${params}`).subscribe({
      next: (result: PagedResult<BookDto>) => {
        this.books = result.items;
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

  loadGenres(): void {
    this.requests.get(this.GENRE_BASE).subscribe({
      next: (genres: GenreDto[]) => { this.genres = genres; },
      error: () => {}
    });
  }

  onTitleChange(value: string): void { this.titleSubject.next(value); }
  onAuthorChange(value: string): void { this.authorSubject.next(value); }

  onGenreChange(): void {
    this.pageNumber = 1;
    this.loadBooks();
  }

  clearFilters(): void {
    this.filterTitle = '';
    this.filterAuthor = '';
    this.filterGenreId = null;
    this.pageNumber = 1;
    this.loadBooks();
  }

  goToPage(page: number): void {
    if (page < 1 || page > this.totalPages) return;
    this.pageNumber = page;
    this.loadBooks();
  }

  nextPage(): void {
    if (this.hasNextPage) { this.pageNumber++; this.loadBooks(); }
  }

  previousPage(): void {
    if (this.hasPreviousPage) { this.pageNumber--; this.loadBooks(); }
  }

  editBook(id: number): void {
    this.router.navigate(['/admin/products/edit', id]);
  }

  openDeleteModal(id: number): void {
    this.bookToDeleteId = id;
    this.deleteModalOpen = true;
  }

  cancelDelete(): void {
    this.deleteModalOpen = false;
    this.bookToDeleteId = null;
  }

  confirmDelete(): void {
    if (!this.bookToDeleteId) return;
    this.requests.delete(`${this.BASE}/${this.bookToDeleteId}`).subscribe({
      next: () => {
        this.deleteModalOpen = false;
        this.bookToDeleteId = null;
        this.loadBooks();
      },
      error: () => { this.deleteModalOpen = false; }
    });
  }

  get pages(): number[] {
    const pages: number[] = [];
    const start = Math.max(1, this.pageNumber - 2);
    const end = Math.min(this.totalPages, this.pageNumber + 2);
    for (let i = start; i <= end; i++) pages.push(i);
    return pages;
  }
}
