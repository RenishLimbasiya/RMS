using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RMS.Models.Entities;
using RMS.Services;

namespace RMS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ReservationsController : ControllerBase
    {
        private readonly ReservationService _svc;
        public ReservationsController(ReservationService svc) { _svc = svc; }

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _svc.GetAllAsync());

        [HttpPost]
        public async Task<IActionResult> Create(Reservation r) => Ok(await _svc.AddAsync(r));

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Reservation r)
        {
            r.Id = id;
            return await _svc.UpdateAsync(r) ? Ok() : NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
            => await _svc.DeleteAsync(id) ? Ok() : NotFound();
    }
}
