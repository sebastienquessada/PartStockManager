
using System.ComponentModel.DataAnnotations;

namespace PartStockManager.Adapter.Models
{
    public class PartDeletionRequest
    {
        [Required(ErrorMessage = "The part reference is mandatory !")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "The part reference should have between 1 and 100 characters !")]
        public string PartReference { get; set; }
    }
}
