using System.Collections.Generic;

namespace CustomerAPI.Core.ValueObjects.Customers
{
    public class CustomerIdResult : CustomerResult
    {
        public IList<ContactIdResult> Contacts { get; set; }
    }
}
