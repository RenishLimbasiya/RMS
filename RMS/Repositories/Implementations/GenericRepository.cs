using Microsoft.EntityFrameworkCore;
using RMS.Data;
using RMS.Repositories.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RMS.Repositories.Implementations
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly RmsDbContext _ctx;
        private readonly DbSet<T> _db;

        public GenericRepository(RmsDbContext ctx)
        {
            _ctx = ctx;
            _db = _ctx.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync() => await _db.ToListAsync();
        public async Task<T?> GetByIdAsync(int id) => await _db.FindAsync(id);
        public async Task AddAsync(T entity) => await _db.AddAsync(entity);
        public void Update(T entity) => _db.Update(entity);
        public void Delete(T entity) => _db.Remove(entity);
        public async Task SaveAsync() => await _ctx.SaveChangesAsync();
    }
}
