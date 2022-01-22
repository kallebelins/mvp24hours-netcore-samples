namespace CustomerAPI.Core.ValueObjects.Customers
{
    public class GetByCustomerFilterRequest
    {
        public string Name { get; set; }

        public bool HasCellContact { get; set; }
        public bool HasEmailContact { get; set; }
    }
}
