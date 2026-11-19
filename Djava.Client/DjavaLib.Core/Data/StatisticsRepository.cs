using DjavaLib.Data;
using DjavaLib.Models;
using Npgsql;
using System;
using System.Collections.Generic;

namespace DjavaLib.Core.Data
{
    public class StatisticsRepository : IStatisticsRepository
    {
        private readonly DbContext _context;

        public StatisticsRepository(DbContext context)
        {
            _context = context;
        }

        public StatisticsModel GetGeneralStatistics()
        {
            var model = new StatisticsModel();
            string connectionString = _context.ConnectionString;

            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();

                // 1. Общая выручка (статус 6 - Выполнен)
                using (var cmd = new NpgsqlCommand("SELECT COALESCE(SUM(\"Сумма_Заказа\"), 0) FROM \"Заказы\" WHERE \"ID_Статуса\" = 6", conn))
                {
                    model.TotalRevenue = Convert.ToDecimal(cmd.ExecuteScalar());
                }

                // 2. Общее количество заказов
                using (var cmd = new NpgsqlCommand("SELECT COUNT(*) FROM \"Заказы\"", conn))
                {
                    model.TotalOrders = Convert.ToInt32(cmd.ExecuteScalar());
                }

                // 3. Выполненные заказы
                using (var cmd = new NpgsqlCommand("SELECT COUNT(*) FROM \"Заказы\" WHERE \"ID_Статуса\" = 6", conn))
                {
                    model.CompletedOrders = Convert.ToInt32(cmd.ExecuteScalar());
                }

                // 4. Отменённые заказы
                using (var cmd = new NpgsqlCommand("SELECT COUNT(*) FROM \"Заказы\" WHERE \"ID_Статуса\" = 7", conn))
                {
                    model.CancelledOrders = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }

            // 5. Средний чек
            model.AverageCheck = model.CompletedOrders > 0
                ? Math.Round(model.TotalRevenue / model.CompletedOrders, 2)
                : 0;
            model.Rows = new List<StatisticsRow>
            {
                new StatisticsRow { IndicatorName = "Общая выручка", DisplayValue = $"{model.TotalRevenue:N2} ₽" },
                new StatisticsRow { IndicatorName = "Количество заказов", DisplayValue = model.TotalOrders.ToString() },
                new StatisticsRow { IndicatorName = "Выполненные заказы", DisplayValue = model.CompletedOrders.ToString() },
                new StatisticsRow { IndicatorName = "Средний чек", DisplayValue = $"{model.AverageCheck:N2} ₽" },
                new StatisticsRow { IndicatorName = "Отменённые заказы", DisplayValue = model.CancelledOrders.ToString() }
            };

            return model;
        }
    }
}