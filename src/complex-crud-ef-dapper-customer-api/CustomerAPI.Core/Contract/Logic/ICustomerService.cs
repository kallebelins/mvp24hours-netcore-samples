﻿using CustomerAPI.Core.ValueObjects.Customers;
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
        Task<IPagingResult<IEnumerable<GetByCustomerResponse>>> GetBy(GetByCustomerRequest filter, IPagingCriteria criteria, CancellationToken cancellationToken = default);
        Task<IBusinessResult<GetByIdCustomerResponse>> GetById(int id, CancellationToken cancellationToken = default);
        Task<IBusinessResult<int>> Create(CreateCustomerRequest dto, CancellationToken cancellationToken = default);
        Task<IBusinessResult<int>> Update(int id, UpdateCustomerRequest dto, CancellationToken cancellationToken = default);
        Task<IBusinessResult<int>> Delete(int id, CancellationToken cancellationToken = default);
    }
}
