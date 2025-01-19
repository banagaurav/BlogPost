using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string FullName { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; }

        [Required]
        [StringLength(100)]
        public string Password { get; set; }

        [Required]
        [EmailAddress]
        public string Gmail { get; set; }

        [Required]
        [StringLength(10)]
        public string Role { get; set; } = "User"; // Default role

        // Navigation property for Notifications
        public ICollection<Notification> Notifications { get; set; }
    }
}
