using System.Collections.Generic;
using System.Linq;

namespace DjavaLib.Models
{
    public class OrderListModel
    {
        public List<Order> Orders { get; set; } = new List<Order>();

        public int TotalCount
        {
            get { return Orders != null ? Orders.Count : 0; }
        }

        // ============ ФИЛЬТРАЦИЯ ============

        public List<Order> FilterByStatus(OrderStatus? status)
        {
            if (status == null)
                return Orders.ToList();

            return Orders.Where(o => o.Status == status.Value).ToList();
        }

        // ============ СОРТИРОВКА ============

        public List<Order> SortByDate(List<Order> orders, bool ascending)
        {
            if (ascending)
                return orders.OrderBy(o => o.OrderDate).ToList();
            return orders.OrderByDescending(o => o.OrderDate).ToList();
        }

        public List<Order> SortByAmount(List<Order> orders, bool ascending)
        {
            if (ascending)
                return orders.OrderBy(o => o.TotalAmount).ToList();
            return orders.OrderByDescending(o => o.TotalAmount).ToList();
        }

        public List<Order> SortByNumber(List<Order> orders, bool ascending)
        {
            if (ascending)
                return orders.OrderBy(o => o.OrderId).ToList();
            return orders.OrderByDescending(o => o.OrderId).ToList();
        }

        public List<Order> SortByClientName(List<Order> orders, bool ascending)
        {
            if (ascending)
                return orders.OrderBy(o => o.ClientName).ToList();
            return orders.OrderByDescending(o => o.ClientName).ToList();
        }

        public List<Order> SortByContact(List<Order> orders, bool ascending)
        {
            if (ascending)
                return orders.OrderBy(o => o.ClientContact).ToList();
            return orders.OrderByDescending(o => o.ClientContact).ToList();
        }

        public List<Order> SortByDeliveryMethod(List<Order> orders, bool ascending)
        {
            if (ascending)
                return orders.OrderBy(o => o.DeliveryMethod).ToList();
            return orders.OrderByDescending(o => o.DeliveryMethod).ToList();
        }

        public List<Order> SortByStatus(List<Order> orders, bool ascending)
        {
            if (ascending)
                return orders.OrderBy(o => o.Status).ToList();
            return orders.OrderByDescending(o => o.Status).ToList();
        }

        // ============ ПРИМЕНЕНИЕ ФИЛЬТРА + СОРТИРОВКИ ============

        public List<Order> ApplyFilterAndSort(OrderFilter filter, string sortColumn, bool ascending)
        {
            var filtered = FilterByStatus(filter.Status);

            switch (sortColumn)
            {
                case "OrderId": return SortByNumber(filtered, ascending);
                case "OrderDate": return SortByDate(filtered, ascending);
                case "ClientName": return SortByClientName(filtered, ascending);
                case "ClientContact": return SortByContact(filtered, ascending);
                case "DeliveryMethod": return SortByDeliveryMethod(filtered, ascending);
                case "Status": return SortByStatus(filtered, ascending);
                case "TotalAmount": return SortByAmount(filtered, ascending);
                default: return SortByDate(filtered, false);
            }
        }
    }
}