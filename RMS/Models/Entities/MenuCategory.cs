namespace RMS.Models.Entities
{
    public class MenuCategory
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public ICollection<MenuItem> Items { get; set; } = new List<MenuItem>();
    }
}
