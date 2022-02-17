namespace CustomerAPI.Core.ValueObjects.Customers
{
    public class CustomerQuery
    {
        public string Name { get; set; }

        public bool HasCellContact { get; set; }
        public bool HasEmailContact { get; set; }
    }
}
