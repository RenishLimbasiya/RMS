using Microsoft.AspNetCore.Identity;

namespace RMS.Models.Entities
{
    public class User : IdentityUser
    {
        public string FullName { get; set; } = "";
    }
}
