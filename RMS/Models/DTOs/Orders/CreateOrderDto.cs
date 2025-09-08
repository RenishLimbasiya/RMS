namespace RMS.Models.DTOs.Orders
{
    public class CreateOrderDto
    {
        public int TableId { get; set; }
        public decimal Discount { get; set; }
        public decimal TaxPercent { get; set; } = 5;
        public List<OrderItemDto> Items { get; set; } = new();
    }
}
