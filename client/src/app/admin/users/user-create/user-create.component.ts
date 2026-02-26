import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormGroup, FormControl, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { LucideAngularModule } from 'lucide-angular';
import { Icons } from '../../../shared/icons';
import { RequestsService } from '../../../shared/services/requests.service';

@Component({
  selector: 'app-user-create',
  imports: [CommonModule, ReactiveFormsModule, RouterLink, LucideAngularModule],
  templateUrl: './user-create.component.html',
  styleUrl: './user-create.component.scss'
})
export class UserCreateComponent {
  readonly Icons = Icons;

  saving = false;
  errorMsg = '';

  form = new FormGroup({
    userName: new FormControl('', [Validators.required, Validators.maxLength(50)]),
    email:    new FormControl('', [Validators.required, Validators.email]),
    password: new FormControl('', [Validators.required, Validators.minLength(6)])
  });

  get f() { return this.form.controls; }

  constructor(private requests: RequestsService, private router: Router) {}

  submit(): void {
    if (this.form.invalid) { this.form.markAllAsTouched(); return; }
    this.saving = true;
    this.errorMsg = '';

    this.requests.post('/api/User/admin', this.form.value).subscribe({
      next: () => { this.saving = false; this.router.navigate(['/admin/users']); },
      error: (err) => {
        this.saving = false;
        this.errorMsg = err?.error ?? 'Failed to create user.';
      }
    });
  }
}
