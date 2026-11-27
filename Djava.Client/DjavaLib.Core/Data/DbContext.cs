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
        /// Создание заказа. Принимает заказ и комментарий.
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
                        // 1. Вставляем заказ в таблицу "Заказы"
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
                                ""Комментарий""
                            ) VALUES (
                                @clientId, 
                                NOW(), 
                                @pickupMethod, 
                                @address, 
                                @total,
                                @lastName,
                                @firstName,
                                1, 
                                @comment
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

                            orderId = Convert.ToInt32(await cmd.ExecuteScalarAsync());
                        }

                        // 2. Вставляем позиции в "Позиции_Заказа"
                        // ВАЖНО: Добавлена колонка "Сумма" и расчет цены * кол-во
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

                                // Расчет суммы позиции
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
        /// Получение заказа по ID. ДЕЛАЕТ JOIN С ТАБЛИЦЕЙ БЛЮДА ДЛЯ ПОЛУЧЕНИЯ НАЗВАНИЙ!
        /// </summary>
        public async Task<Order> GetOrderByIdAsync(int orderId)
        {
            using (var conn = CreateConnection())
            {
                await conn.OpenAsync();
                Order order = null;

                // Читаем основной заказ
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
                                Status = "Новый" // Пока жестко задаем статус, можно доработать позже
                            };
                        }
                    }
                }

                if (order == null) return null;

                // Читаем позиции С ДЕЛАЕМ JOIN С ТАБЛИЦЕЙ БЛЮДА
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
                                Id = reader.GetInt32(0), // ID_Позиции
                                OrderId = reader.GetInt32(1), // ID_Заказа
                                DishId = reader.GetInt32(2), // ID_Блюда
                                // Берем наименование из JOIN с таблицей Блюда
                                DishName = reader.IsDBNull(3) ? $"Блюдо #{reader.GetInt32(2)}" : reader.GetString(3),
                                Quantity = reader.GetInt32(4), // Количество
                                Price = reader.GetDecimal(5) // Цена_За_Единицу
                            });
                        }
                    }
                }

                return order;
            }
        }
    }
}
