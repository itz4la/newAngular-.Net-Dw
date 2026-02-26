import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule, DecimalPipe } from '@angular/common';
import { Subject, switchMap, takeUntil } from 'rxjs';
import { AnalyticsService } from '../services/analytics.service';
import { DateFilterService } from '../services/date-filter.service';
import { DashboardSummary } from '../models/analytics.models';

interface KpiCard {
  label: string;
  icon: string;
  bgClass: string;
  textClass: string;
  prefix: string;
  value: number;
  format: string;
}

@Component({
  selector: 'app-kpi-cards',
  standalone: true,
  imports: [CommonModule, DecimalPipe],
  template: `
    <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-6 gap-4">
      @if (loading) {
        @for (i of skeletons; track i) {
          <div class="glass-card p-5 rounded-2xl animate-pulse">
            <div class="w-10 h-10 bg-slate-200 rounded-xl mb-3"></div>
            <div class="w-20 h-3 bg-slate-200 rounded mb-2"></div>
            <div class="w-28 h-7 bg-slate-200 rounded"></div>
          </div>
        }
      } @else {
        @for (card of cards; track card.label) {
          <div class="glass-card p-5 rounded-2xl">
            <div class="flex items-center gap-3 mb-3">
              <div [class]="'p-2.5 rounded-xl ' + card.bgClass + ' ' + card.textClass">
                <svg class="w-5 h-5" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" viewBox="0 0 24 24">
                  @switch (card.icon) {
                    @case ('dollar-sign') { <line x1="12" y1="1" x2="12" y2="23"/><path d="M17 5H9.5a3.5 3.5 0 0 0 0 7h5a3.5 3.5 0 0 1 0 7H6"/> }
                    @case ('shopping-cart') { <circle cx="9" cy="21" r="1"/><circle cx="20" cy="21" r="1"/><path d="M1 1h4l2.68 13.39a2 2 0 0 0 2 1.61h9.72a2 2 0 0 0 2-1.61L23 6H6"/> }
                    @case ('package') { <line x1="16.5" y1="9.4" x2="7.5" y2="4.21"/><path d="M21 16V8a2 2 0 0 0-1-1.73l-7-4a2 2 0 0 0-2 0l-7 4A2 2 0 0 0 3 8v8a2 2 0 0 0 1 1.73l7 4a2 2 0 0 0 2 0l7-4A2 2 0 0 0 21 16z"/><polyline points="3.27 6.96 12 12.01 20.73 6.96"/><line x1="12" y1="22.08" x2="12" y2="12"/> }
                    @case ('trending-up') { <polyline points="23 6 13.5 15.5 8.5 10.5 1 18"/><polyline points="17 6 23 6 23 12"/> }
                    @case ('users') { <path d="M17 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2"/><circle cx="9" cy="7" r="4"/><path d="M23 21v-2a4 4 0 0 0-3-3.87"/><path d="M16 3.13a4 4 0 0 1 0 7.75"/> }
                    @case ('box') { <path d="M21 16V8a2 2 0 0 0-1-1.73l-7-4a2 2 0 0 0-2 0l-7 4A2 2 0 0 0 3 8v8a2 2 0 0 0 1 1.73l7 4a2 2 0 0 0 2 0l7-4A2 2 0 0 0 21 16z"/> }
                    @case ('briefcase') { <rect x="2" y="7" width="20" height="14" rx="2" ry="2"/><path d="M16 21V5a2 2 0 0 0-2-2h-4a2 2 0 0 0-2 2v16"/> }
                  }
                </svg>
              </div>
            </div>
            <p class="text-xs font-medium text-slate-500 uppercase tracking-wide">{{ card.label }}</p>
            <p class="text-2xl font-bold text-slate-800 mt-1">{{ card.prefix }}{{ card.value | number: card.format }}</p>
          </div>
        }
      }
    </div>
  `,
  styles: [`
    :host { display: block; }
    .glass-card {
      background: rgba(255,255,255,0.65);
      backdrop-filter: blur(12px);
      -webkit-backdrop-filter: blur(12px);
      border: 1px solid rgba(255,255,255,0.8);
      box-shadow: 0 4px 6px -1px rgba(0,0,0,0.05), 0 2px 4px -1px rgba(0,0,0,0.03);
      transition: transform 0.2s ease, box-shadow 0.2s ease;
    }
    .glass-card:hover {
      transform: translateY(-2px);
      box-shadow: 0 8px 25px -5px rgba(99,102,241,0.15);
    }
  `]
})
export class KpiCardsComponent implements OnInit, OnDestroy {
  loading = true;
  cards: KpiCard[] = [];
  skeletons = Array(6);

  private destroy$ = new Subject<void>();

  constructor(private analytics: AnalyticsService, private dateFilter: DateFilterService) {}

  ngOnInit(): void {
    this.dateFilter.dateFilter$.pipe(
      switchMap(filter => {
        this.loading = true;
        return this.analytics.getDashboardSummary(filter);
      }),
      takeUntil(this.destroy$),
    ).subscribe({
      next: (data) => this.buildCards(data),
      error: () => (this.loading = false),
    });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private buildCards(d: DashboardSummary): void {
    this.cards = [
      { label: 'Total Revenue', icon: 'dollar-sign', bgClass: 'bg-indigo-100/60', textClass: 'text-indigo-600', prefix: '$', value: d.totalRevenue, format: '1.0-0' },
      { label: 'Total Orders', icon: 'shopping-cart', bgClass: 'bg-blue-100/60', textClass: 'text-blue-600', prefix: '', value: d.totalOrders, format: '1.0-0' },
      { label: 'Units Sold', icon: 'package', bgClass: 'bg-emerald-100/60', textClass: 'text-emerald-600', prefix: '', value: d.totalUnitsSold, format: '1.0-0' },
      { label: 'Avg Order Value', icon: 'trending-up', bgClass: 'bg-amber-100/60', textClass: 'text-amber-600', prefix: '$', value: d.averageOrderValue, format: '1.2-2' },
      { label: 'Customers', icon: 'users', bgClass: 'bg-rose-100/60', textClass: 'text-rose-600', prefix: '', value: d.totalCustomers, format: '1.0-0' },
      { label: 'Products', icon: 'box', bgClass: 'bg-violet-100/60', textClass: 'text-violet-600', prefix: '', value: d.totalProducts, format: '1.0-0' },
    ];
    this.loading = false;
  }
}
