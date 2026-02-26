import { Component, OnInit, OnDestroy } from '@angular/core';
import { NgApexchartsModule } from 'ng-apexcharts';
import { Subject, switchMap, takeUntil } from 'rxjs';
import { AnalyticsService } from '../services/analytics.service';
import { DateFilterService } from '../services/date-filter.service';
import { CHART_COLORS, CHART_FONT_FAMILY, abbreviateNumber } from '../charts/chart.helpers';

@Component({
  selector: 'app-top-products-chart',
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
      ></apx-chart>
    } @else {
      <div class="flex items-center justify-center h-64">
        <div class="w-8 h-8 border-2 border-indigo-500 border-t-transparent rounded-full animate-spin"></div>
      </div>
    }
  `,
  styles: [`:host { display: block; }`]
})
export class TopProductsChartComponent implements OnInit, OnDestroy {
  series: any[] = [];
  chart: any = { type: 'bar', height: 380, fontFamily: CHART_FONT_FAMILY, toolbar: { show: false }, background: 'transparent' };
  xaxis: any = {};
  yaxis: any = {};
  plotOptions: any = { bar: { horizontal: true, barHeight: '60%', borderRadius: 4 } };
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
        return this.analytics.getTopProducts(10, filter);
      }),
      takeUntil(this.destroy$),
    ).subscribe({
      next: (data) => {
        const sorted = [...data].sort((a, b) => a.totalUnitsSold - b.totalUnitsSold);
        this.xaxis = { labels: { style: { colors: '#64748b', fontSize: '11px' }, formatter: (v: number) => abbreviateNumber(v) } };
        this.yaxis = { labels: { style: { colors: '#64748b', fontSize: '11px' }, maxWidth: 180 } };
        this.tooltip = { theme: 'light', y: { formatter: (v: number) => v.toLocaleString() + ' units' } };
        this.series = [{ name: 'Units Sold', data: sorted.map(d => ({ x: d.stockItemName.length > 35 ? d.stockItemName.substring(0, 35) + '…' : d.stockItemName, y: d.totalUnitsSold })) }];
      },
    });
  }

  ngOnDestroy(): void { this.destroy$.next(); this.destroy$.complete(); }
}
