import { Component, OnInit, OnDestroy } from '@angular/core';
import { NgApexchartsModule } from 'ng-apexcharts';
import { Subject, takeUntil } from 'rxjs';
import { AnalyticsService } from '../services/analytics.service';
import { CHART_COLORS, CHART_FONT_FAMILY, abbreviateNumber } from '../charts/chart.helpers';

@Component({
  selector: 'app-brand-performance-chart',
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
        [legend]="legend"
      ></apx-chart>
    } @else {
      <div class="flex items-center justify-center h-64">
        <div class="w-8 h-8 border-2 border-indigo-500 border-t-transparent rounded-full animate-spin"></div>
      </div>
    }
  `,
  styles: [`:host { display: block; }`]
})
export class BrandPerformanceChartComponent implements OnInit, OnDestroy {
  series: any[] = [];
  chart: any = { type: 'bar', height: 350, fontFamily: CHART_FONT_FAMILY, toolbar: { show: false }, background: 'transparent', stacked: false };
  xaxis: any = {};
  yaxis: any[] = [];
  plotOptions: any = { bar: { columnWidth: '55%', borderRadius: 4 } };
  colors = [CHART_COLORS[0], CHART_COLORS[2]];
  dataLabels: any = { enabled: false };
  tooltip: any = {};
  grid: any = { borderColor: '#e2e8f0', strokeDashArray: 4 };
  legend: any = { position: 'top', fontFamily: CHART_FONT_FAMILY, fontSize: '12px', labels: { colors: '#64748b' } };

  private destroy$ = new Subject<void>();

  constructor(private analytics: AnalyticsService) {}

  ngOnInit(): void {
    this.analytics.getBrandPerformance().pipe(takeUntil(this.destroy$)).subscribe({
      next: (data) => {
        const sorted = [...data].sort((a, b) => b.totalRevenue - a.totalRevenue).slice(0, 12);
        this.xaxis = { categories: sorted.map(d => d.brand), labels: { style: { colors: '#64748b', fontSize: '11px' }, rotate: -45, trim: true, maxHeight: 80 } };
        this.yaxis = [
          { title: { text: 'Revenue ($)', style: { color: '#64748b', fontSize: '12px' } }, labels: { style: { colors: '#64748b', fontSize: '11px' }, formatter: (v: number) => '$' + abbreviateNumber(v) } },
          { opposite: true, title: { text: 'Units Sold', style: { color: '#64748b', fontSize: '12px' } }, labels: { style: { colors: '#64748b', fontSize: '11px' }, formatter: (v: number) => abbreviateNumber(v) } },
        ];
        this.tooltip = { theme: 'light' };
        this.series = [
          { name: 'Revenue', type: 'column', data: sorted.map(d => d.totalRevenue) },
          { name: 'Units Sold', type: 'column', data: sorted.map(d => d.totalUnitsSold) },
        ];
        this.chart = { ...this.chart, type: 'bar' };
      },
    });
  }

  ngOnDestroy(): void { this.destroy$.next(); this.destroy$.complete(); }
}
