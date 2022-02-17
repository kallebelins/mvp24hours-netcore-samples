using CustomerAPI.Core.Enums;

namespace CustomerAPI.Core.ValueObjects.Customers
{
    public class ContactIdResult
    {
        public ContactType Type { get; set; }
        public string Description { get; set; }
    }
}
