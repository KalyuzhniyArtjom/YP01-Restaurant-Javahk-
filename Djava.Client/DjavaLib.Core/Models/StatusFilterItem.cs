namespace DjavaLib.Models
{
    public class StatusFilterItem
    {
        public string DisplayName { get; private set; }
        public OrderStatus? Value { get; private set; }

        public StatusFilterItem(string displayName, OrderStatus? value)
        {
            DisplayName = displayName;
            Value = value;
        }
    }
}