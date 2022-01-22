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
        Task<IPagingResult<IList<GetByCustomerResponse>>> GetBy(GetByCustomerRequest filter, IPagingCriteria criteria, CancellationToken cancellationToken = default);
        Task<IBusinessResult<GetByIdCustomerResponse>> GetById(string id, CancellationToken cancellationToken = default);
        Task<IBusinessResult<string>> Create(CreateCustomerRequest dto, CancellationToken cancellationToken = default);
        Task<IBusinessResult<int>> Update(string id, UpdateCustomerRequest dto, CancellationToken cancellationToken = default);
        Task<IBusinessResult<int>> Delete(string id, CancellationToken cancellationToken = default);
    }
}
