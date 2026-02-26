import { Component, OnInit, OnDestroy } from '@angular/core';
import { NgApexchartsModule } from 'ng-apexcharts';
import { Subject, switchMap, takeUntil } from 'rxjs';
import { AnalyticsService } from '../services/analytics.service';
import { DateFilterService } from '../services/date-filter.service';
import { CHART_COLORS, CHART_FONT_FAMILY } from '../charts/chart.helpers';

@Component({
  selector: 'app-sales-day-chart',
  standalone: true,
  imports: [NgApexchartsModule],
  template: `
    @if (series.length) {
      <apx-chart
        [series]="series"
        [chart]="chart"
        [xaxis]="xaxis"
        [yaxis]="yaxis"
        [plotOptions]="plotOptions"
        [colors]="colors"
        [dataLabels]="dataLabels"
        [tooltip]="tooltip"
        [grid]="grid"
        [fill]="fill"
      ></apx-chart>
    } @else {
      <div class="flex items-center justify-center h-64">
        <div class="w-8 h-8 border-2 border-indigo-500 border-t-transparent rounded-full animate-spin"></div>
      </div>
    }
  `,
  styles: [`:host { display: block; }`]
})
export class SalesDayChartComponent implements OnInit, OnDestroy {
  series: any[] = [];
  chart: any = { type: 'bar', height: 320, fontFamily: CHART_FONT_FAMILY, toolbar: { show: false }, background: 'transparent' };
  xaxis: any = {};
  yaxis: any = {};
  plotOptions: any = { bar: { columnWidth: '55%', borderRadius: 6, distributed: true } };
  colors = CHART_COLORS;
  dataLabels: any = { enabled: false };
  tooltip: any = {};
  grid: any = { borderColor: '#e2e8f0', strokeDashArray: 4 };
  fill: any = { opacity: 0.9 };

  private destroy$ = new Subject<void>();
  private dayOrder = ['Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday', 'Sunday'];

  constructor(private analytics: AnalyticsService, private dateFilter: DateFilterService) {}

  ngOnInit(): void {
    this.dateFilter.dateFilter$.pipe(
      switchMap(filter => {
        this.series = [];
        return this.analytics.getSalesByDayOfWeek(filter);
      }),
      takeUntil(this.destroy$),
    ).subscribe({
      next: (data) => {
        const sorted = [...data].sort((a, b) => this.dayOrder.indexOf(a.dayOfWeek) - this.dayOrder.indexOf(b.dayOfWeek));
        this.xaxis = { categories: sorted.map(d => d.dayOfWeek.substring(0, 3)), labels: { style: { colors: '#64748b', fontSize: '12px' } } };
        this.yaxis = { labels: { style: { colors: '#64748b', fontSize: '11px' }, formatter: (v: number) => '$' + (v / 1000).toFixed(0) + 'K' } };
        this.tooltip = { theme: 'light', y: { formatter: (v: number) => '$' + v.toLocaleString() } };
        this.series = [{ name: 'Revenue', data: sorted.map(d => d.totalRevenue) }];
      },
    });
  }

  ngOnDestroy(): void { this.destroy$.next(); this.destroy$.complete(); }
}
