using Microsoft.EntityFrameworkCore;
using RMS.Data;
using RMS.Models.Entities;
using RMS.Repositories.Interfaces;

namespace RMS.Repositories.Implementations
{
    public class TableRepository : GenericRepository<RestaurantTable>, ITableRepository
    {
        private readonly RmsDbContext _context;

        public TableRepository(RmsDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<RestaurantTable>> GetAvailableTablesAsync()
        {
            return await _context.Tables
                .Where(t => t.Status == "Available")
                .ToListAsync();
        }
    }
}
