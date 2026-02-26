import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { RequestsService } from '../../../shared/services/requests.service';
import {
  DashboardSummary,
  RevenueByYear,
  MonthlyRevenue,
  QuarterlyRevenue,
  RevenueByTerritory,
  RevenueByState,
  TopProduct,
  BrandPerformance,
  ColorRevenue,
  Salesperson,
  CustomerCategoryRevenue,
  TopCustomer,
  SalesByDay,
  CityAvgOrder,
  DateRange,
  DateFilter,
} from '../models/analytics.models';

@Injectable({ providedIn: 'root' })
export class AnalyticsService {
  private readonly BASE = '/api/Analytics';

  constructor(private requests: RequestsService) {}

  /** Builds query string fragment from a DateFilter (skips nulls). */
  private dateParams(filter?: DateFilter | null): string {
    if (!filter) return '';
    const parts: string[] = [];
    if (filter.from) parts.push(`from=${filter.from}`);
    if (filter.to) parts.push(`to=${filter.to}`);
    return parts.length ? parts.join('&') : '';
  }

  /** Appends date params. If the url already has ?, uses &. */
  private withDate(url: string, filter?: DateFilter | null): string {
    const dp = this.dateParams(filter);
    if (!dp) return url;
    return url.includes('?') ? `${url}&${dp}` : `${url}?${dp}`;
  }

  getDateRange(): Observable<DateRange> {
    return this.requests.get(`${this.BASE}/date-range`);
  }

  getDashboardSummary(filter?: DateFilter | null): Observable<DashboardSummary> {
    return this.requests.get(this.withDate(`${this.BASE}/dashboard`, filter));
  }

  getRevenueByYear(filter?: DateFilter | null): Observable<RevenueByYear[]> {
    return this.requests.get(this.withDate(`${this.BASE}/revenue/yearly`, filter));
  }

  getMonthlyRevenue(filter?: DateFilter | null): Observable<MonthlyRevenue[]> {
    return this.requests.get(this.withDate(`${this.BASE}/revenue/monthly`, filter));
  }

  getQuarterlyRevenue(filter?: DateFilter | null): Observable<QuarterlyRevenue[]> {
    return this.requests.get(this.withDate(`${this.BASE}/revenue/quarterly`, filter));
  }

  getRevenueByTerritory(filter?: DateFilter | null): Observable<RevenueByTerritory[]> {
    return this.requests.get(this.withDate(`${this.BASE}/revenue/territory`, filter));
  }

  getRevenueByState(filter?: DateFilter | null): Observable<RevenueByState[]> {
    return this.requests.get(this.withDate(`${this.BASE}/revenue/state`, filter));
  }

  getTopProducts(top = 10, filter?: DateFilter | null): Observable<TopProduct[]> {
    return this.requests.get(this.withDate(`${this.BASE}/products/top?top=${top}`, filter));
  }

  getBrandPerformance(filter?: DateFilter | null): Observable<BrandPerformance[]> {
    return this.requests.get(this.withDate(`${this.BASE}/products/brands`, filter));
  }

  getRevenueByColor(filter?: DateFilter | null): Observable<ColorRevenue[]> {
    return this.requests.get(this.withDate(`${this.BASE}/products/colors`, filter));
  }

  getSalespersonLeaderboard(filter?: DateFilter | null): Observable<Salesperson[]> {
    return this.requests.get(this.withDate(`${this.BASE}/employees/leaderboard`, filter));
  }

  getCustomerCategories(filter?: DateFilter | null): Observable<CustomerCategoryRevenue[]> {
    return this.requests.get(this.withDate(`${this.BASE}/customers/categories`, filter));
  }

  getTopCustomers(top = 5, filter?: DateFilter | null): Observable<TopCustomer[]> {
    return this.requests.get(this.withDate(`${this.BASE}/customers/top-vip?top=${top}`, filter));
  }

  getSalesByDayOfWeek(filter?: DateFilter | null): Observable<SalesByDay[]> {
    return this.requests.get(this.withDate(`${this.BASE}/sales/day-of-week`, filter));
  }

  getCityAvgOrder(filter?: DateFilter | null): Observable<CityAvgOrder[]> {
    return this.requests.get(this.withDate(`${this.BASE}/cities/avg-order`, filter));
  }
}
