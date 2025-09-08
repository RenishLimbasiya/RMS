using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RMS.Models.Entities;
using RMS.Services;

namespace RMS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TablesController : ControllerBase
    {
        private readonly TableService _svc;
        public TablesController(TableService svc) { _svc = svc; }

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _svc.GetAllAsync());

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(RestaurantTable t) => Ok(await _svc.AddAsync(t));

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, RestaurantTable t)
        {
            t.Id = id;
            return await _svc.UpdateAsync(t) ? Ok() : NotFound();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
            => await _svc.DeleteAsync(id) ? Ok() : NotFound();
    }
}
