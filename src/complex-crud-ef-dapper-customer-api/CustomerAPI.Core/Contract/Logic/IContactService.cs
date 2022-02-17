using CustomerAPI.Core.ValueObjects.Contacts;
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
        Task<IBusinessResult<IList<ContactIdResult>>> GetBy(int customerId, CancellationToken cancellationToken = default);
        Task<IBusinessResult<int>> Create(int customerId, ContactCreate dto, CancellationToken cancellationToken = default);
        Task<IBusinessResult<int>> Update(int customerId, int id, ContactUpdate dto, CancellationToken cancellationToken = default);
        Task<IBusinessResult<int>> Delete(int customerId, int id, CancellationToken cancellationToken = default);
    }
}
