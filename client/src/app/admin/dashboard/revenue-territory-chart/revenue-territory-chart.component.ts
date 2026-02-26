import { Component, OnInit, OnDestroy } from '@angular/core';
import { NgApexchartsModule } from 'ng-apexcharts';
import { Subject, switchMap, takeUntil } from 'rxjs';
import { AnalyticsService } from '../services/analytics.service';
import { DateFilterService } from '../services/date-filter.service';
import { CHART_COLORS, CHART_FONT_FAMILY } from '../charts/chart.helpers';

@Component({
  selector: 'app-revenue-territory-chart',
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
      ></apx-chart>
    } @else {
      <div class="flex items-center justify-center h-64">
        <div class="w-8 h-8 border-2 border-indigo-500 border-t-transparent rounded-full animate-spin"></div>
      </div>
    }
  `,
  styles: [`:host { display: block; }`]
})
export class RevenueTerritoryChartComponent implements OnInit, OnDestroy {
  series: number[] = [];
  labels: string[] = [];
  chart: any = { type: 'donut', height: 350, fontFamily: CHART_FONT_FAMILY, background: 'transparent' };
  colors = CHART_COLORS;
  legend: any = { position: 'bottom', fontFamily: CHART_FONT_FAMILY, fontSize: '12px', labels: { colors: '#64748b' } };
  plotOptions: any = { pie: { donut: { size: '55%', labels: { show: true, name: { fontSize: '14px' }, value: { fontSize: '18px', formatter: (v: string) => '$' + Number(v).toLocaleString() }, total: { show: true, label: 'Total', fontSize: '13px', formatter: (w: any) => '$' + w.globals.seriesTotals.reduce((a: number, b: number) => a + b, 0).toLocaleString() } } } } };
  dataLabels: any = { enabled: false };
  tooltip: any = { theme: 'light', y: { formatter: (v: number) => '$' + v.toLocaleString() } };
  responsive: any[] = [{ breakpoint: 480, options: { chart: { height: 300 }, legend: { position: 'bottom' } } }];

  private destroy$ = new Subject<void>();

  constructor(private analytics: AnalyticsService, private dateFilter: DateFilterService) {}

  ngOnInit(): void {
    this.dateFilter.dateFilter$.pipe(
      switchMap(filter => {
        this.series = [];
        return this.analytics.getRevenueByTerritory(filter);
      }),
      takeUntil(this.destroy$),
    ).subscribe({
      next: (data) => {
        const sorted = [...data].sort((a, b) => b.totalRevenue - a.totalRevenue);
        this.labels = sorted.map(d => d.salesTerritory);
        this.series = sorted.map(d => d.totalRevenue);
      },
    });
  }

  ngOnDestroy(): void { this.destroy$.next(); this.destroy$.complete(); }
}
