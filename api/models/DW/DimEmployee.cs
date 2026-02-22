using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.models.DW
    {
    [Table("Dim_Employee")]
    public class DimEmployee
        {
        [Key]
        public int EmployeeSK { get; set; }
        public int PersonID { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public string PreferredName { get; set; } = string.Empty;
        public bool IsSalesperson { get; set; }
        }
    }
