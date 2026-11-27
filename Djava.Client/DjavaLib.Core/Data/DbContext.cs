using DjavaLib.Core.Models;
using DjavaLib.Models;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DjavaLib.Data
{
    public class DbContext
    {
        private readonly string _connectionString;

        public DbContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        public string ConnectionString => _connectionString;

        public NpgsqlConnection CreateConnection()
        {
            return new NpgsqlConnection(_connectionString);
        }

        /// <summary>
        /// Создание заказа. ТЕЛЕФОН СОХРАНЯЕТСЯ ПРЯМО В ТАБЛИЦУ ЗАКАЗЫ.
        /// </summary>
        public async Task<int> CreateOrderAsync(Order order, string comment = "")
        {
            using (var conn = CreateConnection())
            {
                await conn.OpenAsync();

                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // 1. Вставляем заказ. ДОБАВЛЕНО: колонка "Телефон"
                        string insertOrderSql = @"
                            INSERT INTO ""Заказы"" (
                                ""ID_Клиента"", 
                                ""Дата_Оформления"", 
                                ""Способ_Получения"", 
                                ""Адрес_Доставки"", 
                                ""Сумма_Заказа"",
                                ""Фамилия"",
                                ""Имя"",
                                ""ID_Статуса"",
                                ""Комментарий"",
                                ""Телефон"" 
                            ) VALUES (
                                @clientId, 
                                NOW(), 
                                @pickupMethod, 
                                @address, 
                                @total,
                                @lastName,
                                @firstName,
                                1, 
                                @comment,
                                @phone 
                            ) RETURNING ""ID_Заказа"";";

                        int orderId;
                        using (var cmd = new NpgsqlCommand(insertOrderSql, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@clientId", order.ClientId);
                            cmd.Parameters.AddWithValue("@pickupMethod", (object)order.PickupMethod ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@address", (object)order.DeliveryAddress ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@total", order.TotalAmount);
                            cmd.Parameters.AddWithValue("@lastName", (object)order.LastName ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@firstName", (object)order.FirstName ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@comment", (object)comment ?? DBNull.Value);

                            // Сохраняем телефон из модели Order прямо в базу
                            cmd.Parameters.AddWithValue("@phone", (object)order.Phone ?? DBNull.Value);

                            orderId = Convert.ToInt32(await cmd.ExecuteScalarAsync());
                        }

                        // 2. Вставляем позиции в "Позиции_Заказа"
                        string insertItemSql = @"
                            INSERT INTO ""Позиции_Заказа"" (
                                ""ID_Заказа"", 
                                ""ID_Блюда"", 
                                ""Количество"", 
                                ""Цена_За_Единицу"",
                                ""Сумма""
                            ) VALUES (
                                @orderId, 
                                @dishId, 
                                @qty, 
                                @price,
                                @sum
                            );";

                        foreach (var item in order.Items)
                        {
                            using (var cmd = new NpgsqlCommand(insertItemSql, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@orderId", orderId);
                                cmd.Parameters.AddWithValue("@dishId", item.DishId);
                                cmd.Parameters.AddWithValue("@qty", item.Quantity);
                                cmd.Parameters.AddWithValue("@price", item.Price);

                                decimal sum = item.Price * item.Quantity;
                                cmd.Parameters.AddWithValue("@sum", sum);

                                await cmd.ExecuteNonQueryAsync();
                            }
                        }

                        transaction.Commit();
                        return orderId;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception($"Ошибка БД при создании заказа: {ex.Message}", ex);
                    }
                }
            }
        }

        /// <summary>
        /// Получение заказа по ID. ТЕЛЕФОН БЕРЕТСЯ ПРЯМО ИЗ ТАБЛИЦЫ ЗАКАЗЫ.
        /// </summary>
        public async Task<Order> GetOrderByIdAsync(int orderId)
        {
            using (var conn = CreateConnection())
            {
                await conn.OpenAsync();
                Order order = null;

                // Читаем заказ напрямую (телефон уже лежит в этой таблице)
                string getOrderSql = "SELECT * FROM \"Заказы\" WHERE \"ID_Заказа\" = @id LIMIT 1;";

                using (var cmd = new NpgsqlCommand(getOrderSql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", orderId);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            order = new Order
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("ID_Заказа")),
                                ClientId = reader.GetInt32(reader.GetOrdinal("ID_Клиента")),
                                CreatedAt = reader.GetDateTime(reader.GetOrdinal("Дата_Оформления")),
                                PickupMethod = reader.IsDBNull(reader.GetOrdinal("Способ_Получения")) ? null : reader.GetString(reader.GetOrdinal("Способ_Получения")),
                                DeliveryAddress = reader.IsDBNull(reader.GetOrdinal("Адрес_Доставки")) ? null : reader.GetString(reader.GetOrdinal("Адрес_Доставки")),
                                TotalAmount = reader.GetDecimal(reader.GetOrdinal("Сумма_Заказа")),
                                LastName = reader.IsDBNull(reader.GetOrdinal("Фамилия")) ? null : reader.GetString(reader.GetOrdinal("Фамилия")),
                                FirstName = reader.IsDBNull(reader.GetOrdinal("Имя")) ? null : reader.GetString(reader.GetOrdinal("Имя")),

                                // Берем телефон прямо из колонки "Телефон" таблицы Заказы
                                Phone = reader.IsDBNull(reader.GetOrdinal("Телефон")) ? null : reader.GetString(reader.GetOrdinal("Телефон")),

                                Status = "Новый"
                            };
                        }
                    }
                }

                if (order == null) return null;

                // Чтение позиций (JOIN с Блюдами нужен только для названий)
                string getItemsSql = @"
                    SELECT p.""ID_Позиции"", p.""ID_Заказа"", p.""ID_Блюда"", b.""Наименование"", 
                           p.""Количество"", p.""Цена_За_Единицу""
                    FROM ""Позиции_Заказа"" p
                    JOIN ""Блюда"" b ON p.""ID_Блюда"" = b.""ID_Блюда""
                    WHERE p.""ID_Заказа"" = @orderId;";

                using (var cmd = new NpgsqlCommand(getItemsSql, conn))
                {
                    cmd.Parameters.AddWithValue("@orderId", orderId);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            order.Items.Add(new OrderItem
                            {
                                Id = reader.GetInt32(0),
                                OrderId = reader.GetInt32(1),
                                DishId = reader.GetInt32(2),
                                DishName = reader.IsDBNull(3) ? $"Блюдо #{reader.GetInt32(2)}" : reader.GetString(3),
                                Quantity = reader.GetInt32(4),
                                Price = reader.GetDecimal(5)
                            });
                        }
                    }
                }

                return order;
            }
        }
    }
}