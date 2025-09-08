namespace RMS.Models.DTOs.Users
{
    public class UpdateUserDto
    {
        public string FullName { get; set; } = string.Empty;

        // Nullable: update only if provided
        public string? Password { get; set; }

        // Role update (default Waiter if nothing provided)
        public string Role { get; set; } = "Waiter";
    }
}
