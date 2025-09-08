using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RMS.Models.DTOs.Auth;
using RMS.Models.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RMS.Services
{
    public class AuthService
    {
        private readonly UserManager<User> _userMgr;
        private readonly RoleManager<IdentityRole> _roleMgr;
        private readonly IConfiguration _cfg;

        public AuthService(UserManager<User> userMgr, RoleManager<IdentityRole> roleMgr, IConfiguration cfg)
        {
            _userMgr = userMgr;
            _roleMgr = roleMgr;
            _cfg = cfg;
        }

        public async Task<IdentityResult> RegisterAsync(RegisterDto dto)
        {
            var user = new User
            {
                FullName = dto.FullName,
                Email = dto.Email,
                UserName = dto.Email
            };

            var result = await _userMgr.CreateAsync(user, dto.Password);
            if (!result.Succeeded) return result;

            if (!await _roleMgr.RoleExistsAsync(dto.Role))
                await _roleMgr.CreateAsync(new IdentityRole(dto.Role));

            await _userMgr.AddToRoleAsync(user, dto.Role);
            return result;
        }

        public async Task<string?> LoginAsync(LoginDto dto)
        {
            var user = await _userMgr.FindByEmailAsync(dto.Email);

            if (user == null)
            {
                Console.WriteLine($"[AuthService] Login failed → User not found: {dto.Email}");
                return null;
            }

            var valid = await _userMgr.CheckPasswordAsync(user, dto.Password);
            if (!valid)
            {
                Console.WriteLine($"[AuthService] Login failed → Invalid password for {dto.Email}");
                return null;
            }

            var roles = await _userMgr.GetRolesAsync(user);
            Console.WriteLine($"[AuthService] Login success → {user.Email} Roles: {string.Join(",", roles)}");

            return GenerateJwt(user, roles);
        }

        private string GenerateJwt(User user, IList<string> roles)
        {
            var jwt = _cfg.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim("id", user.Id),
                new Claim("email", user.Email ?? ""),
                new Claim("fullName", user.FullName ?? "") // ✅ Added FullName
            };

            foreach (var r in roles)
            {
                claims.Add(new Claim("role", r));
            }

            var token = new JwtSecurityToken(
                issuer: jwt["Issuer"],
                audience: jwt["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(double.Parse(jwt["ExpireMinutes"] ?? "120")),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
