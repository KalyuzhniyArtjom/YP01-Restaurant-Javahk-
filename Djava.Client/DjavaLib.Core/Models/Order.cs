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

        public string PickupMethod { get; set; }
        public string DeliveryAddress { get; set; }
        public decimal TotalAmount { get; set; }

        // ДОБАВЛЕНО: Телефон клиента
        public string Phone { get; set; }

        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Status { get; set; } = "Новый";

        public List<OrderItem> Items { get; set; }

        public Order()
        {
            Items = new List<OrderItem>();
        }
    }
}