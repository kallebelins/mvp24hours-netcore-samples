using CustomerAPI.Core.Entities;
using Mvp24Hours.Core.Contract.ValueObjects.Logic;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CustomerAPI.Core.Contract.Logic
{
    /// <summary>
    /// Customer contact service
    /// </summary>
    public interface IContactService
    {
        Task<IBusinessResult<IList<Contact>>> GetBy(int customerId, CancellationToken cancellationToken = default);
        Task<IBusinessResult<int>> Create(int customerId, Contact entityModel, CancellationToken cancellationToken = default);
        Task<IBusinessResult<int>> Update(int customerId, int id, Contact entityModel, CancellationToken cancellationToken = default);
        Task<IBusinessResult<int>> Delete(int customerId, int id, CancellationToken cancellationToken = default);
    }
}
