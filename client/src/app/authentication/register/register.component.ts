import { Component } from '@angular/core';
import { Icons } from '../../shared/icons';
import { LucideAngularModule } from 'lucide-angular';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-register',
  imports: [LucideAngularModule , RouterModule],
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss'
})
export class RegisterComponent {
  readonly Icons = Icons;

}
