import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormGroup, FormControl, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { LucideAngularModule } from 'lucide-angular';
import { Icons } from '../../../shared/icons';
import { RequestsService } from '../../../shared/services/requests.service';
import { UpdateBookDto } from '../../../shared/dtos/book.dto';
import { GenreDto } from '../../../shared/dtos/genre.dto';

@Component({
  selector: 'app-product-edit',
  imports: [CommonModule, ReactiveFormsModule, RouterLink, LucideAngularModule],
  templateUrl: './product-edit.component.html',
  styleUrl: './product-edit.component.scss'
})
export class ProductEditComponent implements OnInit {
  readonly Icons = Icons;

  bookId = 0;
  saving = false;
  loadingBook = true;
  errorMessage = '';
  genres: GenreDto[] = [];

  private readonly BASE = '/api/Book';
  private readonly GENRE_BASE = '/api/Genre';

  bookForm = new FormGroup({
    title: new FormControl('', [Validators.required, Validators.maxLength(150)]),
    author: new FormControl('', [Validators.required, Validators.maxLength(100)]),
    description: new FormControl('', [Validators.maxLength(5000)]),
    genreId: new FormControl<number | null>(null, [Validators.required]),
    coverImageUrl: new FormControl('', [Validators.maxLength(255)]),
    publishedDate: new FormControl('', [Validators.required])
  });

  constructor(
    private requests: RequestsService,
    private router: Router,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.bookId = Number(this.route.snapshot.paramMap.get('id'));
    if (!this.bookId) {
      this.router.navigate(['/admin/products']);
      return;
    }
    this.loadGenres();
    this.loadBook();
  }

  loadGenres(): void {
    this.requests.get(this.GENRE_BASE).subscribe({
      next: (genres: GenreDto[]) => { this.genres = genres; },
      error: () => {}
    });
  }

  loadBook(): void {
    this.loadingBook = true;
    this.requests.get(`${this.BASE}/${this.bookId}`).subscribe({
      next: (book) => {
        this.bookForm.patchValue({
          title: book.title,
          author: book.author,
          description: book.description || '',
          genreId: book.genreId,
          coverImageUrl: book.coverImageUrl || '',
          publishedDate: book.publishedDate ? book.publishedDate.substring(0, 10) : ''
        });
        this.loadingBook = false;
      },
      error: () => {
        this.errorMessage = 'Failed to load book.';
        this.loadingBook = false;
      }
    });
  }

  get f() { return this.bookForm.controls; }

  onSubmit(): void {
    if (this.bookForm.invalid) {
      this.bookForm.markAllAsTouched();
      return;
    }

    this.saving = true;
    this.errorMessage = '';

    const dto: UpdateBookDto = {
      title: this.f['title'].value!,
      author: this.f['author'].value!,
      description: this.f['description'].value || undefined,
      genreId: this.f['genreId'].value!,
      coverImageUrl: this.f['coverImageUrl'].value || undefined,
      publishedDate: this.f['publishedDate'].value!
    };

    this.requests.put(`${this.BASE}/${this.bookId}`, dto).subscribe({
      next: () => { this.saving = false; this.router.navigate(['/admin/products']); },
      error: (err) => {
        this.saving = false;
        this.errorMessage = err?.error || 'An error occurred while updating the book.';
      }
    });
  }
}
