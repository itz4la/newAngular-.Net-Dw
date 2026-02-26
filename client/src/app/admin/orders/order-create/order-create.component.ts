import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormGroup, FormControl, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { LucideAngularModule } from 'lucide-angular';
import { Icons } from '../../../shared/icons';
import { RequestsService } from '../../../shared/services/requests.service';
import { BookDto } from '../../../shared/dtos/book.dto';
import { UserDto } from '../../../shared/dtos/user.dto';

@Component({
  selector: 'app-order-create',
  imports: [CommonModule, ReactiveFormsModule, RouterLink, LucideAngularModule],
  templateUrl: './order-create.component.html',
  styleUrl: './order-create.component.scss'
})
export class OrderCreateComponent implements OnInit {
  readonly Icons = Icons;

  availableBooks: BookDto[] = [];
  nonAdminUsers: UserDto[] = [];
  loadingBooks = false;
  loadingUsers = false;
  saving = false;
  errorMsg = '';

  form = new FormGroup({
    bookId: new FormControl<number | null>(null, [Validators.required]),
    userId: new FormControl<string>('', [Validators.required]),
    customDueDate: new FormControl('')
  });

  get f() { return this.form.controls; }

  constructor(private requests: RequestsService, private router: Router) {}

  ngOnInit(): void {
    this.loadAvailableBooks();
    this.loadNonAdminUsers();
  }

  loadAvailableBooks(): void {
    this.loadingBooks = true;
    this.requests.get('/api/Book/available').subscribe({
      next: (books: BookDto[]) => {
        this.availableBooks = books;
        this.loadingBooks = false;
      },
      error: () => { this.loadingBooks = false; }
    });
  }

  loadNonAdminUsers(): void {
    this.loadingUsers = true;
    this.requests.get('/api/User/non-admin').subscribe({
      next: (users: UserDto[]) => {
        this.nonAdminUsers = users;
        this.loadingUsers = false;
      },
      error: () => { this.loadingUsers = false; }
    });
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.saving = true;
    this.errorMsg = '';

    const payload: any = {
      bookId: this.f.bookId.value,
      userId: this.f.userId.value
    };
    if (this.f.customDueDate.value) {
      payload.customDueDate = this.f.customDueDate.value;
    }

    this.requests.post('/api/Loan', payload).subscribe({
      next: () => {
        this.saving = false;
        this.router.navigate(['/admin/orders']);
      },
      error: (err) => {
        this.saving = false;
        this.errorMsg = err?.error?.message ?? err?.error ?? 'Failed to create loan.';
      }
    });
  }
}
