namespace RMS.Models.DTOs.Billing
{
    public class BillFinalizeDto
    {
        public int OrderId { get; set; }
        public string PaymentType { get; set; } = "Cash";
    }
}
