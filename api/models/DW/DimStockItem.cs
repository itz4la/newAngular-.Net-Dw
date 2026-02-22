using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.models.DW
    {
    [Table("Dim_StockItem")]
    public class DimStockItem
        {
        [Key]
        public int StockItemSK { get; set; }
        public int StockItemID { get; set; }
        public string StockItemName { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public string Size { get; set; } = string.Empty;
        public string ColorName { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        }
    }
