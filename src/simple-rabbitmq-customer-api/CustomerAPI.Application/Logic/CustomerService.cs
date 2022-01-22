using CustomerAPI.Core.Contract.Logic;
using CustomerAPI.Core.Entities;
using CustomerAPI.Core.Resources;
using CustomerAPI.Core.ValueObjects.Customers;
using Mvp24Hours.Application.Logic;
using Mvp24Hours.Core.Contract.Data;
using Mvp24Hours.Core.Contract.ValueObjects.Logic;
using Mvp24Hours.Core.Enums;
using Mvp24Hours.Core.ValueObjects.Logic;
using Mvp24Hours.Extensions;
using Mvp24Hours.Helpers;
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

        public async Task<IPagingResult<IList<GetByCustomerResponse>>> GetBy(GetByCustomerRequest filter, IPagingCriteria criteria, CancellationToken cancellationToken = default)
        {
            try
            {
                // apply filter default
                Expression<Func<Customer, bool>> clause =
                    x => (string.IsNullOrEmpty(filter.Name) || x.Name.Contains(filter.Name))
                        && (filter.Active == null || filter.Active.Value);

                // checks if there are any records in the database from the filter
                if (!await Repository.GetByAnyAsync(clause, cancellationToken: cancellationToken))
                {
                    // reply with standard message for record not found
                    return Messages.RECORD_NOT_FOUND.ToMessageResult(MessageType.Error)
                        .ToBusinessPaging<IList<GetByCustomerResponse>>();
                }

                // apply filter with pagination
                return await GetByWithPaginationAsync(clause, criteria, cancellationToken: cancellationToken)
                    .MapPagingToAsync<IList<Customer>, IList<GetByCustomerResponse>>();
            }
            catch (Exception ex)
            {
                Logging.Error(ex);
                throw ex;
            }
        }

        public async Task<IBusinessResult<GetByIdCustomerResponse>> GetById(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                // try to retrieve identifier with navigation property
                return await GetByIdAsync(id, cancellationToken)
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

            // apply data validation to the model/entity with FluentValidation or DataAnnotation
            if (entity.Validate())
            {
                if (await AddAsync(entity, cancellationToken: cancellationToken) > 0)
                {
                    return entity.Id.ToBusiness(
                        Messages.OPERATION_SUCCESS
                            .ToMessageResult("CustomerCreate", MessageType.Success));
                }
            }

            // get message in request context, if not, use default message
            return NotificationContext
                .ToBusiness<int>(
                    defaultMessage: Messages.OPERATION_FAIL
                        .ToMessageResult(MessageType.Error)
            );
        }

        public async Task<IBusinessResult<int>> Update(int id, UpdateCustomerRequest dto, CancellationToken cancellationToken = default)
        {
            // gets entity through the identifier informed in the resource
            var entity = await Repository.GetByIdAsync(id, cancellationToken: cancellationToken);
            if (entity == null)
            {
                return Messages.RECORD_NOT_FOUND_FOR_ID
                    .ToMessageResult("NotFound", MessageType.Error)
                        .ToBusiness<int>();
            }

            // preenchimento entidade com propriedades do DTO
            AutoMapperHelper.Map<Customer>(entity, dto);

            // apply data validation to the model/entity with FluentValidation or DataAnnotation
            if (entity.Validate())
            {
                // apply changes to database
                int affectedRows = await ModifyAsync(entity, cancellationToken: cancellationToken);
                if (affectedRows > 0)
                {
                    return affectedRows.ToBusiness(
                        Messages.OPERATION_SUCCESS
                            .ToMessageResult("Update", MessageType.Success));
                }
                else
                {
                    return affectedRows.ToBusiness();
                }
            }

            // get message in request context, if not, use default message
            return NotificationContext
                .ToBusiness<int>(
                    defaultMessage: Messages.OPERATION_FAIL
                        .ToMessageResult(MessageType.Error)
            );
        }

        public async Task<IBusinessResult<int>> Delete(int id, CancellationToken cancellationToken = default)
        {
            // try to retrieve entity by identifier
            var entity = await Repository.GetByIdAsync(id);
            if (entity == null)
            {
                return Messages.RECORD_NOT_FOUND_FOR_ID
                    .ToMessageResult("NotFound", MessageType.Error)
                        .ToBusiness<int>();
            }

            // perform delete action
            int affectedRows = await RemoveAsync(entity, cancellationToken: cancellationToken);
            if (affectedRows > 0)
            {
                return affectedRows.ToBusiness(
                    Messages.OPERATION_SUCCESS
                        .ToMessageResult("Delete", MessageType.Success));
            }

            // get message in request context, if not, use default message
            return NotificationContext
                .ToBusiness<int>(
                    defaultMessage: Messages.OPERATION_FAIL
                        .ToMessageResult(MessageType.Error)
            );
        }

        #endregion
    }
}
