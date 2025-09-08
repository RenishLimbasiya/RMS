using Microsoft.EntityFrameworkCore;
using RMS.Data;
using RMS.Models.Entities;
using RMS.Repositories.Interfaces;

namespace RMS.Repositories.Implementations
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        private readonly RmsDbContext _context;

        public OrderRepository(RmsDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Order>> GetOrdersWithItemsAsync()
        {
            return await _context.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.MenuItem)
                .ToListAsync();
        }

        public async Task<Order?> GetOrderDetailsAsync(int orderId)
        {
            return await _context.Orders
                .Include(o => o.Table)
                .Include(o => o.Items).ThenInclude(i => i.MenuItem)
                .FirstOrDefaultAsync(o => o.Id == orderId);
        }
    }
}
