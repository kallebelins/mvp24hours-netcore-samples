using CustomerAPI.Core.Contract.Logic;
using CustomerAPI.Core.Entities;
using CustomerAPI.Core.Resources;
using CustomerAPI.Core.Specifications.Customers;
using CustomerAPI.Core.ValueObjects.Customers;
using FluentValidation;
using Mvp24Hours.Application.Logic;
using Mvp24Hours.Core.Contract.Data;
using Mvp24Hours.Core.Contract.Infrastructure.Contexts;
using Mvp24Hours.Core.Contract.Infrastructure.Logging;
using Mvp24Hours.Core.Contract.ValueObjects.Logic;
using Mvp24Hours.Core.Enums;
using Mvp24Hours.Extensions;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace CustomerAPI.Application.Logic
{
    public class CustomerService : RepositoryPagingServiceAsync<Customer, IUnitOfWorkAsync>, ICustomerService
    {
        #region [ Ctor ]

        public CustomerService(IUnitOfWorkAsync unitOfWork, ILoggingService logging, INotificationContext notification, IValidator<Customer> validator)
            : base(unitOfWork, logging, notification, validator)
        {
        }

        #endregion

        #region [ Queries ]

        public async Task<IPagingResult<IList<CustomerResult>>> GetBy(CustomerQuery filter, IPagingCriteria criteria, CancellationToken cancellationToken = default)
        {
            // apply filter default
            Expression<Func<Customer, bool>> clause = x => true;

            if (filter.Name.HasValue())
            {
                clause = clause.And(x => x.Name.Contains(filter.Name));
            }
            if (filter.Active.HasValue)
            {
                clause = clause.And(x => x.Active == filter.Active);
            }

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

            // try to get the paginated data
            var result = await GetByWithPaginationAsync(clause, criteria, cancellationToken: cancellationToken);

            // checks if there are any records in the database from the filter
            if (!result.HasData())
            {
                // reply with standard message for record not found
                return Messages.RECORD_NOT_FOUND.ToMessageResult(MessageType.Error)
                    .ToBusinessPaging<IList<CustomerResult>>();
            }

            // apply mapping
            return result.MapPagingTo<IList<Customer>, IList<CustomerResult>>();
        }

        public async Task<IBusinessResult<CustomerIdResult>> GetById(string id, CancellationToken cancellationToken = default)
        {
            // try to retrieve identifier
            return await GetByIdAsync(id, cancellationToken)
                .MapBusinessToAsync<Customer, CustomerIdResult>();
        }

        #endregion

        #region [ Commands ]

        public async Task<IBusinessResult<string>> Create(CustomerCreate dto, CancellationToken cancellationToken = default)
        {
            var entity = dto.MapTo<Customer>();

            // apply data validation to the model/entity with FluentValidation or DataAnnotation
            if (entity.Validate(NotificationContext, Validator)
                && await AddAsync(entity, cancellationToken: cancellationToken) > 0)
            {
                return entity.Id.ToString().ToBusiness(
                    Messages.OPERATION_SUCCESS
                        .ToMessageResult("CustomerCreate", MessageType.Success));
            }

            // get message in request context, if not, use default message
            return NotificationContext
                .ToBusiness<string>(
                    defaultMessage: Messages.OPERATION_FAIL
                        .ToMessageResult(MessageType.Error)
            );
        }

        public async Task<IBusinessResult<int>> Update(string id, CustomerUpdate dto, CancellationToken cancellationToken = default)
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
            dto.CopyPropertiesTo(entity);

            // apply data validation to the model/entity with FluentValidation or DataAnnotation
            if (entity.Validate(NotificationContext, Validator))
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

        public async Task<IBusinessResult<int>> Delete(string id, CancellationToken cancellationToken = default)
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
