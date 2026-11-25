namespace DjavaLib.Models
{
    public class CartItem
    {
        public int CartId { get; set; }
        public int DishId { get; set; }
        public string DishName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string PhotoPath { get; set; }

        // Вычисляемая сумма по позиции
        public decimal TotalPrice => Price * Quantity;
    }
}