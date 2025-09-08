using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RMS.Models.DTOs.Auth;
using RMS.Services;

namespace RMS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _auth;

        public AuthController(AuthService auth) { _auth = auth; }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var res = await _auth.RegisterAsync(dto);
            if (!res.Succeeded) return BadRequest(res.Errors);
            return Ok();
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var token = await _auth.LoginAsync(dto);
            if (token == null) return Unauthorized("Invalid credentials");
            return Ok(new { token });
        }
    }
}
