import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { LucideAngularModule } from 'lucide-angular';
import { Icons } from '../../shared/icons';
import { RequestsService } from '../../shared/services/requests.service';
import { GenreDto, GenreFilterDto } from '../../shared/dtos/genre.dto';
import { PagedResult } from '../../shared/dtos/paged-result.dto';
import { Subject, debounceTime, distinctUntilChanged } from 'rxjs';

@Component({
  selector: 'app-categories',
  imports: [CommonModule, FormsModule, RouterLink, LucideAngularModule],
  templateUrl: './categories.component.html',
  styleUrl: './categories.component.scss'
})
export class CategoriesComponent implements OnInit {
  readonly Icons = Icons;

  genres: GenreDto[] = [];
  totalCount = 0;
  pageNumber = 1;
  pageSize = 10;
  totalPages = 0;
  hasPreviousPage = false;
  hasNextPage = false;

  searchTerm = '';
  private searchSubject = new Subject<string>();

  loading = false;

  private readonly BASE = '/api/Genre';

  constructor(private requests: RequestsService, private router: Router) {}

  ngOnInit(): void {
    this.loadGenres();

    this.searchSubject
      .pipe(debounceTime(400), distinctUntilChanged())
      .subscribe((term) => {
        this.searchTerm = term;
        this.pageNumber = 1;
        this.loadGenres();
      });
  }

  loadGenres(): void {
    this.loading = true;
    const filter: GenreFilterDto = {
      pageNumber: this.pageNumber,
      pageSize: this.pageSize,
      name: this.searchTerm || undefined,
    };

    let params = `?PageNumber=${filter.pageNumber}&PageSize=${filter.pageSize}`;
    if (filter.name) {
      params += `&name=${encodeURIComponent(filter.name)}`;
    }
    this.requests.get(`${this.BASE}/paginated${params}`).subscribe({
      next: (result: PagedResult<GenreDto>) => {
        this.genres = result.items;
        this.totalCount = result.totalCount;
        this.pageNumber = result.pageNumber;
        this.totalPages = result.totalPages;
        this.hasPreviousPage = result.hasPreviousPage;
        this.hasNextPage = result.hasNextPage;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      },
    });
  }

  onSearchChange(value: string): void {
    this.searchSubject.next(value);
  }

  goToPage(page: number): void {
    if (page < 1 || page > this.totalPages) return;
    this.pageNumber = page;
    this.loadGenres();
  }

  nextPage(): void {
    if (this.hasNextPage) {
      this.pageNumber++;
      this.loadGenres();
    }
  }

  previousPage(): void {
    if (this.hasPreviousPage) {
      this.pageNumber--;
      this.loadGenres();
    }
  }

  editGenre(id: number): void {
    this.router.navigate(['/admin/categories/edit', id]);
  }

  get pages(): number[] {
    const pages: number[] = [];
    const start = Math.max(1, this.pageNumber - 2);
    const end = Math.min(this.totalPages, this.pageNumber + 2);
    for (let i = start; i <= end; i++) {
      pages.push(i);
    }
    return pages;
  }
}
