using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.models.DW
    {
    [Table("Dim_City")]
    public class DimCity
        {
        [Key]
        public int CitySK { get; set; }
        public int CityID { get; set; }
        public string CityName { get; set; } = string.Empty;
        public string StateProvinceName { get; set; } = string.Empty;
        public string SalesTerritory { get; set; } = string.Empty;
        }
    }
