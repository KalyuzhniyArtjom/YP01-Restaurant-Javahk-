using DjavaLib.Core.Models;
using DjavaLib.Data;
using Npgsql;
using System;

namespace DjavaLib.Core.Data
{
    public class StatisticsRepository : IStatisticsRepository
    {
        private readonly DbContext _context;

        // Если ваш DbContext принимает строку подключения в конструкторе, 
        // передавайте её сюда при создании репозитория.
        public StatisticsRepository(DbContext context)
        {
            _context = context;
        }

        public StatisticsModel GetGeneralStatistics()
        {
            var model = new StatisticsModel();

            // ВАЖНО: Здесь нужно использовать вашу строку подключения. 
            // Если в вашем DbContext есть свойство для неё (например, _context.ConnectionString), 
            // замените код ниже на него.
            // В противном случае, используйте строку напрямую (лучше вынести в App.config/Program.cs)
            string connectionString = "Host=localhost;Port=5432;Database=ВашаБаза;Username=postgres;Password=ваш_пароль";

            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                // 1. Общая выручка (ID_Статуса = 6)
                using (var cmd = new NpgsqlCommand("SELECT COALESCE(SUM(\"Сумма_Заказа\"), 0) FROM \"Заказы\" WHERE \"ID_Статуса\" = 6", connection))
                {
                    model.TotalRevenue = Convert.ToDecimal(cmd.ExecuteScalar());
                }

                // 2. Общее количество заказов
                using (var cmd = new NpgsqlCommand("SELECT COUNT(*) FROM \"Заказы\"", connection))
                {
                    model.TotalOrders = Convert.ToInt32(cmd.ExecuteScalar());
                }

                // 3. Выполненные заказы (ID_Статуса = 6)
                using (var cmd = new NpgsqlCommand("SELECT COUNT(*) FROM \"Заказы\" WHERE \"ID_Статуса\" = 6", connection))
                {
                    model.CompletedOrders = Convert.ToInt32(cmd.ExecuteScalar());
                }

                // 4. Отменённые заказы (ID_Статуса = 7)
                using (var cmd = new NpgsqlCommand("SELECT COUNT(*) FROM \"Заказы\" WHERE \"ID_Статуса\" = 7", connection))
                {
                    model.CancelledOrders = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }

            // 5. Средний чек (программно, защита от деления на ноль)
            model.AverageCheck = model.CompletedOrders > 0
                ? Math.Round(model.TotalRevenue / model.CompletedOrders, 2)
                : 0;

            // Формирование строк для DataGridView
            // Используем полный синтаксис new StatisticsRow(), чтобы не было ошибки C# 7.3
            model.Rows = new System.Collections.Generic.List<StatisticsRow>
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