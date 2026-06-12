using PartStockManager.CoreLogic.Models;
using System.ComponentModel.DataAnnotations;

namespace PartStockManager.Adapter.Database.Entities
{
    public class UserEntity
    {
        [Key]
        [MaxLength(100)]
        public string Username { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [Required]
        public UserProfile Profile { get; set; }
    }
}
