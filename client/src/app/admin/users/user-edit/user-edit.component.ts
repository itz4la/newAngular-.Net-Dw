import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormGroup, FormControl, Validators } from '@angular/forms';
import { Router, RouterLink, ActivatedRoute } from '@angular/router';
import { LucideAngularModule } from 'lucide-angular';
import { Icons } from '../../../shared/icons';
import { RequestsService } from '../../../shared/services/requests.service';
import { UserDto } from '../../../shared/dtos/user.dto';

@Component({
  selector: 'app-user-edit',
  imports: [CommonModule, ReactiveFormsModule, RouterLink, LucideAngularModule],
  templateUrl: './user-edit.component.html',
  styleUrl: './user-edit.component.scss'
})
export class UserEditComponent implements OnInit {
  readonly Icons = Icons;

  loadingUser = true;
  saving = false;
  errorMsg = '';

  form = new FormGroup({
    userName: new FormControl('', [Validators.required, Validators.maxLength(50)]),
    email:    new FormControl('', [Validators.required, Validators.email]),
    role:     new FormControl('', [Validators.required])
  });

  get f() { return this.form.controls; }

  private userId = '';

  constructor(
    private requests: RequestsService,
    private router: Router,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.userId = this.route.snapshot.paramMap.get('id') ?? '';
    this.requests.get(`/api/User/${this.userId}`).subscribe({
      next: (user: UserDto) => {
        this.form.patchValue({ userName: user.userName, email: user.email, role: user.role });
        this.loadingUser = false;
      },
      error: () => { this.loadingUser = false; }
    });
  }

  submit(): void {
    if (this.form.invalid) { this.form.markAllAsTouched(); return; }
    this.saving = true;
    this.errorMsg = '';

    this.requests.put(`/api/User/${this.userId}`, this.form.value).subscribe({
      next: () => { this.saving = false; this.router.navigate(['/admin/users']); },
      error: (err) => {
        this.saving = false;
        this.errorMsg = err?.error ?? 'Failed to update user.';
      }
    });
  }
}
