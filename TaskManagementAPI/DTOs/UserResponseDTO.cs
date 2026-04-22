namespace TaskManagementAPI.DTOs
{
    public class UserResponseDTO
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public DateTime CreatedAt  { get; set; }
        public string Token { get; set; } = string.Empty;//for jwt after login

    }
}
