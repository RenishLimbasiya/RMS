namespace RMS.Models.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public int TableId { get; set; }
        public RestaurantTable? Table { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = "Pending"; // Pending, InKitchen, Ready, Billed

        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();

        public decimal Discount { get; set; } // absolute amount
        public decimal TaxPercent { get; set; } = 5;
        public bool ReadyForBilling { get; set; } = false;

        // 🔹 Navigation property for Bill (1:1 relationship)
        public Bill? Bill { get; set; }
    }
}
