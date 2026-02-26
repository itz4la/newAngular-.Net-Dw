import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormGroup, FormControl, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { LucideAngularModule } from 'lucide-angular';
import { Icons } from '../../../shared/icons';
import { RequestsService } from '../../../shared/services/requests.service';
import { CreateBookDto } from '../../../shared/dtos/book.dto';
import { GenreDto } from '../../../shared/dtos/genre.dto';

@Component({
  selector: 'app-product-create',
  imports: [CommonModule, ReactiveFormsModule, RouterLink, LucideAngularModule],
  templateUrl: './product-create.component.html',
  styleUrl: './product-create.component.scss'
})
export class ProductCreateComponent implements OnInit {
  readonly Icons = Icons;

  saving = false;
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

  constructor(private requests: RequestsService, private router: Router) {}

  ngOnInit(): void {
    this.requests.get(this.GENRE_BASE).subscribe({
      next: (genres: GenreDto[]) => { this.genres = genres; },
      error: () => {}
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

    const dto: CreateBookDto = {
      title: this.f['title'].value!,
      author: this.f['author'].value!,
      description: this.f['description'].value || undefined,
      genreId: this.f['genreId'].value!,
      coverImageUrl: this.f['coverImageUrl'].value || undefined,
      publishedDate: this.f['publishedDate'].value!
    };

    this.requests.post(this.BASE, dto).subscribe({
      next: () => { this.saving = false; this.router.navigate(['/admin/products']); },
      error: (err) => {
        this.saving = false;
        this.errorMessage = err?.error || 'An error occurred while creating the book.';
      }
    });
  }
}
