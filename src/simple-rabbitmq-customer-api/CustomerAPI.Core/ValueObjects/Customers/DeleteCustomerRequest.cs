namespace CustomerAPI.Core.ValueObjects.Customers
{
    public class DeleteCustomerRequest : MessageRequest
    {
        public int Id { get; set; }
    }
}
