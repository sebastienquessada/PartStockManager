using System.ComponentModel.DataAnnotations;

namespace PartStockManager.API.DTOs
{
    public class UserDeletionRequest
    {
        [Required]
        public string Username { get; set; }
    }
}
