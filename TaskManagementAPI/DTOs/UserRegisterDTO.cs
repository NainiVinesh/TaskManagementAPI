using System.ComponentModel.DataAnnotations;
namespace TaskManagementAPI.DTOs
{
    public class UserRegisterDTO
    {
        [Required(ErrorMessage = "Username is required")]
        [MinLength(3, ErrorMessage = "Username must be at least 3 characters ")]
        [MaxLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters ")]
        public string Password { get; set; } = string.Empty;

        [Required]
        [Compare("Password", ErrorMessage ="Password do not match")]
        public string confirmPassword { get; set; } = string.Empty;
    }
}
