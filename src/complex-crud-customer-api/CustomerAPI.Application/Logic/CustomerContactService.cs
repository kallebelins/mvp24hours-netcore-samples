using CustomerAPI.Core.Contract.Logic;
using CustomerAPI.Core.Entities;
using CustomerAPI.Core.Resources;
using CustomerAPI.Core.Specifications.Customers;
using CustomerAPI.Core.ValueObjects.Contacts;
using CustomerAPI.Core.ValueObjects.Customers;
using Mvp24Hours.Application.Logic;
using Mvp24Hours.Core.Contract.Data;
using Mvp24Hours.Core.Contract.ValueObjects.Logic;
using Mvp24Hours.Core.DTOs;
using Mvp24Hours.Core.Enums;
using Mvp24Hours.Infrastructure.Extensions;
using Mvp24Hours.Infrastructure.Helpers;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace CustomerAPI.Application.Logic
{
    public class CustomerContactService : RepositoryPagingServiceAsync<Contact, IUnitOfWorkAsync>, ICustomerContactService
    {
        #region [ Commands ]

        public async Task<IBusinessResult<int>> Create(int customerId, CreateContactRequest dto, CancellationToken cancellationToken = default)
        {
            var entity = dto.MapTo<Contact>();
            entity.CustomerId = customerId;

            if (await AddAsync(entity) > 0)
            {
                return entity.Id.ToBusiness(Messages.OPERATION_SUCCESS.ToMessageResult("ContactCreate", MessageType.Success));
            }
            return Messages.OPERATION_FAIL
                .ToMessageResult("ContactCreate", MessageType.Error)
                .ToBusiness<int>();
        }

        public async Task<IBusinessResult<VoidResult>> Update(int customerId, int id, UpdateContactRequest dto, CancellationToken cancellationToken = default)
        {
            var entity = await Repository.GetByAsync(x => x.Id == id && x.CustomerId == customerId);
            if (entity == null)
            {
                return Messages.OPERATION_FAIL_NOT_FOUND
                    .ToMessageResult("Update", MessageType.Error)
                    .ToBusiness<VoidResult>();
            }

            AutoMapperHelper.Map<Contact>(entity, dto);

            if (await ModifyAsync(entity) > 0)
            {
                return Messages.OPERATION_SUCCESS
                    .ToMessageResult("Update", MessageType.Success)
                    .ToBusiness<VoidResult>();
            }

            return Messages.OPERATION_FAIL
                .ToMessageResult("Update", MessageType.Warning)
                .ToBusiness<VoidResult>();
        }

        public async Task<IBusinessResult<VoidResult>> Delete(int customerId, int id, CancellationToken cancellationToken = default)
        {
            var entity = await Repository.GetByAsync(x => x.Id == id && x.CustomerId == customerId);
            if (entity == null)
            {
                return Messages.OPERATION_FAIL_NOT_FOUND
                    .ToMessageResult("Delete", MessageType.Error)
                    .ToBusiness<VoidResult>();
            }

            if (await RemoveAsync(entity) > 0)
            {
                return Messages.OPERATION_SUCCESS
                    .ToMessageResult("Delete", MessageType.Success)
                    .ToBusiness<VoidResult>();
            }

            return Messages.OPERATION_FAIL
                .ToMessageResult("Delete", MessageType.Error)
                .ToBusiness<VoidResult>();
        }

        #endregion
    }
}
