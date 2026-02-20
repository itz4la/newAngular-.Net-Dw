import { Component } from '@angular/core';
import { Icons } from '../../shared/icons';
import { LucideAngularModule } from 'lucide-angular';
import { RequestsService } from '../../shared/services/requests.service';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ProductCardComponent } from '../component/product-card/product-card.component';

@Component({
  selector: 'app-browse',
  imports: [CommonModule, RouterModule, LucideAngularModule, ProductCardComponent],
  templateUrl: './browse.component.html',
  styleUrl: './browse.component.scss'
})
export class BrowseComponent {
  readonly Icons = Icons;

  open: boolean = false;
  data: any = [];
  errorMsg: any;

  constructor(private requestsService: RequestsService) { }


  ngOnInit(): void {
    this.initData();
  }


  initData() {
    this.open = false;

    this.requestsService.get(`/api/Book?PageNumber=1&PageSize=6`).subscribe({
      next: (res) => {
        this.data = res.items;
        console.log("🚀 ~ HomeComponent ~ initData ~ this.data:", this.data)
      },
      error: (err) => {
        this.errorMsg = err;
      },
      complete: () => {
        this.open = true;
      }
    });
  };

}
