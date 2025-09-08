using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RMS.Data;
using RMS.Models.Entities;
using RMS.Models.DTOs.Users;   // 👈 use your DTO
using RMS.Repositories.Interfaces;

namespace RMS.Repositories.Implementations
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        private readonly RmsDbContext _context;
        private readonly UserManager<User> _userManager;

        public UserRepository(RmsDbContext context, UserManager<User> userManager) : base(context)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<UserDto?> GetUserWithRolesAsync(string userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return null;

            var roles = await _userManager.GetRolesAsync(user);

            return new UserDto
            {
                Id = user.Id,
                FullName = user.UserName ?? "",   // 👈 you can change to FirstName + LastName if available
                Email = user.Email ?? "",
                Roles = roles.ToList()
            };
        }
    }
}
