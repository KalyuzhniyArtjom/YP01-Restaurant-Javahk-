using DjavaLib.Models;
using System.Collections.Generic;

namespace DjavaLib.Data
{
    public interface IOrderListRepository
    {
        List<Order> GetOrders(OrderFilter filter);
        List<OrderItem> GetOrderItems(int orderId);
        bool UpdateOrderStatus(int orderId, OrderStatus status);
    }
}