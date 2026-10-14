using DjavaLib.Models;
using Npgsql;
using System;
using System.Collections.Generic;

namespace DjavaLib.Data
{
    public class OrderListRepository : IOrderListRepository
    {
        private readonly DbContext _context;

        public OrderListRepository(DbContext context)
        {
            _context = context;
        }

        public List<Order> GetOrders()
        {
            var result = new List<Order>();

            using (var conn = _context.CreateConnection())
            {
                conn.Open();

                var sql = @"
                    SELECT з.""ID_Заказа"",
                           з.""Дата_Оформления"",
                           к.""Фамилия"" || ' ' || к.""Имя"",
                           COALESCE(к.""Телефон"", к.""Email"", ''),
                           з.""Способ_Получения"",
                           ст.""Наименование"",
                           COALESCE(SUM(пз.""Сумма""), 0)
                    FROM ""Заказы"" з
                    JOIN ""Клиенты"" к ON з.""ID_Клиента"" = к.""ID_Клиента""
                    JOIN ""Статусы_Заказа"" ст ON з.""ID_Статуса"" = ст.""ID_Статуса""
                    LEFT JOIN ""Позиции_Заказа"" пз ON з.""ID_Заказа"" = пз.""ID_Заказа""
                    GROUP BY з.""ID_Заказа"", з.""Дата_Оформления"", к.""Фамилия"", к.""Имя"",
                             к.""Телефон"", к.""Email"", з.""Способ_Получения"", ст.""Наименование""
                    ORDER BY з.""Дата_Оформления"" DESC";

                using (var cmd = new NpgsqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(new Order
                        {
                            OrderId = reader.GetInt32(0),
                            OrderDate = reader.GetDateTime(1),
                            ClientName = reader.GetString(2),
                            ClientContact = reader.GetString(3),
                            DeliveryMethod = ParseDeliveryType(reader.GetString(4)),
                            Status = ParseOrderStatus(reader.GetString(5)),
                            TotalAmount = reader.GetDecimal(6)
                        });
                    }
                }
            }

            return result;
        }

        public List<OrderItem> GetOrderItems(int orderId)
        {
            var result = new List<OrderItem>();

            using (var conn = _context.CreateConnection())
            {
                conn.Open();

                var sql = @"
                    SELECT б.""ID_Блюда"",
                           б.""Наименование"",
                           пз.""Количество"",
                           пз.""Цена_За_Единицу""
                    FROM ""Позиции_Заказа"" пз
                    JOIN ""Блюда"" б ON пз.""ID_Блюда"" = б.""ID_Блюда""
                    WHERE пз.""ID_Заказа"" = @orderId";

                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@orderId", orderId);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(new OrderItem
                            {
                                DishId = reader.GetInt32(0),
                                DishName = reader.GetString(1),
                                Quantity = reader.GetInt32(2),
                                Price = reader.GetDecimal(3)
                            });
                        }
                    }
                }
            }

            return result;
        }

        public bool UpdateOrderStatus(int orderId, OrderStatus status)
        {
            using (var conn = _context.CreateConnection())
            {
                conn.Open();

                var sql = "UPDATE \"Заказы\" SET \"ID_Статуса\" = @statusId WHERE \"ID_Заказа\" = @orderId";

                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@statusId", GetStatusId(status));
                    cmd.Parameters.AddWithValue("@orderId", orderId);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        private DeliveryType ParseDeliveryType(string value)
        {
            if (value == "Доставка") return DeliveryType.Delivery;
            return DeliveryType.Pickup;
        }

        private OrderStatus ParseOrderStatus(string value)
        {
            switch (value)
            {
                case "Новый": return OrderStatus.New;
                case "Принят": return OrderStatus.Accepted;
                case "Готовится": return OrderStatus.Cooking;
                case "Готов": return OrderStatus.Ready;
                case "Доставляется": return OrderStatus.Delivering;
                case "Выполнен": return OrderStatus.Completed;
                case "Отменён": return OrderStatus.Cancelled;
                default: return OrderStatus.New;
            }
        }

        private int GetStatusId(OrderStatus status)
        {
            switch (status)
            {
                case OrderStatus.New: return 1;
                case OrderStatus.Accepted: return 2;
                case OrderStatus.Cooking: return 3;
                case OrderStatus.Ready: return 4;
                case OrderStatus.Delivering: return 5;
                case OrderStatus.Completed: return 6;
                case OrderStatus.Cancelled: return 7;
                default: return 1; 
            }
        }
    }
}