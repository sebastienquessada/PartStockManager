using System.ComponentModel.DataAnnotations;

namespace PartStockManager.Adapter.Models
{
    public class InventoryItem
    {
        public InventoryItem(string reference, int quantity)
        {
            Reference = reference;
            Quantity = quantity;
        }

        [Required(ErrorMessage = "The part reference is mandatory !")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "The part reference should have between 1 and 100 characters !")]
        public string Reference { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "The stock quantity can't be lower than zero !")]
        public int Quantity { get; set; }
    }
}
