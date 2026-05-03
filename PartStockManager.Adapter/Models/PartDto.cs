using System.ComponentModel.DataAnnotations;

namespace PartStockManager.Adapter.Models
{
    public class PartDto
    {
        [Required(ErrorMessage = "The part name is mandatory !")]
        [StringLength(200, MinimumLength = 1, ErrorMessage = "The part name should have between 1 and 200 characters !")]
        public string Name { get; set; }

        [Required(ErrorMessage = "The part reference is mandatory !")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "The part reference should have between 1 and 100 characters !")]
        public string Reference { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "The stock quantity can't be lower than zero !")]
        public int StockQuantity { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "The low stock threshold can't be lower than zero !")]
        public int LowStockThreshold { get; set; }
    }
}
