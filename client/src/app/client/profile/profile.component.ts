import { Component } from '@angular/core';
import { Icons } from '../../shared/icons';
import { LucideAngularModule } from 'lucide-angular';

@Component({
  selector: 'app-profile',
  imports: [LucideAngularModule],
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.scss'
})
export class ProfileComponent {
  readonly Icons = Icons;

}
