using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.models.DW
    {
    [Table("Dim_Date")]
    public class DimDate
        {
        [Key]
        public int DateSK { get; set; }
        public DateTime FullDate { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public string MonthName { get; set; } = string.Empty;
        public int Quarter { get; set; }
        public string DayOfWeek { get; set; } = string.Empty;
        }
    }
