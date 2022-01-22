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
        Task<IBusinessResult<IList<GetByCustomerResponse>>> GetBy(GetByCustomerFilterRequest filter);
        Task<IBusinessResult<GetByIdCustomerResponse>> GetById(int id);
    }
}
