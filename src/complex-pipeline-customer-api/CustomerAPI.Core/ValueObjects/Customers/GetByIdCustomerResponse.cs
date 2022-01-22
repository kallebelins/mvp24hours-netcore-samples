using System.Collections.Generic;

namespace CustomerAPI.Core.ValueObjects.Customers
{
    public class GetByIdCustomerResponse : GetByCustomerResponse
    {
        public IList<GetByIdContactResponse> Contacts { get; set; }
    }
}
