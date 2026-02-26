import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormsModule, FormGroup, FormControl, Validators } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { LucideAngularModule } from 'lucide-angular';
import { Icons } from '../../shared/icons';
import { RequestsService } from '../../shared/services/requests.service';

@Component({
  selector: 'app-register',
  imports: [CommonModule, LucideAngularModule, RouterModule, FormsModule, ReactiveFormsModule],
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss'
})
export class RegisterComponent {
  readonly Icons = Icons;

  errorMsg = '';
  successMsg = '';
  loading = false;

  registerForm = new FormGroup({
    username: new FormControl('', [Validators.required, Validators.maxLength(50)]),
    email:    new FormControl('', [Validators.required, Validators.email]),
    password: new FormControl('', [Validators.required, Validators.minLength(6)]),
  });

  get f() { return this.registerForm.controls; }

  constructor(private requests: RequestsService, private router: Router) {}

  onRegister(): void {
    this.errorMsg = '';
    this.successMsg = '';
    if (this.registerForm.invalid) { this.registerForm.markAllAsTouched(); return; }
    this.loading = true;
    this.requests.post('/api/User/register', this.registerForm.value).subscribe({
      next: () => {
        this.loading = false;
        this.successMsg = 'Account created successfully! Redirecting to login...';
        setTimeout(() => this.router.navigate(['/authentication/login']), 1800);
      },
      error: (err: any) => {
        this.loading = false;
        this.errorMsg = err?.error.message ?? 'Registration failed. Please try again.';
      }
    });
  }
}
