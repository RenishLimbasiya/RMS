namespace RMS.Models.DTOs.Tables
{
    public class TableDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public int Capacity { get; set; }
        public string Status { get; set; } = "Available";
    }
}
