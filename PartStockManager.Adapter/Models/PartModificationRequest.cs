
using System.ComponentModel.DataAnnotations;

namespace PartStockManager.Adapter.Models
{
    public class PartModificationRequest
    {
        [Required(ErrorMessage = "The current part reference is mandatory !")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "The current part reference should have between 1 and 100 characters !")] 
        public string CurrentPartReference { get; set; }

        [Required(ErrorMessage = "The updated part is mandatory !")]
        public PartDto UpdatedPart { get; set; }
    }
}
