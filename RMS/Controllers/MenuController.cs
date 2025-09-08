using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RMS.Models.Entities;
using RMS.Services;

namespace RMS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MenuController : ControllerBase
    {
        private readonly MenuService _svc;
        public MenuController(MenuService svc) { _svc = svc; }

        // ---------- Categories ----------
        [AllowAnonymous]
        [HttpGet("categories")]
        public async Task<IActionResult> Categories() =>
            Ok(await _svc.GetCategoriesAsync());

        [Authorize(Roles = "Admin")]
        [HttpPost("categories")]
        public async Task<IActionResult> AddCategory([FromBody] MenuCategory c) =>
            Ok(await _svc.AddCategoryAsync(c));

        [Authorize(Roles = "Admin")]
        [HttpPut("categories/{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] MenuCategory c)
        {
            c.Id = id;
            return await _svc.UpdateCategoryAsync(c) ? Ok() : NotFound();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("categories/{id}")]
        public async Task<IActionResult> DeleteCategory(int id) =>
            await _svc.DeleteCategoryAsync(id) ? Ok() : NotFound();

        // ---------- Items ----------
        [AllowAnonymous]
        [HttpGet("items")]
        public async Task<IActionResult> Items() =>
            Ok(await _svc.GetItemsAsync());

        [AllowAnonymous]
        [HttpGet("items/{id}")]
        public async Task<IActionResult> Item(int id)
        {
            var item = await _svc.GetItemAsync(id);
            return item is null ? NotFound() : Ok(item);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("items")]
        public async Task<IActionResult> AddItem([FromBody] MenuItem i) =>
            Ok(await _svc.AddItemAsync(i));

        [Authorize(Roles = "Admin")]
        [HttpPut("items/{id}")]
        public async Task<IActionResult> UpdateItem(int id, [FromBody] MenuItem i)
        {
            i.Id = id;
            return await _svc.UpdateItemAsync(i) ? Ok() : NotFound();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("items/{id}")]
        public async Task<IActionResult> DeleteItem(int id) =>
            await _svc.DeleteItemAsync(id) ? Ok() : NotFound();
    }
}
