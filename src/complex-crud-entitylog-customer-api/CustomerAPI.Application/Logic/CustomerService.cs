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

namespace CustomerAPI.Application.Logic
{
    public class CustomerService : RepositoryPagingServiceAsync<Customer, IUnitOfWorkAsync>, ICustomerService
    {
        #region [ Queries ]

        public async Task<IPagingResult<IList<GetByCustomerResponse>>> GetBy(GetByCustomerRequest filter, IPagingCriteria criteria)
        {
            try
            {
                // apply filter default
                Expression<Func<Customer, bool>> clause =
                    x => (string.IsNullOrEmpty(filter.Name) || x.Name.Contains(filter.Name))
                        && (filter.Active == null || filter.Active.Value);

                // has cell
                if (filter.HasCellContact)
                {
                    clause = clause.And<Customer, CustomerHasCellContactSpec>();
                }

                // has email
                if (filter.HasEmailContact)
                {
                    clause = clause.And<Customer, CustomerHasEmailContactSpec>();
                }

                // has no
                if (filter.HasNoContact)
                {
                    clause = clause.And<Customer, CustomerHasNoContactSpec>();
                }

                // is prospect
                if (filter.IsProspect)
                {
                    clause = clause.And<Customer, CustomerIsPropectSpec>();
                }

                return await GetByWithPaginationAsync(clause, criteria)
                    .MapPagingToAsync<IList<Customer>, IList<GetByCustomerResponse>>();
            }
            catch (Exception ex)
            {
                Logging.Error(ex);
                throw ex;
            }
        }

        public async Task<IBusinessResult<GetByIdCustomerResponse>> GetById(int id)
        {
            try
            {
                return await GetByIdAsync(id)
                    .MapBusinessToAsync<Customer, GetByIdCustomerResponse>();
            }
            catch (Exception ex)
            {
                Logging.Error(ex);
                throw ex;
            }
        }

        #endregion

        #region [ Commands ]

        public async Task<IBusinessResult<int>> Create(CreateCustomerRequest dto, CancellationToken cancellationToken = default)
        {
            var entity = dto.MapTo<Customer>();

            if (await AddAsync(entity) > 0)
            {
                return entity.Id.ToBusiness(Messages.OPERATION_SUCCESS.ToMessageResult("CustomerCreate", MessageType.Success));
            }
            return Messages.OPERATION_FAIL
                .ToMessageResult("CustomerCreate", MessageType.Error)
                .ToBusiness<int>();
        }

        public async Task<IBusinessResult<VoidResult>> Update(int id, UpdateCustomerRequest dto, CancellationToken cancellationToken = default)
        {
            var entity = await Repository.GetByIdAsync(id);
            if (entity == null)
            {
                return Messages.OPERATION_FAIL_NOT_FOUND
                    .ToMessageResult("Update", MessageType.Error)
                    .ToBusiness<VoidResult>();
            }

            AutoMapperHelper.Map<Customer>(entity, dto);

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

        public async Task<IBusinessResult<VoidResult>> Delete(int id, CancellationToken cancellationToken = default)
        {
            var entity = await Repository.GetByIdAsync(id);
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
