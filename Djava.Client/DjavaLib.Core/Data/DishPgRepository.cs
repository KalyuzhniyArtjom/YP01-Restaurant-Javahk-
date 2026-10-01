using DjavaLib.Models;
using Npgsql;
using System.Collections.Generic;

namespace DjavaLib.Data
{
    public class DishPgRepository : IDishRepository
    {
        private readonly string _connectionString;

        public DishPgRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<Dish> GetAllAvailableDishes()
        {
            var result = new List<Dish>();
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                string sql = "SELECT \"ID_Блюда\", \"ID_Категории\", \"Наименование\", \"Описание\", " +
                             "\"Состав\", \"Цена\", \"Вес_Объём\", \"Признак_Доступности\" " +
                             "FROM \"Блюда\" WHERE \"Признак_Доступности\" = TRUE " +
                             "ORDER BY \"ID_Категории\", \"Наименование\"";

                using (var cmd = new NpgsqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(MapDish(reader));
                    }
                }
            }
            return result;
        }

        public List<Category> GetAllCategories()
        {
            var result = new List<Category>();
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                string sql = "SELECT \"ID_Категории\", \"Наименование\", \"Описание\", \"Порядок_Отображения\" " +
                             "FROM \"Категории_Блюд\" ORDER BY \"Порядок_Отображения\"";

                using (var cmd = new NpgsqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(new Category
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Description = reader.IsDBNull(2) ? "" : reader.GetString(2),
                            DisplayOrder = reader.GetInt32(3)
                        });
                    }
                }
            }
            return result;
        }

        public List<Dish> GetDishesByCategory(int categoryId)
        {
            var result = new List<Dish>();
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                string sql = "SELECT \"ID_Блюда\", \"ID_Категории\", \"Наименование\", \"Описание\", " +
                             "\"Состав\", \"Цена\", \"Вес_Объём\", \"Признак_Доступности\" " +
                             "FROM \"Блюда\" WHERE \"ID_Категории\" = @catId AND \"Признак_Доступности\" = TRUE";

                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@catId", categoryId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(MapDish(reader));
                        }
                    }
                }
            }
            return result;
        }

        public Dish GetDishById(int id)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                string sql = "SELECT \"ID_Блюда\", \"ID_Категории\", \"Наименование\", \"Описание\", " +
                             "\"Состав\", \"Цена\", \"Вес_Объём\", \"Признак_Доступности\" " +
                             "FROM \"Блюда\" WHERE \"ID_Блюда\" = @id";

                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read()) return MapDish(reader);
                    }
                }
            }
            return null;
        }

        private Dish MapDish(NpgsqlDataReader reader)
        {
            return new Dish
            {
                Id = reader.GetInt32(0),
                CategoryId = reader.GetInt32(1),
                Name = reader.GetString(2),
                Description = reader.IsDBNull(3) ? "" : reader.GetString(3),
                Composition = reader.IsDBNull(4) ? "" : reader.GetString(4),
                Price = reader.GetDecimal(5),
                Weight = reader.IsDBNull(6) ? (decimal?)null : reader.GetDecimal(6),
                IsAvailable = reader.GetBoolean(7)
            };
        }
    }
}