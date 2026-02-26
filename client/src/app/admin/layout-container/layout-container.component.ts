import { Component } from '@angular/core';
import { Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { Icons } from '../../shared/icons';
import { LucideAngularModule } from 'lucide-angular';
import { AuthService } from '../../shared/services/auth.service';

@Component({
  selector: 'app-layout-container',
  imports: [RouterOutlet, RouterLink, RouterLinkActive, LucideAngularModule],
  templateUrl: './layout-container.component.html',
  styleUrl: './layout-container.component.scss'
})
export class LayoutContainerComponent {
  readonly Icons = Icons;
  user: any = null;

  constructor(private authService: AuthService, private router: Router) { }

  ngOnInit(): void {
    this.user = this.authService.getUserInformation();
  }

  signOut(): void {
    this.authService.logout();
    this.router.navigate(['/authentication/login']);
  }

  get avatarSeed(): string {
    return this.user?.username ?? 'default';
  }
}
