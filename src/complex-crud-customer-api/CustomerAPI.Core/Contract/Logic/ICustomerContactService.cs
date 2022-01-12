using CustomerAPI.Core.ValueObjects.Contacts;
using Mvp24Hours.Core.Contract.ValueObjects.Logic;
using Mvp24Hours.Core.DTOs;
using System.Threading;
using System.Threading.Tasks;

namespace CustomerAPI.Core.Contract.Logic
{
    public interface ICustomerContactService
    {
        Task<IBusinessResult<int>> Create(int customerId, CreateContactRequest dto, CancellationToken cancellationToken = default);
        Task<IBusinessResult<VoidResult>> Update(int customerId, int id, UpdateContactRequest dto, CancellationToken cancellationToken = default);
        Task<IBusinessResult<VoidResult>> Delete(int customerId, int id, CancellationToken cancellationToken = default);
    }
}
