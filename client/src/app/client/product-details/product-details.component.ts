import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { Icons } from '../../shared/icons';
import { LucideAngularModule } from 'lucide-angular';
import { RequestsService } from '../../shared/services/requests.service';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { ProductCardComponent } from '../component/product-card/product-card.component';
import { AuthService } from '../../shared/services/auth.service';

@Component({
  selector: 'app-product-details',
  imports: [CommonModule, RouterModule, LucideAngularModule, ProductCardComponent],
  templateUrl: './product-details.component.html',
  styleUrl: './product-details.component.scss'
})
export class ProductDetailsComponent {
  readonly Icons = Icons;
  open: boolean = false;
  data: any = [];
  relatedData: any = [];
  dataId: any;
  errorMsg: any;

  loanLoading: boolean = false;
  loanSuccess: boolean = false;
  loanError: string | null = null;

  constructor(
    private requestsService: RequestsService,
    private activatedRoute: ActivatedRoute,
    private authService: AuthService
  ) { }


  ngOnInit(): void {
    this.activatedRoute.params.subscribe(params => {
      this.dataId = params['id'];
      this.loanSuccess = false;
      this.loanError = null;
      this.initData();
    });
  }


  initData() {
    this.open = false;

    this.requestsService.get(`/api/Book/` + this.dataId).subscribe({
      next: (res) => {
        this.data = res;
        this.initRelatedData();
        console.log('🚀 ~ ProductDetailsComponent ~ initData ~ this.data:', this.data);
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
        console.log('🚀 ~ ProductDetailsComponent ~ initRelatedData ~ this.relatedData:', this.relatedData);
      },
      error: (err) => {
        this.errorMsg = err;
      },
      complete: () => {
        this.open = true;
      }
    });
  }

  loanBook() {
    const user = this.authService.getUserInformation();
    if (!user?.userId) return;

    const dueDate = new Date();
    dueDate.setDate(dueDate.getDate() + 15);

    const body = {
      bookId: this.data.id,
      userId: user.userId,
      customDueDate: dueDate.toISOString()
    };

    this.loanLoading = true;
    this.loanSuccess = false;
    this.loanError = null;

    this.requestsService.post(`/api/Loan`, body).subscribe({
      next: () => {
        this.loanSuccess = true;
      },
      error: (err) => {
        this.loanError = err?.error?.message ?? 'Failed to borrow book. Please try again.';
      },
      complete: () => {
        this.loanLoading = false;
      }
    });
  }

  get loanDueDate(): Date {
    const d = new Date();
    d.setDate(d.getDate() + 15);
    return d;
  }
}
