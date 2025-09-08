using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RMS.Models.DTOs.Billing;
using RMS.Services;

namespace RMS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class BillingController : ControllerBase
    {
        private readonly BillingService _svc;
        public BillingController(BillingService svc) { _svc = svc; }

        [HttpGet("preview/{orderId}")]
        public async Task<IActionResult> Preview(int orderId)
        {
            var p = await _svc.PreviewAsync(orderId);
            return p == null ? NotFound() : Ok(p);
        }

        [HttpPost("finalize")]
        public async Task<IActionResult> Finalize(BillFinalizeDto dto)
        {
            var b = await _svc.FinalizeAsync(dto);
            return b == null ? BadRequest("Order not found") : Ok(b);
        }

        [HttpGet("history")]
        public async Task<IActionResult> History()
        {
            var bills = await _svc.GetHistoryAsync();
            return Ok(bills);
        }

        [HttpGet("{billId}/pdf")]
        public async Task<IActionResult> ExportPdf(int billId)
        {
            var bytes = await _svc.ExportBillPdfAsync(billId);
            if (bytes == null) return NotFound();
            return File(bytes, "application/pdf", $"bill-{billId}.pdf");
        }


        [HttpGet("preview/{orderId}/pdf")]
        public async Task<IActionResult> PreviewPdf(int orderId)
        {
            var bytes = await _svc.ExportPreviewPdfAsync(orderId);
            if (bytes == null) return NotFound();
            return File(bytes, "application/pdf", $"bill-preview-{orderId}.pdf");
        }

    }
}
