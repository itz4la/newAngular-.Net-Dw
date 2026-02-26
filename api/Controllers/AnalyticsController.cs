using api.DTOs.Analytics;
using api.Repositories.Analytics;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
    {
    [Route("api/[controller]")]
    [ApiController]
    public class AnalyticsController : ControllerBase
        {
        private readonly IAnalyticsRepository _analyticsRepository;

        public AnalyticsController(IAnalyticsRepository analyticsRepository)
            {
            _analyticsRepository = analyticsRepository;
            }

        // ── Date range endpoint ───────────────────────────────────────────────

        /// <summary>
        /// Returns the min and max dates available in the Dim_Date dimension.
        /// </summary>
        [HttpGet("date-range")]
        public async Task<ActionResult<DateRangeDto>> GetDateRange()
            {
            var result = await _analyticsRepository.GetDateRangeAsync();
            return Ok(result);
            }

        /// <summary>
        /// Stat 12 – High-level KPI summary: total revenue, orders, customers, products, salespersons and average order value.
        /// </summary>
        [HttpGet("dashboard")]
        public async Task<ActionResult<DashboardSummaryDto>> GetDashboardSummary([FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null)
            {
            var result = await _analyticsRepository.GetDashboardSummaryAsync(from, to);
            return Ok(result);
            }

        // ── Revenue endpoints ─────────────────────────────────────────────────

        /// <summary>
        /// Stat 1 – Total revenue per year (trend analysis).
        /// </summary>
        [HttpGet("revenue/yearly")]
        public async Task<ActionResult<List<RevenueByYearDto>>> GetRevenueByYear([FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null)
            {
            var result = await _analyticsRepository.GetRevenueByYearAsync(from, to);
            return Ok(result);
            }

        /// <summary>
        /// Stat 10 – Monthly revenue for all years (year-over-year comparison).
        /// </summary>
        [HttpGet("revenue/monthly")]
        public async Task<ActionResult<List<MonthlyRevenueDto>>> GetMonthlyRevenue([FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null)
            {
            var result = await _analyticsRepository.GetMonthlyRevenueAsync(from, to);
            return Ok(result);
            }

        /// <summary>
        /// Stat 11 – Quarterly revenue breakdown per year.
        /// </summary>
        [HttpGet("revenue/quarterly")]
        public async Task<ActionResult<List<QuarterlyRevenueDto>>> GetQuarterlyRevenue([FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null)
            {
            var result = await _analyticsRepository.GetQuarterlyRevenueAsync(from, to);
            return Ok(result);
            }

        /// <summary>
        /// Stat 4 – Revenue grouped by sales territory (geographical analysis).
        /// </summary>
        [HttpGet("revenue/territory")]
        public async Task<ActionResult<List<RevenueByTerritoryDto>>> GetRevenueByTerritory([FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null)
            {
            var result = await _analyticsRepository.GetRevenueByTerritoryAsync(from, to);
            return Ok(result);
            }

        /// <summary>
        /// Stat 13 – Revenue grouped by state/province.
        /// </summary>
        [HttpGet("revenue/state")]
        public async Task<ActionResult<List<RevenueByStateDto>>> GetRevenueByState([FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null)
            {
            var result = await _analyticsRepository.GetRevenueByStateAsync(from, to);
            return Ok(result);
            }

        // ── Product endpoints ─────────────────────────────────────────────────

        /// <summary>
        /// Stat 2 – Top N best-selling products by units sold. Default top = 10, max = 100.
        /// </summary>
        [HttpGet("products/top")]
        public async Task<ActionResult<List<TopProductDto>>> GetTopSellingProducts([FromQuery] int top = 10, [FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null)
            {
            if (top <= 0 || top > 100)
                return BadRequest("top must be between 1 and 100.");

            var result = await _analyticsRepository.GetTopSellingProductsAsync(top, from, to);
            return Ok(result);
            }

        /// <summary>
        /// Stat 8 – Revenue and units sold per brand (N/A brands excluded).
        /// </summary>
        [HttpGet("products/brands")]
        public async Task<ActionResult<List<BrandPerformanceDto>>> GetBrandPerformance([FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null)
            {
            var result = await _analyticsRepository.GetBrandPerformanceAsync(from, to);
            return Ok(result);
            }

        /// <summary>
        /// Stat 14 – Revenue and units sold grouped by product colour (N/A colours excluded).
        /// </summary>
        [HttpGet("products/colors")]
        public async Task<ActionResult<List<ColorRevenueDto>>> GetRevenueByColor([FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null)
            {
            var result = await _analyticsRepository.GetRevenueByColorAsync(from, to);
            return Ok(result);
            }

        // ── Employee endpoints ────────────────────────────────────────────────

        /// <summary>
        /// Stat 3 – Salesperson leaderboard sorted by revenue generated.
        /// </summary>
        [HttpGet("employees/leaderboard")]
        public async Task<ActionResult<List<SalespersonDto>>> GetSalespersonLeaderboard([FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null)
            {
            var result = await _analyticsRepository.GetSalespersonLeaderboardAsync(from, to);
            return Ok(result);
            }

        // ── Customer endpoints ────────────────────────────────────────────────

        /// <summary>
        /// Stat 5 – Revenue per customer category (market segmentation).
        /// </summary>
        [HttpGet("customers/categories")]
        public async Task<ActionResult<List<CustomerCategoryRevenueDto>>> GetRevenueByCustomerCategory([FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null)
            {
            var result = await _analyticsRepository.GetRevenueByCustomerCategoryAsync(from, to);
            return Ok(result);
            }

        /// <summary>
        /// Stat 7 – Top N VIP customers by lifetime spend. Default top = 5, max = 100.
        /// </summary>
        [HttpGet("customers/top-vip")]
        public async Task<ActionResult<List<TopCustomerDto>>> GetTopCustomers([FromQuery] int top = 5, [FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null)
            {
            if (top <= 0 || top > 100)
                return BadRequest("top must be between 1 and 100.");

            var result = await _analyticsRepository.GetTopCustomersByLifetimeValueAsync(top, from, to);
            return Ok(result);
            }

        // ── Sales behaviour endpoints ─────────────────────────────────────────

        /// <summary>
        /// Stat 6 – Units sold and revenue grouped by day of the week (peak day analysis).
        /// </summary>
        [HttpGet("sales/day-of-week")]
        public async Task<ActionResult<List<SalesByDayDto>>> GetSalesByDayOfWeek([FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null)
            {
            var result = await _analyticsRepository.GetSalesByDayOfWeekAsync(from, to);
            return Ok(result);
            }

        /// <summary>
        /// Stat 9 – Average order value per city (pricing strategy insight).
        /// </summary>
        [HttpGet("cities/avg-order")]
        public async Task<ActionResult<List<CityAvgOrderDto>>> GetAverageOrderValueByCity([FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null)
            {
            var result = await _analyticsRepository.GetAverageOrderValueByCityAsync(from, to);
            return Ok(result);
            }
        }
    }
