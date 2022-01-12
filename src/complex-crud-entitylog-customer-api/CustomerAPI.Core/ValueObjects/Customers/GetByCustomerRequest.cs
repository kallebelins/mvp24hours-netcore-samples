namespace CustomerAPI.Core.ValueObjects.Customers
{
    public class GetByCustomerRequest
    {
        public string Name { get; set; }
        public bool? Active { get; set; }

        public bool HasCellContact { get; set; }
        public bool HasEmailContact { get; set; }
        public bool HasNoContact { get; set; }
        public bool IsProspect { get; set; }
    }
}
