using api.DTOs.Analytics;
using api.models;
using Microsoft.EntityFrameworkCore;

namespace api.Repositories.Analytics
    {
    public class AnalyticsRepository : IAnalyticsRepository
        {
        private readonly DWContext _context;

        public AnalyticsRepository(DWContext context)
            {
            _context = context;
            }

        // Stat 1 – Total revenue grouped by year
        public async Task<List<RevenueByYearDto>> GetRevenueByYearAsync()
            {
            return await _context.Fact_Sales
                .Join(_context.Dim_Date,
                    f => f.DateSK,
                    d => (int?)d.DateSK,
                    (f, d) => new { f.TotalAmount, d.Year })
                .GroupBy(x => x.Year)
                .Select(g => new RevenueByYearDto
                    {
                    Year = g.Key,
                    TotalRevenue = g.Sum(x => x.TotalAmount),
                    TotalOrders = g.Count()
                    })
                .OrderBy(x => x.Year)
                .ToListAsync();
            }

        // Stat 2 – Top N products by units sold
        public async Task<List<TopProductDto>> GetTopSellingProductsAsync(int top = 10)
            {
            var results = await _context.Fact_Sales
                .Join(_context.Dim_StockItem,
                    f => f.StockItemSK,
                    s => (int?)s.StockItemSK,
                    (f, s) => new { f.Quantity, f.TotalAmount, s.StockItemName })
                .GroupBy(x => x.StockItemName)
                .Select(g => new TopProductDto
                    {
                    StockItemName = g.Key,
                    TotalUnitsSold = g.Sum(x => x.Quantity),
                    TotalRevenue = g.Sum(x => x.TotalAmount)
                    })
                .OrderByDescending(x => x.TotalUnitsSold)
                .Take(top)
                .ToListAsync();

            for (int i = 0; i < results.Count; i++)
                results[i].Rank = i + 1;

            return results;
            }

        // Stat 3 – Salesperson leaderboard by revenue generated
        public async Task<List<SalespersonDto>> GetSalespersonLeaderboardAsync()
            {
            return await _context.Fact_Sales
                .Join(_context.Dim_Employee,
                    f => f.EmployeeSK,
                    e => (int?)e.EmployeeSK,
                    (f, e) => new { f.TotalAmount, e.EmployeeName, e.PreferredName })
                .GroupBy(x => new { x.EmployeeName, x.PreferredName })
                .Select(g => new SalespersonDto
                    {
                    EmployeeName = g.Key.EmployeeName,
                    PreferredName = g.Key.PreferredName,
                    RevenueGenerated = g.Sum(x => x.TotalAmount),
                    TotalOrders = g.Count()
                    })
                .OrderByDescending(x => x.RevenueGenerated)
                .ToListAsync();
            }

        // Stat 4 – Revenue broken down by sales territory
        public async Task<List<RevenueByTerritoryDto>> GetRevenueByTerritoryAsync()
            {
            return await _context.Fact_Sales
                .Join(_context.Dim_City,
                    f => f.CitySK,
                    c => (int?)c.CitySK,
                    (f, c) => new { f.TotalAmount, c.SalesTerritory })
                .GroupBy(x => x.SalesTerritory)
                .Select(g => new RevenueByTerritoryDto
                    {
                    SalesTerritory = g.Key,
                    TotalRevenue = g.Sum(x => x.TotalAmount),
                    TotalOrders = g.Count()
                    })
                .OrderByDescending(x => x.TotalRevenue)
                .ToListAsync();
            }

        // Stat 5 – Revenue and unique customer count per customer category
        public async Task<List<CustomerCategoryRevenueDto>> GetRevenueByCustomerCategoryAsync()
            {
            return await _context.Fact_Sales
                .Join(_context.Dim_Customer,
                    f => f.CustomerSK,
                    c => (int?)c.CustomerSK,
                    (f, c) => new { f.TotalAmount, c.CustomerCategoryName, c.CustomerSK })
                .GroupBy(x => x.CustomerCategoryName)
                .Select(g => new CustomerCategoryRevenueDto
                    {
                    CustomerCategoryName = g.Key,
                    TotalRevenue = g.Sum(x => x.TotalAmount),
                    TotalOrders = g.Count(),
                    UniqueCustomers = g.Select(x => x.CustomerSK).Distinct().Count()
                    })
                .OrderByDescending(x => x.TotalRevenue)
                .ToListAsync();
            }

        // Stat 6 – Units sold and revenue per day of the week
        public async Task<List<SalesByDayDto>> GetSalesByDayOfWeekAsync()
            {
            return await _context.Fact_Sales
                .Join(_context.Dim_Date,
                    f => f.DateSK,
                    d => (int?)d.DateSK,
                    (f, d) => new { f.Quantity, f.TotalAmount, d.DayOfWeek })
                .GroupBy(x => x.DayOfWeek)
                .Select(g => new SalesByDayDto
                    {
                    DayOfWeek = g.Key,
                    TotalUnitsSold = g.Sum(x => x.Quantity),
                    TotalRevenue = g.Sum(x => x.TotalAmount)
                    })
                .OrderByDescending(x => x.TotalRevenue)
                .ToListAsync();
            }

        // Stat 7 – Top N customers by total lifetime spend (VIP list)
        public async Task<List<TopCustomerDto>> GetTopCustomersByLifetimeValueAsync(int top = 5)
            {
            var results = await _context.Fact_Sales
                .Join(_context.Dim_Customer,
                    f => f.CustomerSK,
                    c => (int?)c.CustomerSK,
                    (f, c) => new { f.TotalAmount, c.CustomerName, c.CustomerCategoryName })
                .GroupBy(x => new { x.CustomerName, x.CustomerCategoryName })
                .Select(g => new TopCustomerDto
                    {
                    CustomerName = g.Key.CustomerName,
                    CustomerCategoryName = g.Key.CustomerCategoryName,
                    LifetimeValue = g.Sum(x => x.TotalAmount),
                    TotalOrders = g.Count()
                    })
                .OrderByDescending(x => x.LifetimeValue)
                .Take(top)
                .ToListAsync();

            for (int i = 0; i < results.Count; i++)
                results[i].Rank = i + 1;

            return results;
            }

        // Stat 8 – Revenue and units sold per product brand (N/A brands excluded)
        public async Task<List<BrandPerformanceDto>> GetBrandPerformanceAsync()
            {
            return await _context.Fact_Sales
                .Join(_context.Dim_StockItem,
                    f => f.StockItemSK,
                    s => (int?)s.StockItemSK,
                    (f, s) => new { f.TotalAmount, f.Quantity, s.Brand, s.StockItemSK })
                .Where(x => x.Brand != "N/A")
                .GroupBy(x => x.Brand)
                .Select(g => new BrandPerformanceDto
                    {
                    Brand = g.Key,
                    TotalRevenue = g.Sum(x => x.TotalAmount),
                    TotalUnitsSold = g.Sum(x => x.Quantity),
                    ProductCount = g.Select(x => x.StockItemSK).Distinct().Count()
                    })
                .OrderByDescending(x => x.TotalRevenue)
                .ToListAsync();
            }

        // Stat 9 – Average order value per city (pricing strategy insight)
        public async Task<List<CityAvgOrderDto>> GetAverageOrderValueByCityAsync()
            {
            return await _context.Fact_Sales
                .Join(_context.Dim_City,
                    f => f.CitySK,
                    c => (int?)c.CitySK,
                    (f, c) => new { f.TotalAmount, c.CityName, c.StateProvinceName })
                .GroupBy(x => new { x.CityName, x.StateProvinceName })
                .Select(g => new CityAvgOrderDto
                    {
                    CityName = g.Key.CityName,
                    StateProvinceName = g.Key.StateProvinceName,
                    AverageOrderValue = g.Average(x => x.TotalAmount),
                    TotalOrders = g.Count()
                    })
                .OrderByDescending(x => x.AverageOrderValue)
                .ToListAsync();
            }

        // Stat 10 – Monthly revenue (use for year-over-year line chart)
        public async Task<List<MonthlyRevenueDto>> GetMonthlyRevenueAsync()
            {
            return await _context.Fact_Sales
                .Join(_context.Dim_Date,
                    f => f.DateSK,
                    d => (int?)d.DateSK,
                    (f, d) => new { f.TotalAmount, d.Year, d.Month, d.MonthName })
                .GroupBy(x => new { x.Year, x.Month, x.MonthName })
                .Select(g => new MonthlyRevenueDto
                    {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    MonthName = g.Key.MonthName,
                    TotalRevenue = g.Sum(x => x.TotalAmount),
                    TotalOrders = g.Count()
                    })
                .OrderBy(x => x.Year)
                .ThenBy(x => x.Month)
                .ToListAsync();
            }

        // Stat 11 – Quarterly revenue breakdown
        public async Task<List<QuarterlyRevenueDto>> GetQuarterlyRevenueAsync()
            {
            return await _context.Fact_Sales
                .Join(_context.Dim_Date,
                    f => f.DateSK,
                    d => (int?)d.DateSK,
                    (f, d) => new { f.TotalAmount, d.Year, d.Quarter })
                .GroupBy(x => new { x.Year, x.Quarter })
                .Select(g => new QuarterlyRevenueDto
                    {
                    Year = g.Key.Year,
                    Quarter = g.Key.Quarter,
                    TotalRevenue = g.Sum(x => x.TotalAmount),
                    TotalOrders = g.Count()
                    })
                .OrderBy(x => x.Year)
                .ThenBy(x => x.Quarter)
                .ToListAsync();
            }

        // Stat 12 – High-level KPI dashboard (single-query aggregation)
        public async Task<DashboardSummaryDto> GetDashboardSummaryAsync()
            {
            var totalOrders = await _context.Fact_Sales.CountAsync();
            if (totalOrders == 0)
                return new DashboardSummaryDto();

            var totalRevenue = await _context.Fact_Sales.SumAsync(f => f.TotalAmount);
            var totalUnitsSold = await _context.Fact_Sales.SumAsync(f => f.Quantity);
            var avgOrderValue = await _context.Fact_Sales.AverageAsync(f => f.TotalAmount);
            var totalCustomers = await _context.Fact_Sales
                .Where(f => f.CustomerSK != null)
                .Select(f => f.CustomerSK)
                .Distinct()
                .CountAsync();
            var totalProducts = await _context.Fact_Sales
                .Where(f => f.StockItemSK != null)
                .Select(f => f.StockItemSK)
                .Distinct()
                .CountAsync();
            var totalSalespersons = await _context.Fact_Sales
                .Where(f => f.EmployeeSK != null)
                .Select(f => f.EmployeeSK)
                .Distinct()
                .CountAsync();

            return new DashboardSummaryDto
                {
                TotalRevenue = totalRevenue,
                TotalOrders = totalOrders,
                TotalUnitsSold = totalUnitsSold,
                AverageOrderValue = Math.Round(avgOrderValue, 2),
                TotalCustomers = totalCustomers,
                TotalProducts = totalProducts,
                TotalSalespersons = totalSalespersons
                };
            }

        // Stat 13 – Revenue aggregated by state/province
        public async Task<List<RevenueByStateDto>> GetRevenueByStateAsync()
            {
            return await _context.Fact_Sales
                .Join(_context.Dim_City,
                    f => f.CitySK,
                    c => (int?)c.CitySK,
                    (f, c) => new { f.TotalAmount, c.StateProvinceName, c.SalesTerritory })
                .GroupBy(x => new { x.StateProvinceName, x.SalesTerritory })
                .Select(g => new RevenueByStateDto
                    {
                    StateProvinceName = g.Key.StateProvinceName,
                    SalesTerritory = g.Key.SalesTerritory,
                    TotalRevenue = g.Sum(x => x.TotalAmount),
                    TotalOrders = g.Count()
                    })
                .OrderByDescending(x => x.TotalRevenue)
                .ToListAsync();
            }

        // Stat 14 – Revenue by product color (N/A colors excluded)
        public async Task<List<ColorRevenueDto>> GetRevenueByColorAsync()
            {
            return await _context.Fact_Sales
                .Join(_context.Dim_StockItem,
                    f => f.StockItemSK,
                    s => (int?)s.StockItemSK,
                    (f, s) => new { f.TotalAmount, f.Quantity, s.ColorName })
                .Where(x => x.ColorName != "N/A")
                .GroupBy(x => x.ColorName)
                .Select(g => new ColorRevenueDto
                    {
                    ColorName = g.Key,
                    TotalRevenue = g.Sum(x => x.TotalAmount),
                    TotalUnitsSold = g.Sum(x => x.Quantity)
                    })
                .OrderByDescending(x => x.TotalRevenue)
                .ToListAsync();
            }
        }
    }
