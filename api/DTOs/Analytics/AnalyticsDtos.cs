namespace api.DTOs.Analytics
    {
    public class RevenueByYearDto
        {
        public int Year { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalOrders { get; set; }
        }

    public class TopProductDto
        {
        public int Rank { get; set; }
        public string StockItemName { get; set; } = string.Empty;
        public int TotalUnitsSold { get; set; }
        public decimal TotalRevenue { get; set; }
        }

    public class SalespersonDto
        {
        public string EmployeeName { get; set; } = string.Empty;
        public string PreferredName { get; set; } = string.Empty;
        public decimal RevenueGenerated { get; set; }
        public int TotalOrders { get; set; }
        }

    public class RevenueByTerritoryDto
        {
        public string SalesTerritory { get; set; } = string.Empty;
        public decimal TotalRevenue { get; set; }
        public int TotalOrders { get; set; }
        }

    public class CustomerCategoryRevenueDto
        {
        public string CustomerCategoryName { get; set; } = string.Empty;
        public decimal TotalRevenue { get; set; }
        public int TotalOrders { get; set; }
        public int UniqueCustomers { get; set; }
        }

    public class SalesByDayDto
        {
        public string DayOfWeek { get; set; } = string.Empty;
        public int TotalUnitsSold { get; set; }
        public decimal TotalRevenue { get; set; }
        }

    public class TopCustomerDto
        {
        public int Rank { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerCategoryName { get; set; } = string.Empty;
        public decimal LifetimeValue { get; set; }
        public int TotalOrders { get; set; }
        }

    public class BrandPerformanceDto
        {
        public string Brand { get; set; } = string.Empty;
        public decimal TotalRevenue { get; set; }
        public int TotalUnitsSold { get; set; }
        public int ProductCount { get; set; }
        }

    public class CityAvgOrderDto
        {
        public string CityName { get; set; } = string.Empty;
        public string StateProvinceName { get; set; } = string.Empty;
        public decimal AverageOrderValue { get; set; }
        public int TotalOrders { get; set; }
        }

    public class MonthlyRevenueDto
        {
        public int Year { get; set; }
        public int Month { get; set; }
        public string MonthName { get; set; } = string.Empty;
        public decimal TotalRevenue { get; set; }
        public int TotalOrders { get; set; }
        }

    public class QuarterlyRevenueDto
        {
        public int Year { get; set; }
        public int Quarter { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalOrders { get; set; }
        }

    public class DashboardSummaryDto
        {
        public decimal TotalRevenue { get; set; }
        public int TotalOrders { get; set; }
        public int TotalUnitsSold { get; set; }
        public decimal AverageOrderValue { get; set; }
        public int TotalCustomers { get; set; }
        public int TotalProducts { get; set; }
        public int TotalSalespersons { get; set; }
        }

    public class RevenueByStateDto
        {
        public string StateProvinceName { get; set; } = string.Empty;
        public string SalesTerritory { get; set; } = string.Empty;
        public decimal TotalRevenue { get; set; }
        public int TotalOrders { get; set; }
        }

    public class ColorRevenueDto
        {
        public string ColorName { get; set; } = string.Empty;
        public decimal TotalRevenue { get; set; }
        public int TotalUnitsSold { get; set; }
        }
    }
