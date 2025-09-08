namespace RMS.Models.DTOs.Billing
{
    public class BillPreviewDto
    {
        public int OrderId { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Discount { get; set; }
        public decimal Tax { get; set; }
        public decimal Total { get; set; }
    }
}
