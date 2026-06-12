using System.ComponentModel.DataAnnotations;

namespace PartStockManager.API.DTOs
{
    public class UserModificationRequest
    {
        [Required]
        public string CurrentUsername { get; set; }

        [Required]
        public UserDto UpdatedUser { get; set; }
    }
}
