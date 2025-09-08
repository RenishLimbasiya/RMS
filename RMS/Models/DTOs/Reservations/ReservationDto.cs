namespace RMS.Models.DTOs.Reservations
{
    public class ReservationDto
    {
        public int Id { get; set; }
        public int TableId { get; set; }
        public string CustomerName { get; set; } = "";
        public string Phone { get; set; } = "";
        public DateTime ReservedAt { get; set; }
        public int Guests { get; set; }
        public string Status { get; set; } = "Booked";
    }
}
