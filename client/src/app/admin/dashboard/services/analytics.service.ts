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
} from '../models/analytics.models';

@Injectable({ providedIn: 'root' })
export class AnalyticsService {
  private readonly BASE = '/api/Analytics';

  constructor(private requests: RequestsService) {}

  getDashboardSummary(): Observable<DashboardSummary> {
    return this.requests.get(`${this.BASE}/dashboard`);
  }

  getRevenueByYear(): Observable<RevenueByYear[]> {
    return this.requests.get(`${this.BASE}/revenue/yearly`);
  }

  getMonthlyRevenue(): Observable<MonthlyRevenue[]> {
    return this.requests.get(`${this.BASE}/revenue/monthly`);
  }

  getQuarterlyRevenue(): Observable<QuarterlyRevenue[]> {
    return this.requests.get(`${this.BASE}/revenue/quarterly`);
  }

  getRevenueByTerritory(): Observable<RevenueByTerritory[]> {
    return this.requests.get(`${this.BASE}/revenue/territory`);
  }

  getRevenueByState(): Observable<RevenueByState[]> {
    return this.requests.get(`${this.BASE}/revenue/state`);
  }

  getTopProducts(top = 10): Observable<TopProduct[]> {
    return this.requests.get(`${this.BASE}/products/top?top=${top}`);
  }

  getBrandPerformance(): Observable<BrandPerformance[]> {
    return this.requests.get(`${this.BASE}/products/brands`);
  }

  getRevenueByColor(): Observable<ColorRevenue[]> {
    return this.requests.get(`${this.BASE}/products/colors`);
  }

  getSalespersonLeaderboard(): Observable<Salesperson[]> {
    return this.requests.get(`${this.BASE}/employees/leaderboard`);
  }

  getCustomerCategories(): Observable<CustomerCategoryRevenue[]> {
    return this.requests.get(`${this.BASE}/customers/categories`);
  }

  getTopCustomers(top = 5): Observable<TopCustomer[]> {
    return this.requests.get(`${this.BASE}/customers/top-vip?top=${top}`);
  }

  getSalesByDayOfWeek(): Observable<SalesByDay[]> {
    return this.requests.get(`${this.BASE}/sales/day-of-week`);
  }

  getCityAvgOrder(): Observable<CityAvgOrder[]> {
    return this.requests.get(`${this.BASE}/cities/avg-order`);
  }
}
