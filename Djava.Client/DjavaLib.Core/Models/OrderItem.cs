namespace DjavaLib.Models
{
    public class OrderItem
    {
        public int DishId { get; set; }
        public string DishName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }

        public decimal Sum
        {
            get { return Price * Quantity; }
        }
    }
}