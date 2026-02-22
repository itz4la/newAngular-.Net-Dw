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

        /// <summary>
        /// Stat 12 – High-level KPI summary: total revenue, orders, customers, products, salespersons and average order value.
        /// </summary>
        [HttpGet("dashboard")]
        public async Task<ActionResult<DashboardSummaryDto>> GetDashboardSummary()
            {
            var result = await _analyticsRepository.GetDashboardSummaryAsync();
            return Ok(result);
            }

        // ── Revenue endpoints ─────────────────────────────────────────────────

        /// <summary>
        /// Stat 1 – Total revenue per year (trend analysis).
        /// </summary>
        [HttpGet("revenue/yearly")]
        public async Task<ActionResult<List<RevenueByYearDto>>> GetRevenueByYear()
            {
            var result = await _analyticsRepository.GetRevenueByYearAsync();
            return Ok(result);
            }

        /// <summary>
        /// Stat 10 – Monthly revenue for all years (year-over-year comparison).
        /// </summary>
        [HttpGet("revenue/monthly")]
        public async Task<ActionResult<List<MonthlyRevenueDto>>> GetMonthlyRevenue()
            {
            var result = await _analyticsRepository.GetMonthlyRevenueAsync();
            return Ok(result);
            }

        /// <summary>
        /// Stat 11 – Quarterly revenue breakdown per year.
        /// </summary>
        [HttpGet("revenue/quarterly")]
        public async Task<ActionResult<List<QuarterlyRevenueDto>>> GetQuarterlyRevenue()
            {
            var result = await _analyticsRepository.GetQuarterlyRevenueAsync();
            return Ok(result);
            }

        /// <summary>
        /// Stat 4 – Revenue grouped by sales territory (geographical analysis).
        /// </summary>
        [HttpGet("revenue/territory")]
        public async Task<ActionResult<List<RevenueByTerritoryDto>>> GetRevenueByTerritory()
            {
            var result = await _analyticsRepository.GetRevenueByTerritoryAsync();
            return Ok(result);
            }

        /// <summary>
        /// Stat 13 – Revenue grouped by state/province.
        /// </summary>
        [HttpGet("revenue/state")]
        public async Task<ActionResult<List<RevenueByStateDto>>> GetRevenueByState()
            {
            var result = await _analyticsRepository.GetRevenueByStateAsync();
            return Ok(result);
            }

        // ── Product endpoints ─────────────────────────────────────────────────

        /// <summary>
        /// Stat 2 – Top N best-selling products by units sold. Default top = 10, max = 100.
        /// </summary>
        [HttpGet("products/top")]
        public async Task<ActionResult<List<TopProductDto>>> GetTopSellingProducts([FromQuery] int top = 10)
            {
            if (top <= 0 || top > 100)
                return BadRequest("top must be between 1 and 100.");

            var result = await _analyticsRepository.GetTopSellingProductsAsync(top);
            return Ok(result);
            }

        /// <summary>
        /// Stat 8 – Revenue and units sold per brand (N/A brands excluded).
        /// </summary>
        [HttpGet("products/brands")]
        public async Task<ActionResult<List<BrandPerformanceDto>>> GetBrandPerformance()
            {
            var result = await _analyticsRepository.GetBrandPerformanceAsync();
            return Ok(result);
            }

        /// <summary>
        /// Stat 14 – Revenue and units sold grouped by product colour (N/A colours excluded).
        /// </summary>
        [HttpGet("products/colors")]
        public async Task<ActionResult<List<ColorRevenueDto>>> GetRevenueByColor()
            {
            var result = await _analyticsRepository.GetRevenueByColorAsync();
            return Ok(result);
            }

        // ── Employee endpoints ────────────────────────────────────────────────

        /// <summary>
        /// Stat 3 – Salesperson leaderboard sorted by revenue generated.
        /// </summary>
        [HttpGet("employees/leaderboard")]
        public async Task<ActionResult<List<SalespersonDto>>> GetSalespersonLeaderboard()
            {
            var result = await _analyticsRepository.GetSalespersonLeaderboardAsync();
            return Ok(result);
            }

        // ── Customer endpoints ────────────────────────────────────────────────

        /// <summary>
        /// Stat 5 – Revenue per customer category (market segmentation).
        /// </summary>
        [HttpGet("customers/categories")]
        public async Task<ActionResult<List<CustomerCategoryRevenueDto>>> GetRevenueByCustomerCategory()
            {
            var result = await _analyticsRepository.GetRevenueByCustomerCategoryAsync();
            return Ok(result);
            }

        /// <summary>
        /// Stat 7 – Top N VIP customers by lifetime spend. Default top = 5, max = 100.
        /// </summary>
        [HttpGet("customers/top-vip")]
        public async Task<ActionResult<List<TopCustomerDto>>> GetTopCustomers([FromQuery] int top = 5)
            {
            if (top <= 0 || top > 100)
                return BadRequest("top must be between 1 and 100.");

            var result = await _analyticsRepository.GetTopCustomersByLifetimeValueAsync(top);
            return Ok(result);
            }

        // ── Sales behaviour endpoints ─────────────────────────────────────────

        /// <summary>
        /// Stat 6 – Units sold and revenue grouped by day of the week (peak day analysis).
        /// </summary>
        [HttpGet("sales/day-of-week")]
        public async Task<ActionResult<List<SalesByDayDto>>> GetSalesByDayOfWeek()
            {
            var result = await _analyticsRepository.GetSalesByDayOfWeekAsync();
            return Ok(result);
            }

        /// <summary>
        /// Stat 9 – Average order value per city (pricing strategy insight).
        /// </summary>
        [HttpGet("cities/avg-order")]
        public async Task<ActionResult<List<CityAvgOrderDto>>> GetAverageOrderValueByCity()
            {
            var result = await _analyticsRepository.GetAverageOrderValueByCityAsync();
            return Ok(result);
            }
        }
    }
