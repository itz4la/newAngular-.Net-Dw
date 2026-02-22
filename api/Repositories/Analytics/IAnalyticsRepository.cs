using api.DTOs.Analytics;

namespace api.Repositories.Analytics
    {
    public interface IAnalyticsRepository
        {
        // Stat 1 – Revenue trend over years
        Task<List<RevenueByYearDto>> GetRevenueByYearAsync();

        // Stat 2 – Top N best-selling products by units sold
        Task<List<TopProductDto>> GetTopSellingProductsAsync(int top = 10);

        // Stat 3 – Salesperson revenue leaderboard
        Task<List<SalespersonDto>> GetSalespersonLeaderboardAsync();

        // Stat 4 – Revenue by sales territory
        Task<List<RevenueByTerritoryDto>> GetRevenueByTerritoryAsync();

        // Stat 5 – Most valuable customer categories
        Task<List<CustomerCategoryRevenueDto>> GetRevenueByCustomerCategoryAsync();

        // Stat 6 – Peak sales day of the week
        Task<List<SalesByDayDto>> GetSalesByDayOfWeekAsync();

        // Stat 7 – Top N customers by lifetime value (VIP)
        Task<List<TopCustomerDto>> GetTopCustomersByLifetimeValueAsync(int top = 5);

        // Stat 8 – Product brand performance
        Task<List<BrandPerformanceDto>> GetBrandPerformanceAsync();

        // Stat 9 – Average order value by city
        Task<List<CityAvgOrderDto>> GetAverageOrderValueByCityAsync();

        // Stat 10 – Monthly revenue (year-over-year comparison)
        Task<List<MonthlyRevenueDto>> GetMonthlyRevenueAsync();

        // Stat 11 – Quarterly revenue breakdown
        Task<List<QuarterlyRevenueDto>> GetQuarterlyRevenueAsync();

        // Stat 12 – Overall KPI dashboard summary
        Task<DashboardSummaryDto> GetDashboardSummaryAsync();

        // Stat 13 – Revenue by state/province
        Task<List<RevenueByStateDto>> GetRevenueByStateAsync();

        // Stat 14 – Revenue by product color
        Task<List<ColorRevenueDto>> GetRevenueByColorAsync();
        }
    }
