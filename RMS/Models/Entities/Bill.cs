namespace RMS.Models.Entities
{
    public class Bill
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public Order? Order { get; set; }

        public decimal Subtotal { get; set; }
        public decimal Discount { get; set; }
        public decimal Tax { get; set; }
        public decimal Total { get; set; }

        public string PaymentType { get; set; } = "Cash"; // Cash, Card, UPI
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? BillNumber { get; set; }
    }
}
