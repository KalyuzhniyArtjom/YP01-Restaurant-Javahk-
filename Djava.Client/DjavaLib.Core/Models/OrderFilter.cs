namespace DjavaLib.Models
{
    public class OrderFilter
    {
        public OrderStatus? Status { get; set; }

        public bool IsDefault
        {
            get { return Status == null; }
        }

        public OrderFilter()
        {
            Status = null;
        }

        public void Reset()
        {
            Status = null;
        }
    }
}