import { CommonModule } from '@angular/common';
import { Component, Input, input } from '@angular/core';
import { RouterModule } from '@angular/router';
import { LucideAngularModule } from 'lucide-angular';
import Icons from '../../../shared/icons';

@Component({
  selector: 'app-product-card',
  imports: [CommonModule, RouterModule, LucideAngularModule],
  templateUrl: './product-card.component.html',
  styleUrl: './product-card.component.scss'
})
export class ProductCardComponent {
   readonly Icons = Icons;
  @Input() item: any;
  @Input() isShowActions: boolean = false;
}
