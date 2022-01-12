using CustomerAPI.Core.ValueObjects.Customers;
using Mvp24Hours.Core.Contract.ValueObjects.Logic;
using Mvp24Hours.Core.DTOs;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CustomerAPI.Core.Contract.Logic
{
    public interface ICustomerService
    {
        Task<IPagingResult<IList<GetByCustomerResponse>>> GetBy(GetByCustomerRequest filter, IPagingCriteria criteria);
        Task<IBusinessResult<GetByIdCustomerResponse>> GetById(int id);
        Task<IBusinessResult<int>> Create(CreateCustomerRequest dto, CancellationToken cancellationToken = default);
        Task<IBusinessResult<VoidResult>> Update(int id, UpdateCustomerRequest dto, CancellationToken cancellationToken = default);
        Task<IBusinessResult<VoidResult>> Delete(int id, CancellationToken cancellationToken = default);
    }
}
