import { Component, OnInit, OnDestroy } from '@angular/core';
import { NgApexchartsModule } from 'ng-apexcharts';
import { Subject, switchMap, takeUntil } from 'rxjs';
import { AnalyticsService } from '../services/analytics.service';
import { DateFilterService } from '../services/date-filter.service';
import { CHART_FONT_FAMILY } from '../charts/chart.helpers';

@Component({
  selector: 'app-revenue-state-chart',
  standalone: true,
  imports: [NgApexchartsModule],
  template: `
    @if (series.length) {
      <apx-chart
        [series]="series"
        [chart]="chart"
        [colors]="colors"
        [dataLabels]="dataLabels"
        [legend]="legend"
        [plotOptions]="plotOptions"
        [tooltip]="tooltip"
      ></apx-chart>
    } @else {
      <div class="flex items-center justify-center h-64">
        <div class="w-8 h-8 border-2 border-indigo-500 border-t-transparent rounded-full animate-spin"></div>
      </div>
    }
  `,
  styles: [`:host { display: block; }`]
})
export class RevenueStateChartComponent implements OnInit, OnDestroy {
  series: any[] = [];
  chart: any = { type: 'treemap', height: 350, fontFamily: CHART_FONT_FAMILY, toolbar: { show: false }, background: 'transparent' };
  colors = ['#6366f1'];
  dataLabels: any = { enabled: true, style: { fontSize: '12px', fontWeight: 600 }, offsetY: -4 };
  legend: any = { show: false };
  plotOptions: any = {
    treemap: {
      enableShades: true,
      shadeIntensity: 0.5,
      colorScale: {
        ranges: [
          { from: 0, to: 500000, color: '#c7d2fe' },
          { from: 500001, to: 2000000, color: '#818cf8' },
          { from: 2000001, to: 10000000, color: '#6366f1' },
          { from: 10000001, to: 100000000, color: '#4338ca' },
        ],
      },
    },
  };
  tooltip: any = { theme: 'light', y: { formatter: (v: number) => '$' + v.toLocaleString() } };

  private destroy$ = new Subject<void>();

  constructor(private analytics: AnalyticsService, private dateFilter: DateFilterService) {}

  ngOnInit(): void {
    this.dateFilter.dateFilter$.pipe(
      switchMap(filter => {
        this.series = [];
        return this.analytics.getRevenueByState(filter);
      }),
      takeUntil(this.destroy$),
    ).subscribe({
      next: (data) => {
        const top20 = [...data].sort((a, b) => b.totalRevenue - a.totalRevenue).slice(0, 20);
        this.series = [{
          data: top20.map(d => ({ x: d.stateProvinceName, y: d.totalRevenue })),
        }];
      },
    });
  }

  ngOnDestroy(): void { this.destroy$.next(); this.destroy$.complete(); }
}
