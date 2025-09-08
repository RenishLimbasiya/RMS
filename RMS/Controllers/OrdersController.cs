using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RMS.Models.DTOs.Orders;
using RMS.Services;

namespace RMS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly OrderService _svc;
        public OrdersController(OrderService svc) { _svc = svc; }

        // ---------------- CREATE ----------------
        [HttpPost]
        [Authorize(Roles = "Admin,Waiter")]
        public async Task<IActionResult> Create(CreateOrderDto dto)
        {
            var result = await _svc.CreateAsync(dto);
            return Ok(result);
        }

        // ---------------- GET SINGLE ----------------
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Waiter,Kitchen")]
        public async Task<IActionResult> Get(int id)
        {
            var o = await _svc.GetAsync(id);
            return o == null ? NotFound() : Ok(o);
        }

        // ---------------- LIST ALL ----------------
        [HttpGet]
        [Authorize(Roles = "Admin,Waiter")]
        public async Task<IActionResult> List()
        {
            var result = await _svc.ListAsync();
            return Ok(result);
        }

        // ---------------- LIVE ONLY ----------------
        [HttpGet("live")]
        [Authorize(Roles = "Admin,Kitchen")] // ✅ Kitchen can access live orders
        public async Task<IActionResult> Live()
        {
            var result = await _svc.LiveOrdersAsync();
            return Ok(result);
        }

        // ---------------- ITEM STATUS ----------------
        [HttpPost("item/{orderItemId}/ready")]
        [Authorize(Roles = "Kitchen")]
        public async Task<IActionResult> MarkItemReady(int orderItemId) =>
            await _svc.MarkItemReadyAsync(orderItemId) ? Ok() : NotFound();

        // ---------------- BILLING ----------------
        [HttpPost("{orderId}/ready-for-billing")]
        [Authorize(Roles = "Waiter,Admin")]
        public async Task<IActionResult> ReadyForBilling(int orderId) =>
            await _svc.MarkReadyForBillingAsync(orderId) ? Ok() : NotFound();

        // ---------------- ADD ITEMS ----------------
        [HttpPost("{orderId}/items")]
        [Authorize(Roles = "Waiter,Admin")]
        public async Task<IActionResult> AddItems(int orderId, List<OrderItemDto> items)
        {
            var success = await _svc.AddItemsAsync(orderId, items);
            return success ? Ok() : NotFound();
        }

        // ---------------- ADD AS NEW ORDER (NEW) ----------------
        // Creates a brand-new order for the SAME TABLE as {id} and pushes selected items there
        [HttpPost("{id}/add-as-new")]
        [Authorize(Roles = "Waiter,Admin")]
        public async Task<IActionResult> AddAsNew(int id, List<OrderItemDto> items)
        {
            var created = await _svc.AddAsNewOrderAsync(id, items);
            return created == null ? NotFound() : Ok(created);
        }

        // ---------------- CLOSE ORDER ----------------
        [HttpPost("{orderId}/close")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Close(int orderId) =>
            await _svc.CloseOrderAsync(orderId) ? Ok() : NotFound();



        [HttpPost("{orderId}/status")]
        public async Task<IActionResult> SetStatus(int orderId, SetStatusDto dto)
        {
            var ok = await _svc.SetStatusAsync(orderId, dto.Status);
            return ok ? Ok() : NotFound();
        }

    }
}
