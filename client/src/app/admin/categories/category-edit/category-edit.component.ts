import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormGroup, FormControl, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { LucideAngularModule } from 'lucide-angular';
import { Icons } from '../../../shared/icons';
import { RequestsService } from '../../../shared/services/requests.service';
import { UpdateGenreDto } from '../../../shared/dtos/genre.dto';

@Component({
  selector: 'app-category-edit',
  imports: [CommonModule, ReactiveFormsModule, RouterLink, LucideAngularModule],
  templateUrl: './category-edit.component.html',
  styleUrl: './category-edit.component.scss'
})
export class CategoryEditComponent implements OnInit {
  readonly Icons = Icons;

  genreId = 0;
  saving = false;
  loadingGenre = true;
  errorMessage = '';

  genreForm = new FormGroup({
    name: new FormControl('', [
      Validators.required,
      Validators.maxLength(50)
    ])
  });

  private readonly BASE = '/api/Genre';

  constructor(
    private requests: RequestsService,
    private router: Router,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.genreId = Number(this.route.snapshot.paramMap.get('id'));
    if (!this.genreId) {
      this.router.navigate(['/admin/categories']);
      return;
    }
    this.loadGenre();
  }

  loadGenre(): void {
    this.loadingGenre = true;
    this.requests.get(`${this.BASE}/${this.genreId}`).subscribe({
      next: (genre) => {
        this.genreForm.patchValue({ name: genre.name });
        this.loadingGenre = false;
      },
      error: () => {
        this.errorMessage = 'Failed to load category.';
        this.loadingGenre = false;
      }
    });
  }

  onSubmit(): void {
    if (this.genreForm.invalid) {
      this.genreForm.markAllAsTouched();
      return;
    }

    this.saving = true;
    this.errorMessage = '';

    this.requests.put(`${this.BASE}/${this.genreId}`, { name: this.genreForm.value.name! } as UpdateGenreDto).subscribe({
      next: () => {
        this.saving = false;
        this.router.navigate(['/admin/categories']);
      },
      error: (err) => {
        this.saving = false;
        this.errorMessage = err?.error || 'An error occurred while updating the category.';
      }
    });
  }

  get nameControl() {
    return this.genreForm.get('name')!;
  }
}
