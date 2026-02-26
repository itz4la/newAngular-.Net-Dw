using api.DTOs.Analytics;

namespace api.Repositories.Analytics
    {
    public interface IAnalyticsRepository
        {
        // Date range from Dim_Date
        Task<DateRangeDto> GetDateRangeAsync();

        // Stat 1 – Revenue trend over years
        Task<List<RevenueByYearDto>> GetRevenueByYearAsync(DateTime? from = null, DateTime? to = null);

        // Stat 2 – Top N best-selling products by units sold
        Task<List<TopProductDto>> GetTopSellingProductsAsync(int top = 10, DateTime? from = null, DateTime? to = null);

        // Stat 3 – Salesperson revenue leaderboard
        Task<List<SalespersonDto>> GetSalespersonLeaderboardAsync(DateTime? from = null, DateTime? to = null);

        // Stat 4 – Revenue by sales territory
        Task<List<RevenueByTerritoryDto>> GetRevenueByTerritoryAsync(DateTime? from = null, DateTime? to = null);

        // Stat 5 – Most valuable customer categories
        Task<List<CustomerCategoryRevenueDto>> GetRevenueByCustomerCategoryAsync(DateTime? from = null, DateTime? to = null);

        // Stat 6 – Peak sales day of the week
        Task<List<SalesByDayDto>> GetSalesByDayOfWeekAsync(DateTime? from = null, DateTime? to = null);

        // Stat 7 – Top N customers by lifetime value (VIP)
        Task<List<TopCustomerDto>> GetTopCustomersByLifetimeValueAsync(int top = 5, DateTime? from = null, DateTime? to = null);

        // Stat 8 – Product brand performance
        Task<List<BrandPerformanceDto>> GetBrandPerformanceAsync(DateTime? from = null, DateTime? to = null);

        // Stat 9 – Average order value by city
        Task<List<CityAvgOrderDto>> GetAverageOrderValueByCityAsync(DateTime? from = null, DateTime? to = null);

        // Stat 10 – Monthly revenue (year-over-year comparison)
        Task<List<MonthlyRevenueDto>> GetMonthlyRevenueAsync(DateTime? from = null, DateTime? to = null);

        // Stat 11 – Quarterly revenue breakdown
        Task<List<QuarterlyRevenueDto>> GetQuarterlyRevenueAsync(DateTime? from = null, DateTime? to = null);

        // Stat 12 – Overall KPI dashboard summary
        Task<DashboardSummaryDto> GetDashboardSummaryAsync(DateTime? from = null, DateTime? to = null);

        // Stat 13 – Revenue by state/province
        Task<List<RevenueByStateDto>> GetRevenueByStateAsync(DateTime? from = null, DateTime? to = null);

        // Stat 14 – Revenue by product color
        Task<List<ColorRevenueDto>> GetRevenueByColorAsync(DateTime? from = null, DateTime? to = null);
        }
    }
