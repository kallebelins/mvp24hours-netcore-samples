using CustomerAPI.Core.Entities;
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
        Task<IPagingResult<IList<Customer>>> GetBy(CustomerQuery model, IPagingCriteria criteria, CancellationToken cancellationToken = default);
        Task<IBusinessResult<Customer>> GetById(int id, CancellationToken cancellationToken = default);
        Task<IBusinessResult<int>> Create(Customer entityModel, CancellationToken cancellationToken = default);
        Task<IBusinessResult<int>> Update(int id, Customer entityModel, CancellationToken cancellationToken = default);
        Task<IBusinessResult<int>> Delete(int id, CancellationToken cancellationToken = default);
    }
}
