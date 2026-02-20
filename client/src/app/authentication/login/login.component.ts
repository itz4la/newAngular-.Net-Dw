import { Component } from '@angular/core';
import { Icons } from '../../shared/icons';
import { LucideAngularModule } from 'lucide-angular';
import { Router, RouterModule } from '@angular/router';
import { FormGroup, FormControl, Validators, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { UsernameValidaor } from '../../shared/validators/username.validator';
import { emailValidator } from '../../shared/validators/email.validators';
import { AuthService } from '../../shared/services/auth.service';
import { RequestsService } from '../../shared/services/requests.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-login',
  imports: [LucideAngularModule, RouterModule, CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent {
  readonly Icons = Icons;
  errorMsg: any;
  open: boolean = false;

  public loginForm = new FormGroup({
    username: new FormControl('', [
      Validators.required,
      emailValidator(),
    ]),
    password: new FormControl('', [
      Validators.required,
      Validators.minLength(8),
    ]),
  });


  constructor(
    private requestsService: RequestsService,
    private authService: AuthService,
    private router: Router
  ) { }


  onSignin() {
    this.requestsService.post('/api/User/login', this.loginForm.value).subscribe({
      next: (res: any) => {
        this.authService.signin(res.token);
        const user = this.authService.getUserInformation();
        const role = user?.role?.toLowerCase();
        this.router.navigateByUrl('/' + (role ?? 'authentication/login'));
        this.open = false;
      },
      error: (err: any) => {
        if (err && err.error) {
          this.errorMsg = err.error === 'Invalid username or password' ? 'Invalid email or password' : err.error;
        } else {
          this.errorMsg = 'Quelque chose ne va pas, réessayez !';
        }
        this.open = true;
        setTimeout(() => {
          this.errorMsg = null;
          this.open = false;
        }, 5000);
      },
    });
  }

  getPasswordErrorMessage() {
    if (this.loginForm.get('password')?.hasError('required')) {
      return 'Ce champ est obligatoire.';
    }
    if (this.loginForm.get('password')?.hasError('minlength')) {
      return 'Ce champ contient au minimum 8 caractères.';
    }
    return '';
  }

  getUsernameErroressage() {
    if (this.loginForm.get('username')?.hasError('required')) {
      return 'Ce champ est obligatoire.';
    }
    if (this.loginForm.get('username')?.hasError('invalidEmail')) {
      return "Email invalide.";
    }
    return '';
  }

}
