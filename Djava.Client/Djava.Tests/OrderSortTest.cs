using DjavaLib.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Djava.Tests
{
    [TestClass]
    public class OrderSortTest
    {
        private OrderListModel _model;
        [TestInitialize]
        public void Setup()
        {
            _model = new OrderListModel();
        }

        // Вспомогательный метод для создания тестовых заказов
        private List<Order> CreateTestOrders()
        {
            return new List<Order>
            {
                new Order { OrderId = 1, Status = OrderStatus.New, OrderDate = new DateTime(2026, 10, 1, 12, 0, 0), TotalAmount = 1050, ClientName = "Кузнецов", ClientContact = "111", DeliveryMethod = DeliveryType.Pickup },
                new Order { OrderId = 2, Status = OrderStatus.Accepted, OrderDate = new DateTime(2026, 10, 2, 15, 30, 0), TotalAmount = 450, ClientName = "Иванов", ClientContact = "222", DeliveryMethod = DeliveryType.Delivery },
                new Order { OrderId = 3, Status = OrderStatus.New, OrderDate = new DateTime(2026, 10, 3, 9, 0, 0), TotalAmount = 900, ClientName = "Смирнова", ClientContact = "333", DeliveryMethod = DeliveryType.Pickup }
            };
        }

        // ==================== ТЕСТЫ ФИЛЬТРАЦИИ ====================

        [TestMethod]
        public void FilterByStatus_Null_ReturnsAllOrders()
        {
            _model.Orders = CreateTestOrders();
            var result = _model.FilterByStatus(null);
            Assert.AreEqual(3, result.Count);
        }

        [TestMethod]
        public void FilterByStatus_New_ReturnsOnlyNewOrders()
        {
            _model.Orders = CreateTestOrders();
            var result = _model.FilterByStatus(OrderStatus.New);
            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.All(o => o.Status == OrderStatus.New));
        }

        // ==================== ТЕСТЫ СОРТИРОВКИ ====================

        [TestMethod]
        public void SortByDate_Descending_ReturnsNewestFirst()
        {
            var orders = CreateTestOrders();
            var result = _model.SortByDate(orders, false); // false = по убыванию

            Assert.AreEqual(3, result[0].OrderId); // 03.10
            Assert.AreEqual(2, result[1].OrderId); // 02.10
            Assert.AreEqual(1, result[2].OrderId); // 01.10
        }

        [TestMethod]
        public void SortByAmount_Ascending_ReturnsCheapestFirst()
        {
            var orders = CreateTestOrders();
            var result = _model.SortByAmount(orders, true); // true = по возрастанию

            Assert.AreEqual(450, result[0].TotalAmount);
            Assert.AreEqual(900, result[1].TotalAmount);
            Assert.AreEqual(1050, result[2].TotalAmount);
        }

        [TestMethod]
        public void SortByClientName_Ascending_ReturnsSortedAlphabetically()
        {
            var orders = CreateTestOrders(); // Иванов, Кузнецов, Смирнова
            var result = _model.SortByClientName(orders, true);

            Assert.AreEqual("Иванов", result[0].ClientName);
            Assert.AreEqual("Кузнецов", result[1].ClientName);
            Assert.AreEqual("Смирнова", result[2].ClientName);
        }
    }
}
