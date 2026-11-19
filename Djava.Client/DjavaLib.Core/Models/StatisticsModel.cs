using System.Collections.Generic;

namespace DjavaLib.Models
{
    public class StatisticsModel
    {
        public List<StatisticsRow> Rows { get; set; } = new List<StatisticsRow>();
        public decimal TotalRevenue { get; set; }
        public int TotalOrders { get; set; }
        public decimal AverageCheck { get; set; }
        public int CompletedOrders { get; set; }
        public int CancelledOrders { get; set; }
    }
}