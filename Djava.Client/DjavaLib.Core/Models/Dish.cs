namespace DjavaLib.Models
{
    public class Dish
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Composition { get; set; }
        public decimal Price { get; set; }
        public decimal? Weight { get; set; }
        public bool IsAvailable { get; set; }
    }
}