namespace RMS.Models.Entities
{
    public class MenuItem
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public MenuCategory? Category { get; set; }
        public string Name { get; set; } = "";
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public bool IsAvailable { get; set; } = true;
    }
}
