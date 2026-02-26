import { Component, OnInit, OnDestroy } from '@angular/core';
import { NgApexchartsModule } from 'ng-apexcharts';
import { Subject, switchMap, takeUntil } from 'rxjs';
import { AnalyticsService } from '../services/analytics.service';
import { DateFilterService } from '../services/date-filter.service';
import { CHART_COLORS, CHART_FONT_FAMILY, abbreviateNumber } from '../charts/chart.helpers';

@Component({
  selector: 'app-revenue-yearly-chart',
  standalone: true,
  imports: [NgApexchartsModule],
  template: `
    @if (series.length) {
      <apx-chart
        [series]="series"
        [chart]="chart"
        [xaxis]="xaxis"
        [yaxis]="yaxis"
        [stroke]="stroke"
        [fill]="fill"
        [colors]="colors"
        [dataLabels]="dataLabels"
        [tooltip]="tooltip"
        [grid]="grid"
      ></apx-chart>
    } @else {
      <div class="flex items-center justify-center h-64">
        <div class="w-8 h-8 border-2 border-indigo-500 border-t-transparent rounded-full animate-spin"></div>
      </div>
    }
  `,
  styles: [`:host { display: block; }`]
})
export class RevenueYearlyChartComponent implements OnInit, OnDestroy {
  series: any[] = [];
  chart: any = { type: 'area', height: 320, fontFamily: CHART_FONT_FAMILY, toolbar: { show: false }, background: 'transparent' };
  xaxis: any = {};
  yaxis: any = {};
  stroke: any = { curve: 'smooth', width: 3 };
  fill: any = { type: 'gradient', gradient: { shadeIntensity: 1, opacityFrom: 0.45, opacityTo: 0.05, stops: [0, 100] } };
  colors = [CHART_COLORS[0]];
  dataLabels: any = { enabled: false };
  tooltip: any = {};
  grid: any = { borderColor: '#e2e8f0', strokeDashArray: 4 };

  private destroy$ = new Subject<void>();

  constructor(private analytics: AnalyticsService, private dateFilter: DateFilterService) {}

  ngOnInit(): void {
    this.dateFilter.dateFilter$.pipe(
      switchMap(filter => {
        this.series = [];
        return this.analytics.getRevenueByYear(filter);
      }),
      takeUntil(this.destroy$),
    ).subscribe({
      next: (data) => {
        const sorted = [...data].sort((a, b) => a.year - b.year);
        this.xaxis = { categories: sorted.map(d => d.year.toString()), labels: { style: { colors: '#64748b', fontSize: '12px' } } };
        this.yaxis = { labels: { style: { colors: '#64748b', fontSize: '12px' }, formatter: (v: number) => '$' + abbreviateNumber(v) } };
        this.tooltip = {
          theme: 'light',
          y: { formatter: (v: number) => '$' + v.toLocaleString() },
        };
        this.series = [{ name: 'Revenue', data: sorted.map(d => d.totalRevenue) }];
      },
    });
  }

  ngOnDestroy(): void { this.destroy$.next(); this.destroy$.complete(); }
}
