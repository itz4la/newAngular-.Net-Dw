import { Component, OnInit } from '@angular/core';
import { LucideAngularModule } from 'lucide-angular';
import { Icons } from '../../shared/icons';
import { RequestsService } from '../../shared/services/requests.service';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ProductCardComponent } from '../component/product-card/product-card.component';

@Component({
  selector: 'app-home',
  imports: [CommonModule, RouterModule, LucideAngularModule, ProductCardComponent],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss'
})
export class HomeComponent implements OnInit {
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

    this.requestsService.get(`/api/Book?PageNumber=1&PageSize=10`).subscribe({
      next: (res) => {
        this.data = res.items;
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
