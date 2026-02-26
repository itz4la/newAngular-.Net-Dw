import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormGroup, FormControl, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { LucideAngularModule } from 'lucide-angular';
import { Icons } from '../../../shared/icons';
import { RequestsService } from '../../../shared/services/requests.service';
import { CreateGenreDto } from '../../../shared/dtos/genre.dto';

@Component({
  selector: 'app-category-create',
  imports: [CommonModule, ReactiveFormsModule, RouterLink, LucideAngularModule],
  templateUrl: './category-create.component.html',
  styleUrl: './category-create.component.scss'
})
export class CategoryCreateComponent {
  readonly Icons = Icons;

  saving = false;
  errorMessage = '';

  genreForm = new FormGroup({
    name: new FormControl('', [
      Validators.required,
      Validators.maxLength(50)
    ])
  });

  private readonly BASE = '/api/Genre';

  constructor(private requests: RequestsService, private router: Router) {}

  onSubmit(): void {
    if (this.genreForm.invalid) {
      this.genreForm.markAllAsTouched();
      return;
    }

    this.saving = true;
    this.errorMessage = '';

    this.requests.post(this.BASE, { name: this.genreForm.value.name! } as CreateGenreDto).subscribe({
      next: () => {
        this.saving = false;
        this.router.navigate(['/admin/categories']);
      },
      error: (err) => {
        this.saving = false;
        this.errorMessage = err?.error || 'An error occurred while creating the category.';
      }
    });
  }

  get nameControl() {
    return this.genreForm.get('name')!;
  }
}
