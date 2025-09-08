namespace RMS.Models.DTOs.Users
{
    public class CreateUserDto
    {
        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        // Default role is "Waiter", but Admin can override
        public string Role { get; set; } = "Waiter";
    }
}
