using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RMS.Data;

namespace RMS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")] // keep this secure
    public class DashboardController : ControllerBase
    {
        private readonly RmsDbContext _ctx;
        public DashboardController(RmsDbContext ctx) { _ctx = ctx; }

        [HttpGet("summary")]
        public async Task<IActionResult> Summary()
        {
            var tables = await _ctx.Tables.CountAsync();
            var categories = await _ctx.MenuCategories.CountAsync();
            var items = await _ctx.MenuItems.CountAsync();
            var billings = await _ctx.Bills.CountAsync();

            var recentBills = await _ctx.Bills
                .OrderByDescending(b => b.Id)
                .Take(5)
                .Select(b => new { b.Id, b.OrderId, b.Total, b.CreatedAt })
                .ToListAsync();

            return Ok(new { tables, categories, items, billings, recentBills });
        }
    }
}
