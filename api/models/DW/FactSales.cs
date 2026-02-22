using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.models.DW
    {
    [Table("Fact_Sales")]
    public class FactSales
        {
        [Key]
        public int SalesSK { get; set; }
        public int InvoiceLineID { get; set; }
        public int? DateSK { get; set; }
        public int? CustomerSK { get; set; }
        public int? StockItemSK { get; set; }
        public int? EmployeeSK { get; set; }
        public int? CitySK { get; set; }
        public int Quantity { get; set; }
        public decimal TotalAmount { get; set; }

        [ForeignKey(nameof(DateSK))]
        public DimDate? Date { get; set; }

        [ForeignKey(nameof(CustomerSK))]
        public DimCustomer? Customer { get; set; }

        [ForeignKey(nameof(StockItemSK))]
        public DimStockItem? StockItem { get; set; }

        [ForeignKey(nameof(EmployeeSK))]
        public DimEmployee? Employee { get; set; }

        [ForeignKey(nameof(CitySK))]
        public DimCity? City { get; set; }
        }
    }
