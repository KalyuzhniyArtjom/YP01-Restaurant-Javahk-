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

        public List<Order> GetOrders(OrderFilter filter)
        {
            var result = new List<Order>();
            using (var conn = _context.CreateConnection())
            {
                conn.Open();
                var sql = @"
                    SELECT з.""ID_Заказа"", 
                           з.""Дата_Оформления"", 
                           з.""Фамилия"" || ' ' || з.""Имя"",
                           COALESCE(з.""Телефон"", ''),
                           з.""Способ_Получения"", 
                           ст.""Наименование"", 
                           з.""Сумма_Заказа""
                    FROM ""Заказы"" з
                    JOIN ""Статусы_Заказа"" ст ON з.""ID_Статуса"" = ст.""ID_Статуса""";

                if (filter != null && filter.Status != null) sql += " WHERE з.\"ID_Статуса\" = @statusId";
                sql += " ORDER BY з.\"ID_Заказа\" ASC";

                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    if (filter != null && filter.Status != null) cmd.Parameters.AddWithValue("@statusId", GetStatusId(filter.Status.Value));
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
                    SELECT б.""ID_Блюда"", б.""Наименование"", пз.""Количество"", пз.""Цена_За_Единицу""
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

        public bool UpdateOrderDetails(int orderId, string clientName, string clientContact, DeliveryType delivery, OrderStatus status)
        {
            string[] nameParts = clientName.Trim().Split(new char[] { ' ' }, 2);
            string lastName = nameParts.Length > 0 ? nameParts[0] : "";
            string firstName = nameParts.Length > 1 ? nameParts[1] : "";

            using (var conn = _context.CreateConnection())
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        string sqlOrder = @"UPDATE ""Заказы"" 
                                            SET ""Фамилия"" = @last, ""Имя"" = @first, ""Телефон"" = @phone,
                                                ""ID_Статуса"" = @status, ""Способ_Получения"" = @delivery 
                                            WHERE ""ID_Заказа"" = @id";
                        using (var cmd = new NpgsqlCommand(sqlOrder, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@last", lastName);
                            cmd.Parameters.AddWithValue("@first", firstName);
                            cmd.Parameters.AddWithValue("@phone", clientContact);
                            cmd.Parameters.AddWithValue("@status", GetStatusId(status));
                            cmd.Parameters.AddWithValue("@delivery", delivery == DeliveryType.Delivery ? "Доставка" : "Самовывоз");
                            cmd.Parameters.AddWithValue("@id", orderId);
                            cmd.ExecuteNonQuery();
                        }

                        transaction.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception("Ошибка БД: " + ex.Message);
                    }
                }
            }
        }

        public List<Dish> GetAvailableDishes()
        {
            var result = new List<Dish>();
            using (var conn = _context.CreateConnection())
            {
                conn.Open();
                string sql = "SELECT \"ID_Блюда\", \"Наименование\", \"Цена\" FROM \"Блюда\" WHERE \"Признак_Доступности\" = TRUE ORDER BY \"Наименование\"";
                using (var cmd = new NpgsqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(new Dish
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Price = reader.GetDecimal(2)
                        });
                    }
                }
            }
            return result;
        }

        public bool UpdateOrderItems(int orderId, List<OrderItem> newItems, decimal newTotalAmount)
        {
            using (var conn = _context.CreateConnection())
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        string sqlDelete = @"DELETE FROM ""Позиции_Заказа"" WHERE ""ID_Заказа"" = @id";
                        using (var cmd = new NpgsqlCommand(sqlDelete, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@id", orderId);
                            cmd.ExecuteNonQuery();
                        }

                        foreach (var item in newItems)
                        {
                            string sqlInsert = @"INSERT INTO ""Позиции_Заказа"" (""ID_Заказа"", ""ID_Блюда"", ""Количество"", ""Цена_За_Единицу"", ""Сумма"") 
                                                 VALUES (@orderId, @dishId, @qty, @price, @sum)";
                            using (var cmd = new NpgsqlCommand(sqlInsert, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@orderId", orderId);
                                cmd.Parameters.AddWithValue("@dishId", item.DishId);
                                cmd.Parameters.AddWithValue("@qty", item.Quantity);
                                cmd.Parameters.AddWithValue("@price", item.Price);
                                cmd.Parameters.AddWithValue("@sum", item.Price * item.Quantity);
                                cmd.ExecuteNonQuery();
                            }
                        }

                        string sqlUpdateOrder = @"UPDATE ""Заказы"" SET ""Сумма_Заказа"" = @total WHERE ""ID_Заказа"" = @id";
                        using (var cmd = new NpgsqlCommand(sqlUpdateOrder, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@total", newTotalAmount);
                            cmd.Parameters.AddWithValue("@id", orderId);
                            cmd.ExecuteNonQuery();
                        }

                        transaction.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception("Ошибка сохранения состава: " + ex.Message);
                    }
                }
            }
        }

        public StatisticsModel GetStatistics(DateTime startDate, DateTime endDate)
        {
            var model = new StatisticsModel();

            using (var conn = _context.CreateConnection())
            {
                conn.Open();

                // Выручка и выполненные
                string sqlRevenue = @"
                    SELECT COALESCE(SUM(""Сумма_Заказа""), 0), COUNT(*) 
                    FROM ""Заказы"" 
                    WHERE ""ID_Статуса"" = 6 
                      AND ""Дата_Оформления"" >= @start AND ""Дата_Оформления"" <= @end";
                decimal totalRevenue = 0;
                int completedCount = 0;
                using (var cmd = new NpgsqlCommand(sqlRevenue, conn))
                {
                    cmd.Parameters.AddWithValue("@start", startDate);
                    cmd.Parameters.AddWithValue("@end", endDate);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            totalRevenue = reader.GetDecimal(0);
                            completedCount = reader.GetInt32(1);
                        }
                    }
                }

                // Общее количество
                string sqlTotal = @"SELECT COUNT(*) FROM ""Заказы"" WHERE ""Дата_Оформления"" >= @start AND ""Дата_Оформления"" <= @end";
                int totalCount = 0;
                using (var cmd = new NpgsqlCommand(sqlTotal, conn))
                {
                    cmd.Parameters.AddWithValue("@start", startDate);
                    cmd.Parameters.AddWithValue("@end", endDate);
                    totalCount = Convert.ToInt32(cmd.ExecuteScalar());
                }

                // Отменённые
                string sqlCancelled = @"SELECT COUNT(*) FROM ""Заказы"" WHERE ""ID_Статуса"" = 7 AND ""Дата_Оформления"" >= @start AND ""Дата_Оформления"" <= @end";
                int cancelledCount = 0;
                using (var cmd = new NpgsqlCommand(sqlCancelled, conn))
                {
                    cmd.Parameters.AddWithValue("@start", startDate);
                    cmd.Parameters.AddWithValue("@end", endDate);
                    cancelledCount = Convert.ToInt32(cmd.ExecuteScalar());
                }

                decimal avgCheck = completedCount > 0 ? totalRevenue / completedCount : 0;

                model.TotalRevenue = totalRevenue;
                model.TotalOrders = totalCount;
                model.CompletedOrders = completedCount;
                model.CancelledOrders = cancelledCount;
                model.AverageCheck = avgCheck;

                model.Rows.Add(new StatisticsRow { IndicatorName = "Общая выручка", DisplayValue = totalRevenue.ToString("N2") + " ₽" });
                model.Rows.Add(new StatisticsRow { IndicatorName = "Количество заказов", DisplayValue = totalCount.ToString() });
                model.Rows.Add(new StatisticsRow { IndicatorName = "Выполненные заказы", DisplayValue = completedCount.ToString() });
                model.Rows.Add(new StatisticsRow { IndicatorName = "Средний чек", DisplayValue = avgCheck.ToString("N2") + " ₽" });
                model.Rows.Add(new StatisticsRow { IndicatorName = "Отменённые заказы", DisplayValue = cancelledCount.ToString() });
            }
            return model;
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
            string statusName = "";
            switch (status)
            {
                case OrderStatus.New: statusName = "Новый"; break;
                case OrderStatus.Accepted: statusName = "Принят"; break;
                case OrderStatus.Cooking: statusName = "Готовится"; break;
                case OrderStatus.Ready: statusName = "Готов"; break;
                case OrderStatus.Delivering: statusName = "Доставляется"; break;
                case OrderStatus.Completed: statusName = "Выполнен"; break;
                case OrderStatus.Cancelled: statusName = "Отменён"; break;
            }

            using (var conn = _context.CreateConnection())
            {
                conn.Open();
                string sql = "SELECT \"ID_Статуса\" FROM \"Статусы_Заказа\" WHERE \"Наименование\" = @name LIMIT 1";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@name", statusName);
                    var result = cmd.ExecuteScalar();
                    if (result != null)
                        return Convert.ToInt32(result);
                }
            }
            return 1;
        }
    }
}
