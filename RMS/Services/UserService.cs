using Microsoft.AspNetCore.Identity;
using RMS.Models.DTOs.Users;
using RMS.Models.Entities;

namespace RMS.Services
{
    public class UserService
    {
        private readonly UserManager<User> _userMgr;
        private readonly RoleManager<IdentityRole> _roleMgr;

        public UserService(UserManager<User> userMgr, RoleManager<IdentityRole> roleMgr)
        {
            _userMgr = userMgr;
            _roleMgr = roleMgr;
        }

        public async Task<IEnumerable<UserDto>> GetAllAsync()
        {
            var users = _userMgr.Users.ToList();
            var list = new List<UserDto>();

            foreach (var u in users)
            {
                var roles = await _userMgr.GetRolesAsync(u);
                list.Add(new UserDto
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Email = u.Email ?? "",
                    Roles = roles
                });
            }

            return list;
        }

        public async Task<UserDto?> GetByIdAsync(string id)
        {
            var u = await _userMgr.FindByIdAsync(id);
            if (u == null) return null;

            var roles = await _userMgr.GetRolesAsync(u);

            return new UserDto
            {
                Id = u.Id,
                FullName = u.FullName,
                Email = u.Email ?? "",
                Roles = roles
            };
        }

        public async Task<bool> CreateAsync(CreateUserDto dto)
        {
            var u = new User
            {
                FullName = dto.FullName,
                Email = dto.Email,
                UserName = dto.Email
            };

            var res = await _userMgr.CreateAsync(u, dto.Password);
            if (!res.Succeeded) return false;

            if (!await _roleMgr.RoleExistsAsync(dto.Role))
                await _roleMgr.CreateAsync(new IdentityRole(dto.Role));

            await _userMgr.AddToRoleAsync(u, dto.Role);
            return true;
        }

        public async Task<bool> UpdateAsync(string id, UpdateUserDto dto)
        {
            var u = await _userMgr.FindByIdAsync(id);
            if (u == null) return false;

            u.FullName = dto.FullName;

            if (!string.IsNullOrWhiteSpace(dto.Password))
            {
                var token = await _userMgr.GeneratePasswordResetTokenAsync(u);
                var passRes = await _userMgr.ResetPasswordAsync(u, token, dto.Password);
                if (!passRes.Succeeded) return false;
            }

            // Reset roles
            var oldRoles = await _userMgr.GetRolesAsync(u);
            await _userMgr.RemoveFromRolesAsync(u, oldRoles);

            if (!await _roleMgr.RoleExistsAsync(dto.Role))
                await _roleMgr.CreateAsync(new IdentityRole(dto.Role));

            await _userMgr.AddToRoleAsync(u, dto.Role);

            var res = await _userMgr.UpdateAsync(u);
            return res.Succeeded;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var u = await _userMgr.FindByIdAsync(id);
            if (u == null) return false;

            var res = await _userMgr.DeleteAsync(u);
            return res.Succeeded;
        }
    }
}
