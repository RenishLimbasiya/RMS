namespace RMS.Models.Entities
{
    public class Reservation
    {
        public int Id { get; set; }
        public int TableId { get; set; }
        public RestaurantTable? Table { get; set; }
        public string CustomerName { get; set; } = "";
        public string Phone { get; set; } = "";
        public DateTime ReservedAt { get; set; }
        public int Guests { get; set; }
        public string Status { get; set; } = "Booked"; // Booked, Seated, Cancelled
    }
}
