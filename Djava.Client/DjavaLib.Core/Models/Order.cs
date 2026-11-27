using DjavaLib.Core.Models;
using System;
using System.Collections.Generic;

namespace DjavaLib.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public DateTime CreatedAt { get; set; }

        // Поля из таблицы "Заказы"
        public string PickupMethod { get; set; }
        public string DeliveryAddress { get; set; }
        public decimal TotalAmount { get; set; }

        // ДОБАВЛЕНО: Статус заказа
        public string Status { get; set; } = "Новый";

        public string LastName { get; set; }
        public string FirstName { get; set; }

        public List<OrderItem> Items { get; set; }

        public Order()
        {
            Items = new List<OrderItem>();
        }
    }
}