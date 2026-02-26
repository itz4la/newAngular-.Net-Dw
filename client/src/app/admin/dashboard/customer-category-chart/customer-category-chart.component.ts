import { Component, OnInit, OnDestroy } from '@angular/core';
import { NgApexchartsModule } from 'ng-apexcharts';
import { Subject, takeUntil } from 'rxjs';
import { AnalyticsService } from '../services/analytics.service';
import { CHART_COLORS, CHART_FONT_FAMILY } from '../charts/chart.helpers';

@Component({
  selector: 'app-customer-category-chart',
  standalone: true,
  imports: [NgApexchartsModule],
  template: `
    @if (series.length) {
      <apx-chart
        [series]="series"
        [chart]="chart"
        [labels]="labels"
        [colors]="colors"
        [legend]="legend"
        [plotOptions]="plotOptions"
        [dataLabels]="dataLabels"
        [tooltip]="tooltip"
        [responsive]="responsive"
        [stroke]="stroke"
      ></apx-chart>
    } @else {
      <div class="flex items-center justify-center h-64">
        <div class="w-8 h-8 border-2 border-indigo-500 border-t-transparent rounded-full animate-spin"></div>
      </div>
    }
  `,
  styles: [`:host { display: block; }`]
})
export class CustomerCategoryChartComponent implements OnInit, OnDestroy {
  series: number[] = [];
  labels: string[] = [];
  chart: any = { type: 'polarArea', height: 350, fontFamily: CHART_FONT_FAMILY, background: 'transparent' };
  colors = CHART_COLORS;
  legend: any = { position: 'bottom', fontFamily: CHART_FONT_FAMILY, fontSize: '12px', labels: { colors: '#64748b' } };
  plotOptions: any = { polarArea: { rings: { strokeWidth: 1, strokeColor: '#e2e8f0' }, spokes: { strokeWidth: 1, connectorColors: '#e2e8f0' } } };
  dataLabels: any = { enabled: true, style: { fontSize: '11px' }, dropShadow: { enabled: false } };
  tooltip: any = { theme: 'light', y: { formatter: (v: number) => '$' + v.toLocaleString() } };
  responsive: any[] = [{ breakpoint: 480, options: { chart: { height: 280 }, legend: { position: 'bottom' } } }];
  stroke: any = { colors: ['#fff'] };

  private destroy$ = new Subject<void>();

  constructor(private analytics: AnalyticsService) {}

  ngOnInit(): void {
    this.analytics.getCustomerCategories().pipe(takeUntil(this.destroy$)).subscribe({
      next: (data) => {
        const sorted = [...data].sort((a, b) => b.totalRevenue - a.totalRevenue);
        this.labels = sorted.map(d => d.customerCategoryName);
        this.series = sorted.map(d => d.totalRevenue);
      },
    });
  }

  ngOnDestroy(): void { this.destroy$.next(); this.destroy$.complete(); }
}
