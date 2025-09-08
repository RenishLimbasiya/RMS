    using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RMS.Models.Entities;

namespace RMS.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider sp)
        {
            using var scope = sp.CreateScope();
            var ctx = scope.ServiceProvider.GetRequiredService<RmsDbContext>();
            await ctx.Database.MigrateAsync();

            var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

            // ----------------------------
            // Ensure Roles
            // ----------------------------
            string[] roles = { "Admin", "Waiter", "Kitchen" };
            foreach (var r in roles)
            {
                if (!await roleMgr.RoleExistsAsync(r))
                    await roleMgr.CreateAsync(new IdentityRole(r));
            }

            // ----------------------------
            // Ensure Default Users
            // ----------------------------
            var users = new[]
            {
                new { Email = "admin@rms.com", Password = "Admin@123", FullName = "System Admin", Role = "Admin" },
                new { Email = "waiter@rms.com", Password = "Waiter@123", FullName = "Default Waiter", Role = "Waiter" },
                new { Email = "kitchen@rms.com", Password = "Kitchen@123", FullName = "Kitchen Staff", Role = "Kitchen" }
            };

            foreach (var u in users)
            {
                if (await userMgr.FindByEmailAsync(u.Email) == null)
                {
                    var user = new User
                    {
                        UserName = u.Email,
                        Email = u.Email,
                        FullName = u.FullName
                    };
                    await userMgr.CreateAsync(user, u.Password);
                    await userMgr.AddToRoleAsync(user, u.Role);
                }
            }

            // ----------------------------
            // Seed Tables
            // ----------------------------
            if (!ctx.Tables.Any())
            {
                ctx.Tables.AddRange(
                    new RestaurantTable { Name = "Table 1", Capacity = 4 },
                    new RestaurantTable { Name = "Table 2", Capacity = 4 },
                    new RestaurantTable { Name = "Table 3", Capacity = 2 }
                );
            }

            // ----------------------------
            // Seed Menu Categories + Items
            // ----------------------------
            if (!ctx.MenuCategories.Any())
            {
                var starters = new MenuCategory { Name = "Starters" };
                starters.Items = new List<MenuItem>
                {
                    new MenuItem { Name = "Garlic Bread", Price = 120 },
                    new MenuItem { Name = "French Fries", Price = 150 }
                };

                var mains = new MenuCategory { Name = "Main Course" };
                mains.Items = new List<MenuItem>
                {
                    new MenuItem { Name = "Paneer Butter Masala", Price = 250 },
                    new MenuItem { Name = "Veg Biryani", Price = 220 }
                };

                ctx.MenuCategories.AddRange(starters, mains);
            }

            await ctx.SaveChangesAsync();
        }
    }
}
