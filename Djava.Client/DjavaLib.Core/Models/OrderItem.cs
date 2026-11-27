using System;
using System.Collections.Generic;
using System.Text;

namespace DjavaLib.Core.Models
{
    public class OrderItem
    {
        public int Id { get; set; }

        public int OrderId { get; set; }

        public int DishId { get; set; }

        public string DishName { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }
    }
}
