using Microsoft.EntityFrameworkCore;
using RMS.Data;
using RMS.Models.Entities;

namespace RMS.Services
{
    public class MenuService
    {
        private readonly RmsDbContext _ctx;
        public MenuService(RmsDbContext ctx) { _ctx = ctx; }

        // ---------------- CATEGORIES ----------------
        public async Task<IEnumerable<MenuCategory>> GetCategoriesAsync() =>
            await _ctx.MenuCategories
                      .Include(c => c.Items)
                      .ToListAsync();

        public async Task<MenuCategory?> GetCategoryAsync(int id) =>
            await _ctx.MenuCategories
                      .Include(c => c.Items)
                      .FirstOrDefaultAsync(c => c.Id == id);

        public async Task<MenuCategory> AddCategoryAsync(MenuCategory c)
        {
            _ctx.MenuCategories.Add(c);
            await _ctx.SaveChangesAsync();
            return c;
        }

        public async Task<bool> UpdateCategoryAsync(MenuCategory c)
        {
            if (!_ctx.MenuCategories.Any(x => x.Id == c.Id)) return false;
            _ctx.MenuCategories.Update(c);
            return await _ctx.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var c = await _ctx.MenuCategories.FindAsync(id);
            if (c == null) return false;
            _ctx.MenuCategories.Remove(c);
            return await _ctx.SaveChangesAsync() > 0;
        }

        // ---------------- ITEMS ----------------
        public async Task<IEnumerable<MenuItem>> GetItemsAsync() =>
            await _ctx.MenuItems
                      .Include(i => i.Category)
                      .ToListAsync();

        public async Task<MenuItem?> GetItemAsync(int id) =>
            await _ctx.MenuItems
                      .Include(i => i.Category)
                      .FirstOrDefaultAsync(i => i.Id == id);

        public async Task<MenuItem> AddItemAsync(MenuItem i)
        {
            _ctx.MenuItems.Add(i);
            await _ctx.SaveChangesAsync();
            return i;
        }

        public async Task<bool> UpdateItemAsync(MenuItem i)
        {
            if (!_ctx.MenuItems.Any(x => x.Id == i.Id)) return false;
            _ctx.MenuItems.Update(i);
            return await _ctx.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteItemAsync(int id)
        {
            var i = await _ctx.MenuItems.FindAsync(id);
            if (i == null) return false;
            _ctx.MenuItems.Remove(i);
            return await _ctx.SaveChangesAsync() > 0;
        }
    }
}
