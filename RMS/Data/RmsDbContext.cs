using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RMS.Models.Entities;

namespace RMS.Data
{
    public class RmsDbContext : IdentityDbContext<User>
    {
        public RmsDbContext(DbContextOptions<RmsDbContext> options) : base(options) { }

        public DbSet<RestaurantTable> Tables => Set<RestaurantTable>();
        public DbSet<Reservation> Reservations => Set<Reservation>();
        public DbSet<MenuCategory> MenuCategories => Set<MenuCategory>();
        public DbSet<MenuItem> MenuItems => Set<MenuItem>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderItem> OrderItems => Set<OrderItem>();
        public DbSet<Bill> Bills => Set<Bill>();
        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

        protected override void OnModelCreating(ModelBuilder b)
        {
            base.OnModelCreating(b);

            // Default table status
            b.Entity<RestaurantTable>()
                .Property(t => t.Status)
                .HasDefaultValue("Available");

            // MenuCategory → MenuItems (1:N)
            b.Entity<MenuCategory>()
                .HasMany(c => c.Items)
                .WithOne(i => i.Category)
                .HasForeignKey(i => i.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            // OrderItem → MenuItem (N:1)
            b.Entity<OrderItem>()
                .HasOne(oi => oi.MenuItem)
                .WithMany()
                .HasForeignKey(oi => oi.MenuItemId);

            // OrderItem → Order (N:1)
            b.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.Items)
                .HasForeignKey(oi => oi.OrderId);

            // Bill → Order (1:1)
            b.Entity<Bill>()
                .HasOne(bi => bi.Order)
                .WithOne(o => o.Bill)
                .HasForeignKey<Bill>(bi => bi.OrderId);

            // Precision for money/decimal fields
            b.Entity<Bill>(entity =>
            {
                entity.Property(bi => bi.Subtotal).HasPrecision(18, 2);
                entity.Property(bi => bi.Discount).HasPrecision(18, 2);
                entity.Property(bi => bi.Tax).HasPrecision(18, 2);
                entity.Property(bi => bi.Total).HasPrecision(18, 2);
            });

            b.Entity<MenuItem>(entity =>
            {
                entity.Property(mi => mi.Price).HasPrecision(18, 2);
            });

            b.Entity<Order>(entity =>
            {
                entity.Property(o => o.Discount).HasPrecision(18, 2);
                entity.Property(o => o.TaxPercent).HasPrecision(5, 2);
            });

            b.Entity<OrderItem>(entity =>
            {
                entity.Property(oi => oi.UnitPrice).HasPrecision(18, 2);
            });
        }
    }
}
