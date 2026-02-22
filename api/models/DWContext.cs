using api.models.DW;
using Microsoft.EntityFrameworkCore;

namespace api.models
    {
    public class DWContext : DbContext
        {
        public DWContext(DbContextOptions<DWContext> options) : base(options) { }

        public DbSet<FactSales> Fact_Sales { get; set; }
        public DbSet<DimDate> Dim_Date { get; set; }
        public DbSet<DimCustomer> Dim_Customer { get; set; }
        public DbSet<DimEmployee> Dim_Employee { get; set; }
        public DbSet<DimCity> Dim_City { get; set; }
        public DbSet<DimStockItem> Dim_StockItem { get; set; }
        }
    }
