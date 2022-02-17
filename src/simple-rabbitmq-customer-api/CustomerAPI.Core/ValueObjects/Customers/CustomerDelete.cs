namespace CustomerAPI.Core.ValueObjects.Customers
{
    public class CustomerDelete : MessageRequest
    {
        public int Id { get; set; }
    }
}
