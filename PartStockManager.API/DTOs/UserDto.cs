using PartStockManager.CoreLogic.Models;
using System.ComponentModel.DataAnnotations;

namespace PartStockManager.API.DTOs
{
    public class UserDto
    {
        [Required]
        [MaxLength(100)]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public UserProfile Profile { get; set; }
    }
}
