using CustomerAPI.Core.ValueObjects.Customers;
using Mvp24Hours.Core.Contract.ValueObjects.Logic;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CustomerAPI.Core.Contract.Logic
{
    /// <summary>
    /// Represents customer service
    /// </summary>
    public interface ICustomerService
    {
        Task<IPagingResult<IList<CustomerResult>>> GetBy(CustomerQuery filter, IPagingCriteria criteria, CancellationToken cancellationToken = default);
        Task<IBusinessResult<CustomerIdResult>> GetById(string id, CancellationToken cancellationToken = default);
        Task<IBusinessResult<string>> Create(CustomerCreate dto, CancellationToken cancellationToken = default);
        Task<IBusinessResult<int>> Update(string id, CustomerUpdate dto, CancellationToken cancellationToken = default);
        Task<IBusinessResult<int>> Delete(string id, CancellationToken cancellationToken = default);
    }
}
