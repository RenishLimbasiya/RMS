using RMS.Models.Entities;

namespace RMS.Repositories.Interfaces
{
    public interface ITableRepository : IGenericRepository<RestaurantTable>
    {
        Task<IEnumerable<RestaurantTable>> GetAvailableTablesAsync();
    }
}
