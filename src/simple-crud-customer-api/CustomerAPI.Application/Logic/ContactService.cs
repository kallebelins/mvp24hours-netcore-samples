using CustomerAPI.Core.Contract.Logic;
using CustomerAPI.Core.Entities;
using Mvp24Hours.Application.Logic;
using Mvp24Hours.Core.Contract.Data;

namespace CustomerAPI.Application.Logic
{
    public class ContactService : RepositoryPagingServiceAsync<Contact, IUnitOfWorkAsync>, IContactService
    {
    }
}
