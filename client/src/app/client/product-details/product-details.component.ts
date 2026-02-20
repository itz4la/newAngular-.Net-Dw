import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { Icons } from '../../shared/icons';
import { LucideAngularModule } from 'lucide-angular';
import { RequestsService } from '../../shared/services/requests.service';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { ProductCardComponent } from '../component/product-card/product-card.component';

@Component({
  selector: 'app-product-details',
  imports: [CommonModule, RouterModule, LucideAngularModule , ProductCardComponent],
  templateUrl: './product-details.component.html',
  styleUrl: './product-details.component.scss'
})
export class ProductDetailsComponent {
  readonly Icons = Icons;
  open: boolean = false
  data: any = [];
  relatedData: any = [];
  dataId: any;
  errorMsg: any;

  constructor(private requestsService: RequestsService, private activatedRoute: ActivatedRoute) { }


  ngOnInit(): void {
    this.activatedRoute.params.subscribe(params => {
      this.dataId = params['id'];
      this.initData();
    });
  }


  initData() {
    this.open = false;

    this.requestsService.get(`/api/Book/` + this.dataId).subscribe({
      next: (res) => {
        this.data = res;
        this.initRelatedData();
        console.log("🚀 ~ ProductDetailsComponent ~ initData ~ this.data:", this.data)
      },
      error: (err) => {
        this.errorMsg = err;
      },
      complete: () => {
        this.open = true;
      }
    });
  };

  initRelatedData() {
    this.requestsService.get(`/api/Book?PageNumber=1&PageSize=5&GenreId=` + this.data.genreId).subscribe({
      next: (res) => {
        this.relatedData = res.items;
        console.log("🚀 ~ ProductDetailsComponent ~ initRelatedData ~ this.relatedData:", this.relatedData)
      },
      error: (err) => {
        this.errorMsg = err;
      },
      complete: () => {
        this.open = true;
      }
    });
  }
}
