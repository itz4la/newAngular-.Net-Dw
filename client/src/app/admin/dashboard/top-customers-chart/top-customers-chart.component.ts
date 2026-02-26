import { Component, OnInit, OnDestroy } from '@angular/core';
import { NgApexchartsModule } from 'ng-apexcharts';
import { Subject, takeUntil } from 'rxjs';
import { AnalyticsService } from '../services/analytics.service';
import { CHART_COLORS, CHART_FONT_FAMILY, abbreviateNumber } from '../charts/chart.helpers';

@Component({
  selector: 'app-top-customers-chart',
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
export class TopCustomersChartComponent implements OnInit, OnDestroy {
  series: any[] = [];
  chart: any = { type: 'bar', height: 320, fontFamily: CHART_FONT_FAMILY, toolbar: { show: false }, background: 'transparent' };
  xaxis: any = {};
  yaxis: any = {};
  plotOptions: any = { bar: { horizontal: true, barHeight: '60%', borderRadius: 4, distributed: true } };
  colors = CHART_COLORS;
  dataLabels: any = { enabled: false };
  tooltip: any = {};
  grid: any = { borderColor: '#e2e8f0', strokeDashArray: 4 };

  private destroy$ = new Subject<void>();

  constructor(private analytics: AnalyticsService) {}

  ngOnInit(): void {
    this.analytics.getTopCustomers(10).pipe(takeUntil(this.destroy$)).subscribe({
      next: (data) => {
        const sorted = [...data].sort((a, b) => a.lifetimeValue - b.lifetimeValue);
        this.xaxis = { labels: { style: { colors: '#64748b', fontSize: '11px' }, formatter: (v: number) => '$' + abbreviateNumber(v) } };
        this.yaxis = { labels: { style: { colors: '#64748b', fontSize: '11px' }, maxWidth: 160 } };
        this.tooltip = {
          theme: 'light',
          y: { formatter: (v: number) => '$' + v.toLocaleString() },
          custom: ({ series, seriesIndex, dataPointIndex, w }: any) => {
            const customer = sorted[dataPointIndex];
            return `<div class="p-2 text-xs"><b>${w.globals.labels[dataPointIndex]}</b><br/>Category: ${customer?.customerCategoryName || ''}<br/>Lifetime Value: $${series[seriesIndex][dataPointIndex].toLocaleString()}</div>`;
          },
        };
        this.series = [{
          name: 'Lifetime Value',
          data: sorted.map(d => ({
            x: d.customerName.length > 30 ? d.customerName.substring(0, 30) + '…' : d.customerName,
            y: d.lifetimeValue,
          })),
        }];
      },
    });
  }

  ngOnDestroy(): void { this.destroy$.next(); this.destroy$.complete(); }
}
