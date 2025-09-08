using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RMS.Models.DTOs.Users;
using RMS.Services;

namespace RMS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class UsersController : ControllerBase
    {
        private readonly UserService _svc;
        public UsersController(UserService svc) { _svc = svc; }

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _svc.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var u = await _svc.GetByIdAsync(id);
            return u == null ? NotFound() : Ok(u);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateUserDto dto)
            => await _svc.CreateAsync(dto) ? Ok() : BadRequest();

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, UpdateUserDto dto)
            => await _svc.UpdateAsync(id, dto) ? Ok() : NotFound();

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
            => await _svc.DeleteAsync(id) ? Ok() : NotFound();
    }
}
