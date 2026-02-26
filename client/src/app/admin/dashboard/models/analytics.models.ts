export interface DashboardSummary {
  totalRevenue: number;
  totalOrders: number;
  totalUnitsSold: number;
  averageOrderValue: number;
  totalCustomers: number;
  totalProducts: number;
  totalSalespersons: number;
}

export interface RevenueByYear {
  year: number;
  totalRevenue: number;
  totalOrders: number;
}

export interface MonthlyRevenue {
  year: number;
  month: number;
  monthName: string;
  totalRevenue: number;
  totalOrders: number;
}

export interface QuarterlyRevenue {
  year: number;
  quarter: number;
  totalRevenue: number;
  totalOrders: number;
}

export interface RevenueByTerritory {
  salesTerritory: string;
  totalRevenue: number;
  totalOrders: number;
}

export interface RevenueByState {
  stateProvinceName: string;
  salesTerritory: string;
  totalRevenue: number;
  totalOrders: number;
}

export interface TopProduct {
  rank: number;
  stockItemName: string;
  totalUnitsSold: number;
  totalRevenue: number;
}

export interface BrandPerformance {
  brand: string;
  totalRevenue: number;
  totalUnitsSold: number;
  productCount: number;
}

export interface ColorRevenue {
  colorName: string;
  totalRevenue: number;
  totalUnitsSold: number;
}

export interface Salesperson {
  employeeName: string;
  preferredName: string;
  revenueGenerated: number;
  totalOrders: number;
}

export interface CustomerCategoryRevenue {
  customerCategoryName: string;
  totalRevenue: number;
  totalOrders: number;
  uniqueCustomers: number;
}

export interface TopCustomer {
  rank: number;
  customerName: string;
  customerCategoryName: string;
  lifetimeValue: number;
  totalOrders: number;
}

export interface SalesByDay {
  dayOfWeek: string;
  totalUnitsSold: number;
  totalRevenue: number;
}

export interface CityAvgOrder {
  cityName: string;
  stateProvinceName: string;
  averageOrderValue: number;
  totalOrders: number;
}
