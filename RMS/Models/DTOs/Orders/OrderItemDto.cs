namespace RMS.Models.DTOs.Orders
{
    public class OrderItemDto
    {
        public int MenuItemId { get; set; }
        public string Name { get; set; }   // ✅ This is filled from MenuItem.Name
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
    }
}
