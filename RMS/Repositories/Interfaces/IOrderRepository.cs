using RMS.Models.Entities;

namespace RMS.Repositories.Interfaces
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
        Task<IEnumerable<Order>> GetOrdersWithItemsAsync();
        Task<Order?> GetOrderDetailsAsync(int orderId);
    }
}
