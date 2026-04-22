using System.ComponentModel.DataAnnotations;

namespace TaskManagementAPI.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;


        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = "User";// admin or user

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<TaskItem>? AssignedTasks { get; set; }
        
        public ICollection<TaskItem>? CreatedBy { get; set; }

    }
}
