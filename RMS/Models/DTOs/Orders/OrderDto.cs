namespace RMS.Models.DTOs.Orders
{
    public class OrderDto
    {
        public int Id { get; set; }
        public string Status { get; set; }
        public int TableId { get; set; }
        public string TableName { get; set; }

        public List<OrderItemDto> Items { get; set; } = new();
    }
}
