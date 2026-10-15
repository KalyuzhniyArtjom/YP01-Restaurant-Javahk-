using System.Collections.Generic;

namespace DjavaLib.Models
{
    public class OrderListModel
    {
        public List<Order> Orders { get; set; }

        public int TotalCount
        {
            get { return Orders != null ? Orders.Count : 0; }
        }
    }
}