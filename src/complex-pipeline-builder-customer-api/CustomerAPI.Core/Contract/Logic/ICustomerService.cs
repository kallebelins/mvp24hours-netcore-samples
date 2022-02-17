using CustomerAPI.Core.ValueObjects.Customers;
using Mvp24Hours.Core.Contract.ValueObjects.Logic;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CustomerAPI.Core.Contract.Logic
{
    /// <summary>
    /// Represents customer service
    /// </summary>
    public interface ICustomerService
    {
        Task<IBusinessResult<IList<CustomerResult>>> GetBy(CustomerQuery filter);
        Task<IBusinessResult<CustomerIdResult>> GetById(int id);
    }
}
