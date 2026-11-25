using System;
using System.Collections.Generic;
using DjavaLib.Models;
using Npgsql;

namespace DjavaLib.Data
{
    public class CartPgRepository : ICartRepository
    {
        private readonly DbContext _context;

        public CartPgRepository(DbContext context)
        {
            _context = context;
        }

        public List<CartItem> GetCartItems(int clientId)
        {
            var result = new List<CartItem>();
            using (var conn = _context.CreateConnection())
            {
                conn.Open();
                // Предполагается, что таблица корзины называется "Корзина"
                var sql = @"
                    SELECT k.""ID_Корзины"", k.""ID_Блюда"", б.""Наименование"", б.""Цена"", k.""Количество"", б.""Путь_К_Фотографии""
                    FROM ""Корзина"" k
                    JOIN ""Блюда"" б ON k.""ID_Блюда"" = б.""ID_Блюда""
                    WHERE k.""ID_Клиента"" = @clientId";

                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@clientId", clientId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(new CartItem
                            {
                                CartId = reader.GetInt32(0),
                                DishId = reader.GetInt32(1),
                                DishName = reader.GetString(2),
                                Price = reader.GetDecimal(3),
                                Quantity = reader.GetInt32(4),
                                PhotoPath = reader.IsDBNull(5) ? "" : reader.GetString(5)
                            });
                        }
                    }
                }
            }
            return result;
        }

        public void AddToCart(int clientId, int dishId)
        {
            using (var conn = _context.CreateConnection())
            {
                conn.Open();

                var checkSql = "SELECT \"Количество\", \"ID_Корзины\" FROM \"Корзина\" WHERE \"ID_Клиента\" = @clientId AND \"ID_Блюда\" = @dishId";
                int? existingQty = null;
                int? cartId = null;

                using (var cmd = new NpgsqlCommand(checkSql, conn))
                {
                    cmd.Parameters.AddWithValue("@clientId", clientId);
                    cmd.Parameters.AddWithValue("@dishId", dishId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            existingQty = reader.GetInt32(0);
                            cartId = reader.GetInt32(1);
                        }
                    }
                }

                if (existingQty.HasValue && cartId.HasValue)
                {
                    var updateSql = "UPDATE \"Корзина\" SET \"Количество\" = @qty WHERE \"ID_Корзины\" = @id";
                    using (var cmd = new NpgsqlCommand(updateSql, conn))
                    {
                        cmd.Parameters.AddWithValue("@qty", existingQty.Value + 1);
                        cmd.Parameters.AddWithValue("@id", cartId.Value);
                        cmd.ExecuteNonQuery();
                    }
                }
                else
                {
                    var insertSql = "INSERT INTO \"Корзина\" (\"ID_Клиента\", \"ID_Блюда\", \"Количество\") VALUES (@clientId, @dishId, 1)";
                    using (var cmd = new NpgsqlCommand(insertSql, conn))
                    {
                        cmd.Parameters.AddWithValue("@clientId", clientId);
                        cmd.Parameters.AddWithValue("@dishId", dishId);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        public void UpdateQuantity(int cartId, int quantity)
        {
            using (var conn = _context.CreateConnection())
            {
                conn.Open();
                if (quantity <= 0)
                {
                    var delSql = "DELETE FROM \"Корзина\" WHERE \"ID_Корзины\" = @id";
                    using (var cmd = new NpgsqlCommand(delSql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", cartId);
                        cmd.ExecuteNonQuery();
                    }
                }
                else
                {
                    var sql = "UPDATE \"Корзина\" SET \"Количество\" = @qty WHERE \"ID_Корзины\" = @id";
                    using (var cmd = new NpgsqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@qty", quantity);
                        cmd.Parameters.AddWithValue("@id", cartId);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        public void RemoveFromCart(int cartId)
        {
            using (var conn = _context.CreateConnection())
            {
                conn.Open();
                var sql = "DELETE FROM \"Корзина\" WHERE \"ID_Корзины\" = @id";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", cartId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void ClearCart(int clientId)
        {
            using (var conn = _context.CreateConnection())
            {
                conn.Open();
                var sql = "DELETE FROM \"Корзина\" WHERE \"ID_Клиента\" = @clientId";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@clientId", clientId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public CartSummary GetCartSummary(int clientId)
        {
            var summary = new CartSummary();
            using (var conn = _context.CreateConnection())
            {
                conn.Open();
                var sql = @"
                    SELECT COALESCE(SUM(k.""Количество""), 0), COALESCE(SUM(k.""Количество"" * б.""Цена""), 0)
                    FROM ""Корзина"" k
                    JOIN ""Блюда"" б ON k.""ID_Блюда"" = б.""ID_Блюда""
                    WHERE k.""ID_Клиента"" = @clientId";

                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@clientId", clientId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            summary.TotalItems = Convert.ToInt32(reader.GetValue(0));
                            summary.TotalAmount = Convert.ToDecimal(reader.GetValue(1));
                        }
                    }
                }
            }
            return summary;
        }
    }
}