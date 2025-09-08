namespace RMS.Models.DTOs.Billing
{
    public class BillDto
    {
        public int Id { get; set; }
        public string BillNumber { get; set; } = "";
        public int OrderId { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Discount { get; set; }
        public decimal Tax { get; set; }
        public decimal Total { get; set; }
        public string PaymentType { get; set; } = "Cash";
        public DateTime CreatedAt { get; set; }
    }
}
