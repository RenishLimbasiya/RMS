using RMS.Models.Entities;
using RMS.Repositories.Interfaces;

namespace RMS.Services
{
    public class TableService
    {
        private readonly IGenericRepository<RestaurantTable> _repo;
        public TableService(IGenericRepository<RestaurantTable> repo) { _repo = repo; }

        public Task<IEnumerable<RestaurantTable>> GetAllAsync() => _repo.GetAllAsync();
        public Task<RestaurantTable?> GetAsync(int id) => _repo.GetByIdAsync(id);

        public async Task<RestaurantTable> AddAsync(RestaurantTable t)
        {
            await _repo.AddAsync(t);
            await _repo.SaveAsync();
            return t;
        }

        public async Task<bool> UpdateAsync(RestaurantTable t)
        {
            _repo.Update(t);
            await _repo.SaveAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var t = await _repo.GetByIdAsync(id);
            if (t == null) return false;
            _repo.Delete(t);
            await _repo.SaveAsync();
            return true;
        }
    }
}
