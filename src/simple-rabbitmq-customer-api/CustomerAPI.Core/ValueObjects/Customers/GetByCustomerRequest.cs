namespace CustomerAPI.Core.ValueObjects.Customers
{
    public class GetByCustomerRequest
    {
        public string Name { get; set; }
        public bool? Active { get; set; }
    }
}
