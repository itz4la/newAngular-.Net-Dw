import { Component, OnInit, OnDestroy } from '@angular/core';
import { NgApexchartsModule } from 'ng-apexcharts';
import { Subject, switchMap, takeUntil } from 'rxjs';
import { AnalyticsService } from '../services/analytics.service';
import { DateFilterService } from '../services/date-filter.service';
import { CHART_FONT_FAMILY } from '../charts/chart.helpers';

/** Maps common colour names to actual hex values for the pie slices. */
const COLOR_MAP: Record<string, string> = {
  'Black': '#1e293b', 'White': '#e2e8f0', 'Red': '#ef4444', 'Blue': '#3b82f6',
  'Green': '#22c55e', 'Yellow': '#eab308', 'Gray': '#6b7280', 'Steel Gray': '#71717a',
  'Light Brown': '#a3765e', 'Dark Brown': '#6d4c30',
};

@Component({
  selector: 'app-color-revenue-chart',
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
        [dataLabels]="dataLabels"
        [tooltip]="tooltip"
        [plotOptions]="plotOptions"
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
export class ColorRevenueChartComponent implements OnInit, OnDestroy {
  series: number[] = [];
  labels: string[] = [];
  chart: any = { type: 'pie', height: 340, fontFamily: CHART_FONT_FAMILY, background: 'transparent' };
  colors: string[] = [];
  legend: any = { position: 'bottom', fontFamily: CHART_FONT_FAMILY, fontSize: '12px', labels: { colors: '#64748b' } };
  dataLabels: any = { enabled: true, style: { fontSize: '11px' }, dropShadow: { enabled: false } };
  tooltip: any = { theme: 'light', y: { formatter: (v: number) => '$' + v.toLocaleString() } };
  plotOptions: any = {};
  responsive: any[] = [{ breakpoint: 480, options: { chart: { height: 280 }, legend: { position: 'bottom' } } }];

  private destroy$ = new Subject<void>();

  constructor(private analytics: AnalyticsService, private dateFilter: DateFilterService) {}

  ngOnInit(): void {
    this.dateFilter.dateFilter$.pipe(
      switchMap(filter => {
        this.series = [];
        return this.analytics.getRevenueByColor(filter);
      }),
      takeUntil(this.destroy$),
    ).subscribe({
      next: (data) => {
        const sorted = [...data].sort((a, b) => b.totalRevenue - a.totalRevenue);
        this.labels = sorted.map(d => d.colorName);
        this.series = sorted.map(d => d.totalRevenue);
        this.colors = sorted.map(d => COLOR_MAP[d.colorName] || '#' + Math.floor(Math.random() * 16777215).toString(16).padStart(6, '0'));
      },
    });
  }

  ngOnDestroy(): void { this.destroy$.next(); this.destroy$.complete(); }
}
