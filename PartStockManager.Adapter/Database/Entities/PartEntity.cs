using System.ComponentModel.DataAnnotations;

namespace PartStockManager.Adapter.Database.Entities
{
    public class PartEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Reference { get; set; }

        [Required]
        public string Name { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "The stock quantity can't be negative !")]
        public int StockQuantity { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "The low stock threshold can't be negative !")]
        public int LowStockThreshold { get; set; }
    }
}
