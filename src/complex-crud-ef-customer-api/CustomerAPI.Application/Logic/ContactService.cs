using CustomerAPI.Core.Contract.Logic;
using CustomerAPI.Core.Entities;
using CustomerAPI.Core.Resources;
using CustomerAPI.Core.ValueObjects.Contacts;
using FluentValidation;
using Mvp24Hours.Application.Logic;
using Mvp24Hours.Core.Contract.Data;
using Mvp24Hours.Core.Contract.Infrastructure.Contexts;
using Mvp24Hours.Core.Contract.Infrastructure.Logging;
using Mvp24Hours.Core.Contract.ValueObjects.Logic;
using Mvp24Hours.Core.Enums;
using Mvp24Hours.Extensions;
using Mvp24Hours.Helpers;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace CustomerAPI.Application.Logic
{
    public class ContactService : RepositoryPagingServiceAsync<Contact, IUnitOfWorkAsync>, IContactService
    {
        #region [ Ctor ]

        public ContactService(IUnitOfWorkAsync unitOfWork, ILoggingService logging, INotificationContext notification, IValidator<Contact> validator)
            : base(unitOfWork, logging, notification, validator)
        {
        }

        #endregion

        #region [ Queries ]

        public async Task<IBusinessResult<IList<ContactIdResult>>> GetBy(int customerId, CancellationToken cancellationToken = default)
        {
            // apply filter default
            Expression<Func<Contact, bool>> clause = x => x.CustomerId == customerId;

            // try to get paginated data with criteria
            var result = await GetByAsync(clause, cancellationToken: cancellationToken);

            // checks if there are any records in the database from the filter
            if (!result.HasData())
            {
                // reply with standard message for record not found
                return Messages.RECORD_NOT_FOUND.ToMessageResult(MessageType.Error)
                    .ToBusiness<IList<ContactIdResult>>();
            }

            // apply mapping
            return result.MapBusinessTo<IList<Contact>, IList<ContactIdResult>>();
        }

        #endregion

        #region [ Commands ]

        public async Task<IBusinessResult<int>> Create(int customerId, ContactCreate dto, CancellationToken cancellationToken = default)
        {
            var entity = dto.MapTo<Contact>();
            entity.CustomerId = customerId;

            // apply data validation to the model/entity with FluentValidation or DataAnnotation
            if (entity.Validate(NotificationContext, Validator)
                && await AddAsync(entity, cancellationToken: cancellationToken) > 0)
            {
                return entity.Id.ToBusiness(
                    Messages.OPERATION_SUCCESS
                        .ToMessageResult("ContactCreate", MessageType.Success));
            }

            // get message in request context, if not, use default message
            return NotificationContext
                .ToBusiness<int>(
                    defaultMessage: Messages.OPERATION_FAIL
                        .ToMessageResult(MessageType.Error)
            );
        }

        public async Task<IBusinessResult<int>> Update(int customerId, int id, ContactUpdate dto, CancellationToken cancellationToken = default)
        {
            // gets entity through the identifier informed in the resource
            var entity = await Repository
                .GetByAsync(x => x.Id == id && x.CustomerId == customerId, cancellationToken: cancellationToken)
                .FirstOrDefaultAsync();
            if (entity == null)
            {
                return Messages.RECORD_NOT_FOUND_FOR_ID
                    .ToMessageResult("NotFound", MessageType.Error)
                        .ToBusiness<int>();
            }

            // entity populating with DTO properties
            AutoMapperHelper.Map<Contact>(entity, dto);

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

        public async Task<IBusinessResult<int>> Delete(int customerId, int id, CancellationToken cancellationToken = default)
        {
            // try to retrieve entity by identifier
            var entity = await Repository
                .GetByAsync(x => x.Id == id && x.CustomerId == customerId, cancellationToken: cancellationToken)
                .FirstOrDefaultAsync();
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
