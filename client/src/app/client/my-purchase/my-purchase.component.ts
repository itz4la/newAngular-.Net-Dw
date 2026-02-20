import { Component } from '@angular/core';
import { LucideAngularModule } from 'lucide-angular';
import { Icons } from '../../shared/icons';

@Component({
  selector: 'app-my-purchase',
  imports: [LucideAngularModule],
  templateUrl: './my-purchase.component.html',
  styleUrl: './my-purchase.component.scss'
})
export class MyPurchaseComponent {
  readonly Icons = Icons;

}
