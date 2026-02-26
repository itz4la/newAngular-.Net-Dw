import { Component, OnInit, OnDestroy } from '@angular/core';
import { NgApexchartsModule } from 'ng-apexcharts';
import { Subject, takeUntil } from 'rxjs';
import { AnalyticsService } from '../services/analytics.service';
import { CHART_COLORS, CHART_FONT_FAMILY, abbreviateNumber } from '../charts/chart.helpers';

@Component({
  selector: 'app-revenue-quarterly-chart',
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
export class RevenueQuarterlyChartComponent implements OnInit, OnDestroy {
  series: any[] = [];
  chart: any = { type: 'bar', height: 320, fontFamily: CHART_FONT_FAMILY, toolbar: { show: false }, background: 'transparent' };
  xaxis: any = {};
  yaxis: any = {};
  plotOptions: any = { bar: { columnWidth: '60%', borderRadius: 4 } };
  colors = CHART_COLORS;
  dataLabels: any = { enabled: false };
  tooltip: any = {};
  grid: any = { borderColor: '#e2e8f0', strokeDashArray: 4 };
  legend: any = { position: 'top', fontFamily: CHART_FONT_FAMILY, fontSize: '12px', labels: { colors: '#64748b' } };

  private destroy$ = new Subject<void>();

  constructor(private analytics: AnalyticsService) {}

  ngOnInit(): void {
    this.analytics.getQuarterlyRevenue().pipe(takeUntil(this.destroy$)).subscribe({
      next: (data) => {
        const quarters = ['Q1', 'Q2', 'Q3', 'Q4'];
        const years = [...new Set(data.map(d => d.year))].sort();

        this.xaxis = { categories: quarters, labels: { style: { colors: '#64748b', fontSize: '12px' } } };
        this.yaxis = { labels: { style: { colors: '#64748b', fontSize: '11px' }, formatter: (v: number) => '$' + abbreviateNumber(v) } };
        this.tooltip = { theme: 'light', y: { formatter: (v: number) => v ? '$' + v.toLocaleString() : 'N/A' } };

        this.series = years.map(year => ({
          name: year.toString(),
          data: quarters.map((_, i) => {
            const found = data.find(d => d.year === year && d.quarter === i + 1);
            return found ? found.totalRevenue : 0;
          }),
        }));
      },
    });
  }

  ngOnDestroy(): void { this.destroy$.next(); this.destroy$.complete(); }
}
