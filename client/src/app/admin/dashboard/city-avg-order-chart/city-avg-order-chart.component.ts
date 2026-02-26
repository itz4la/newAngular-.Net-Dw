import { Component, OnInit, OnDestroy } from '@angular/core';
import { NgApexchartsModule } from 'ng-apexcharts';
import { Subject, takeUntil } from 'rxjs';
import { AnalyticsService } from '../services/analytics.service';
import { CHART_COLORS, CHART_FONT_FAMILY, abbreviateNumber } from '../charts/chart.helpers';

@Component({
  selector: 'app-city-avg-order-chart',
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
export class CityAvgOrderChartComponent implements OnInit, OnDestroy {
  series: any[] = [];
  chart: any = { type: 'bar', height: 400, fontFamily: CHART_FONT_FAMILY, toolbar: { show: false }, background: 'transparent' };
  xaxis: any = {};
  yaxis: any = {};
  plotOptions: any = { bar: { horizontal: true, barHeight: '55%', borderRadius: 4 } };
  colors = [CHART_COLORS[3]];
  dataLabels: any = { enabled: false };
  tooltip: any = {};
  grid: any = { borderColor: '#e2e8f0', strokeDashArray: 4 };

  private destroy$ = new Subject<void>();

  constructor(private analytics: AnalyticsService) {}

  ngOnInit(): void {
    this.analytics.getCityAvgOrder().pipe(takeUntil(this.destroy$)).subscribe({
      next: (data) => {
        const top15 = [...data].sort((a, b) => b.averageOrderValue - a.averageOrderValue).slice(0, 15).reverse();
        this.xaxis = { labels: { style: { colors: '#64748b', fontSize: '11px' }, formatter: (v: number) => '$' + abbreviateNumber(v) } };
        this.yaxis = { labels: { style: { colors: '#64748b', fontSize: '11px' }, maxWidth: 150 } };
        this.tooltip = {
          theme: 'light',
          y: { formatter: (v: number) => '$' + v.toFixed(2) },
        };
        this.series = [{
          name: 'Avg Order Value',
          data: top15.map(d => ({ x: d.cityName, y: d.averageOrderValue })),
        }];
      },
    });
  }

  ngOnDestroy(): void { this.destroy$.next(); this.destroy$.complete(); }
}
