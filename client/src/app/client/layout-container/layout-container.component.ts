import { Component } from '@angular/core';
import { RouterLink, RouterOutlet, RouterLinkActive } from '@angular/router';
import { Icons } from '../../shared/icons';
import { LucideAngularModule } from 'lucide-angular';

@Component({
  selector: 'app-layout-container',
  imports: [RouterOutlet, RouterLink, RouterLinkActive, LucideAngularModule],
  templateUrl: './layout-container.component.html',
  styleUrl: './layout-container.component.scss'
})
export class LayoutContainerComponent {
  readonly Icons = Icons;

}
