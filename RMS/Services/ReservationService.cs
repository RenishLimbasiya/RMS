using Microsoft.EntityFrameworkCore;
using RMS.Data;
using RMS.Models.Entities;

namespace RMS.Services
{
    public class ReservationService
    {
        private readonly RmsDbContext _ctx;
        public ReservationService(RmsDbContext ctx) { _ctx = ctx; }

        public async Task<IEnumerable<Reservation>> GetAllAsync() =>
            await _ctx.Reservations.Include(r => r.Table).ToListAsync();

        public async Task<Reservation?> GetAsync(int id) =>
            await _ctx.Reservations.Include(r => r.Table).FirstOrDefaultAsync(r => r.Id == id);

        public async Task<Reservation> AddAsync(Reservation r)
        {
            _ctx.Reservations.Add(r);
            await _ctx.SaveChangesAsync();
            return r;
        }

        public async Task<bool> UpdateAsync(Reservation r)
        {
            _ctx.Reservations.Update(r);
            return await _ctx.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var r = await _ctx.Reservations.FindAsync(id);
            if (r == null) return false;
            _ctx.Reservations.Remove(r);
            return await _ctx.SaveChangesAsync() > 0;
        }
    }
}
