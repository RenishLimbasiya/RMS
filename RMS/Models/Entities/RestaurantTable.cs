namespace RMS.Models.Entities
{
    public class RestaurantTable
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public int Capacity { get; set; }
        public string Status { get; set; } = "Available"; // Available, Occupied, Reserved
    }
}
