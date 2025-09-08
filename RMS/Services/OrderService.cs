using Microsoft.EntityFrameworkCore;
using RMS.Data;
using RMS.Models.DTOs.Orders;
using RMS.Models.Entities;

namespace RMS.Services
{
    public class OrderService
    {
        private readonly RmsDbContext _ctx;
        public OrderService(RmsDbContext ctx) { _ctx = ctx; }

        // ---------------- CREATE ----------------
        public async Task<OrderDto> CreateAsync(CreateOrderDto dto)
        {
            var order = new Order
            {
                TableId = dto.TableId,
                Discount = dto.Discount,
                TaxPercent = dto.TaxPercent,
                Status = "Pending"
            };
            _ctx.Orders.Add(order);
            await _ctx.SaveChangesAsync();

            foreach (var it in dto.Items)
            {
                _ctx.OrderItems.Add(new OrderItem
                {
                    OrderId = order.Id,
                    MenuItemId = it.MenuItemId,
                    Quantity = it.Quantity,
                    UnitPrice = it.UnitPrice,
                    Status = "Queued"
                });
            }
            await _ctx.SaveChangesAsync();

            return await GetAsync(order.Id) ?? throw new Exception("Order creation failed");
        }

        // ---------------- GET SINGLE ----------------
        public async Task<OrderDto?> GetAsync(int id)
        {
            var order = await _ctx.Orders
                .Include(o => o.Table)
                .Include(o => o.Items).ThenInclude(i => i.MenuItem)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return null;

            return new OrderDto
            {
                Id = order.Id,
                Status = order.Status,
                TableId = order.TableId,
                TableName = order.Table?.Name ?? "",
                Items = order.Items.Select(i => new OrderItemDto
                {
                    MenuItemId = i.MenuItemId,
                    Name = i.MenuItem.Name,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList()
            };
        }

        // ---------------- LIST ALL ----------------
        public async Task<IEnumerable<OrderDto>> ListAsync()
        {
            var orders = await _ctx.Orders
                .Include(o => o.Table)
                .Include(o => o.Items).ThenInclude(i => i.MenuItem)
                .OrderByDescending(o => o.Id)
                .ToListAsync();

            return orders.Select(order => new OrderDto
            {
                Id = order.Id,
                Status = order.Status,
                TableId = order.TableId,
                TableName = order.Table?.Name ?? "",
                Items = order.Items.Select(i => new OrderItemDto
                {
                    MenuItemId = i.MenuItemId,
                    Name = i.MenuItem.Name,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList()
            });
        }

        // ---------------- LIVE ORDERS ----------------
        public async Task<IEnumerable<OrderDto>> LiveOrdersAsync()
        {
            var orders = await _ctx.Orders
                .Include(o => o.Table)
                .Include(o => o.Items).ThenInclude(i => i.MenuItem)
                .Where(o => o.Status != "Billed")
                .ToListAsync();

            return orders.Select(order => new OrderDto
            {
                Id = order.Id,
                Status = order.Status,
                TableId = order.TableId,
                TableName = order.Table?.Name ?? "",
                Items = order.Items.Select(i => new OrderItemDto
                {
                    MenuItemId = i.MenuItemId,
                    Name = i.MenuItem.Name,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList()
            });
        }

        // ---------------- ITEM STATUS ----------------
        public async Task<bool> MarkItemReadyAsync(int orderItemId)
        {
            var item = await _ctx.OrderItems.FindAsync(orderItemId);
            if (item == null) return false;

            item.Status = "Ready";
            await _ctx.SaveChangesAsync();

            var allReady = await _ctx.OrderItems
                .Where(i => i.OrderId == item.OrderId)
                .AllAsync(i => i.Status == "Ready");

            if (allReady)
            {
                var order = await _ctx.Orders.FindAsync(item.OrderId);
                if (order != null)
                {
                    order.Status = "Ready";
                    await _ctx.SaveChangesAsync();
                }
            }
            return true;
        }

        // ---------------- BILLING ----------------
        public async Task<bool> MarkReadyForBillingAsync(int orderId)
        {
            var order = await _ctx.Orders.FindAsync(orderId);
            if (order == null) return false;

            order.Status = "ReadyForBilling";
            await _ctx.SaveChangesAsync();
            return true;
        }

        // ---------------- ADD ITEMS TO EXISTING ORDER ----------------
        public async Task<bool> AddItemsAsync(int orderId, IEnumerable<OrderItemDto> items)
        {
            var order = await _ctx.Orders.FindAsync(orderId);
            if (order == null || order.Status == "Billed") return false;

            foreach (var it in items)
            {
                _ctx.OrderItems.Add(new OrderItem
                {
                    OrderId = orderId,
                    MenuItemId = it.MenuItemId,
                    Quantity = it.Quantity,
                    UnitPrice = it.UnitPrice,
                    Status = "Queued"
                });
            }
            order.Status = "Pending";
            await _ctx.SaveChangesAsync();
            return true;
        }

        // ---------------- ADD AS NEW ORDER (NEW) ----------------
        // Creates a NEW order for the SAME table as sourceOrderId, and attaches the given items
        public async Task<OrderDto?> AddAsNewOrderAsync(int sourceOrderId, IEnumerable<OrderItemDto> items)
        {
            var source = await _ctx.Orders.AsNoTracking().FirstOrDefaultAsync(o => o.Id == sourceOrderId);
            if (source == null || source.Status == "Billed") return null;

            var newOrder = new Order
            {
                TableId = source.TableId,
                Discount = 0,
                TaxPercent = source.TaxPercent,
                Status = "Pending"
            };

            _ctx.Orders.Add(newOrder);
            await _ctx.SaveChangesAsync();

            foreach (var it in items)
            {
                _ctx.OrderItems.Add(new OrderItem
                {
                    OrderId = newOrder.Id,
                    MenuItemId = it.MenuItemId,
                    Quantity = it.Quantity,
                    UnitPrice = it.UnitPrice,
                    Status = "Queued"
                });
            }
            await _ctx.SaveChangesAsync();

            return await GetAsync(newOrder.Id);
        }

        // ---------------- CLOSE ORDER ----------------
        public async Task<bool> CloseOrderAsync(int orderId)
        {
            var order = await _ctx.Orders
                .Include(o => o.Table)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null) return false;

            order.Status = "Billed"; // closed
            if (order.Table != null)
                order.Table.Status = "Available"; // free table

            await _ctx.SaveChangesAsync();
            return true;
        }


        public async Task<bool> SetStatusAsync(int orderId, string status)
        {
            var order = await _ctx.Orders.FindAsync(orderId);
            if (order == null) return false;
            order.Status = status;
            await _ctx.SaveChangesAsync();
            return true;
        }

    }
}
