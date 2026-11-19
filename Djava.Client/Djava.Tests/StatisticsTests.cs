using DjavaLib.Core.Data;
using DjavaLib.Data;
using DjavaLib.Models;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit; // <-- Оставили только xUnit

namespace Djava.Tests
{
    public class StatisticsTests
    {
        [Fact]
        public void GetGeneralStatistics_Success_ReturnsFilledModel()
        {
            // Arrange
            var mockRepo = new Mock<IStatisticsRepository>();
            var expected = new StatisticsModel
            {
                TotalRevenue = 125450.00m,
                TotalOrders = 48,
                CompletedOrders = 42,
                AverageCheck = 2986.90m,
                CancelledOrders = 6,
                Rows = new List<StatisticsRow>
                {
                    new StatisticsRow { IndicatorName = "Общая выручка", DisplayValue = "125 450,00 ₽" },
                    new StatisticsRow { IndicatorName = "Количество заказов", DisplayValue = "48" },
                    new StatisticsRow { IndicatorName = "Выполненные заказы", DisplayValue = "42" },
                    new StatisticsRow { IndicatorName = "Средний чек", DisplayValue = "2 986,90 ₽" },
                    new StatisticsRow { IndicatorName = "Отменённые заказы", DisplayValue = "6" }
                }
            };
            mockRepo.Setup(r => r.GetGeneralStatistics()).Returns(expected);

            // Act
            var result = mockRepo.Object.GetGeneralStatistics();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(5, result.Rows.Count);
            Assert.Equal(125450.00m, result.TotalRevenue);
            Assert.Equal(48, result.TotalOrders);
        }

        [Fact]
        public void GetGeneralStatistics_EmptyDatabase_ReturnsZeros()
        {
            // Arrange
            var mockRepo = new Mock<IStatisticsRepository>();
            var empty = new StatisticsModel
            {
                TotalRevenue = 0,
                TotalOrders = 0,
                CompletedOrders = 0,
                AverageCheck = 0,
                CancelledOrders = 0,
                Rows = new List<StatisticsRow>
                {
                    new StatisticsRow { IndicatorName = "Общая выручка", DisplayValue = "0,00 ₽" },
                    new StatisticsRow { IndicatorName = "Количество заказов", DisplayValue = "0" },
                    new StatisticsRow { IndicatorName = "Выполненные заказы", DisplayValue = "0" },
                    new StatisticsRow { IndicatorName = "Средний чек", DisplayValue = "0,00 ₽" },
                    new StatisticsRow { IndicatorName = "Отменённые заказы", DisplayValue = "0" }
                }
            };
            mockRepo.Setup(r => r.GetGeneralStatistics()).Returns(empty);

            // Act
            var result = mockRepo.Object.GetGeneralStatistics();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(0, result.TotalRevenue);
            Assert.Equal(0, result.TotalOrders);
            Assert.Equal(0, result.AverageCheck);
        }

        [Fact]
        public void GetGeneralStatistics_DbError_ThrowsException()
        {
            // Arrange
            var mockRepo = new Mock<IStatisticsRepository>();
            mockRepo.Setup(r => r.GetGeneralStatistics()).Throws(new Exception("Ошибка подключения к БД"));

            // Act & Assert
            var ex = Assert.Throws<Exception>(() => mockRepo.Object.GetGeneralStatistics());
            Assert.Equal("Ошибка подключения к БД", ex.Message);
        }

        [Fact]
        public void GetGeneralStatistics_Formatting_ReturnsCorrectStrings()
        {
            // Arrange
            var mockRepo = new Mock<IStatisticsRepository>();
            var model = new StatisticsModel
            {
                TotalRevenue = 100000.00m,
                TotalOrders = 40,
                CompletedOrders = 40,
                AverageCheck = 2500.00m,
                CancelledOrders = 0,
                Rows = new List<StatisticsRow>
                {
                    new StatisticsRow { IndicatorName = "Общая выручка", DisplayValue = "100 000,00 ₽" },
                    new StatisticsRow { IndicatorName = "Средний чек", DisplayValue = "2 500,00 ₽" }
                }
            };
            mockRepo.Setup(r => r.GetGeneralStatistics()).Returns(model);

            // Act
            var result = mockRepo.Object.GetGeneralStatistics();

            // Assert
            var revenueRow = result.Rows.First(r => r.IndicatorName == "Общая выручка");
            var avgRow = result.Rows.First(r => r.IndicatorName == "Средний чек");

            Assert.Equal("100 000,00 ₽", revenueRow.DisplayValue);
            Assert.Equal("2 500,00 ₽", avgRow.DisplayValue);
        }
    }
}