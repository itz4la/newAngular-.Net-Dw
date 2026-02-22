using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.models.DW
    {
    [Table("Dim_Customer")]
    public class DimCustomer
        {
        [Key]
        public int CustomerSK { get; set; }
        public int CustomerID { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerCategoryName { get; set; } = string.Empty;
        public string BuyingGroupName { get; set; } = string.Empty;
        }
    }
