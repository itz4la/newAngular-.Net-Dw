import { Component } from '@angular/core';
import { KpiCardsComponent } from './kpi-cards/kpi-cards.component';
import { RevenueYearlyChartComponent } from './revenue-yearly-chart/revenue-yearly-chart.component';
import { RevenueMonthlyChartComponent } from './revenue-monthly-chart/revenue-monthly-chart.component';
import { RevenueQuarterlyChartComponent } from './revenue-quarterly-chart/revenue-quarterly-chart.component';
import { RevenueTerritoryChartComponent } from './revenue-territory-chart/revenue-territory-chart.component';
import { RevenueStateChartComponent } from './revenue-state-chart/revenue-state-chart.component';
import { TopProductsChartComponent } from './top-products-chart/top-products-chart.component';
import { BrandPerformanceChartComponent } from './brand-performance-chart/brand-performance-chart.component';
import { ColorRevenueChartComponent } from './color-revenue-chart/color-revenue-chart.component';
import { SalespersonChartComponent } from './salesperson-chart/salesperson-chart.component';
import { CustomerCategoryChartComponent } from './customer-category-chart/customer-category-chart.component';
import { TopCustomersChartComponent } from './top-customers-chart/top-customers-chart.component';
import { SalesDayChartComponent } from './sales-day-chart/sales-day-chart.component';
import { CityAvgOrderChartComponent } from './city-avg-order-chart/city-avg-order-chart.component';
import { DateFilterComponent } from './date-filter/date-filter.component';

@Component({
  selector: 'app-dashboard',
  imports: [
    DateFilterComponent,
    KpiCardsComponent,
    RevenueYearlyChartComponent,
    RevenueMonthlyChartComponent,
    RevenueQuarterlyChartComponent,
    RevenueTerritoryChartComponent,
    RevenueStateChartComponent,
    TopProductsChartComponent,
    BrandPerformanceChartComponent,
    ColorRevenueChartComponent,
    SalespersonChartComponent,
    CustomerCategoryChartComponent,
    TopCustomersChartComponent,
    SalesDayChartComponent,
    CityAvgOrderChartComponent,
  ],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class DashboardComponent {}
