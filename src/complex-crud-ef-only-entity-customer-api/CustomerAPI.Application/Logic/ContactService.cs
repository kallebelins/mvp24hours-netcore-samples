using CustomerAPI.Core.Contract.Logic;
using CustomerAPI.Core.Entities;
using CustomerAPI.Core.Resources;
using Mvp24Hours.Application.Logic;
using Mvp24Hours.Core.Contract.Data;
using Mvp24Hours.Core.Contract.ValueObjects.Logic;
using Mvp24Hours.Core.DTOs;
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
    public class ContactService : RepositoryPagingServiceAsync<Contact, IUnitOfWorkAsync>, IContactService
    {
        #region [ Queries ]

        public async Task<IBusinessResult<IList<Contact>>> GetBy(int customerId, CancellationToken cancellationToken = default)
        {
            // apply filter default
            Expression<Func<Contact, bool>> clause = x => x.CustomerId == customerId;

            // checks if there are any records in the database from the filter
            if (!await Repository.GetByAnyAsync(clause, cancellationToken: cancellationToken))
            {
                // reply with standard message for record not found
                return Messages.RECORD_NOT_FOUND.ToMessageResult(MessageType.Error)
                    .ToBusinessPaging<IList<Contact>>();
            }

            // apply filter
            return await GetByAsync(clause, cancellationToken: cancellationToken);
        }

        #endregion

        #region [ Commands ]

        public async Task<IBusinessResult<int>> Create(int customerId, Contact entityModel, CancellationToken cancellationToken = default)
        {
            // sets default values
            entityModel.Active = true;
            entityModel.Created = DateTime.Now;
            entityModel.CustomerId = customerId;

            // apply data validation to the model/entity with FluentValidation or DataAnnotation
            if (entityModel.Validate())
            {
                if (await AddAsync(entityModel, cancellationToken: cancellationToken) > 0)
                {
                    return entityModel.Id.ToBusiness(
                        messageResult: Messages.OPERATION_SUCCESS
                            .ToMessageResult("ContactCreate", MessageType.Success));
                }
            }

            // get message in request context, if not, use default message
            return NotificationContext
                .ToBusiness<int>(
                    defaultMessage: Messages.OPERATION_FAIL
                        .ToMessageResult(MessageType.Error)
            );
        }

        public async Task<IBusinessResult<int>> Update(int customerId, int id, Contact entityModel, CancellationToken cancellationToken = default)
        {
            // gets entity through the identifier informed in the resource
            var entityDb = await Repository
                .GetByAsync(x => x.Id == id && x.CustomerId == customerId, cancellationToken: cancellationToken)
                .FirstOrDefaultAsync();
            if (entityDb == null)
            {
                return Messages.RECORD_NOT_FOUND_FOR_ID
                    .ToMessageResult("NotFound", MessageType.Error)
                        .ToBusiness<int>();
            }

            // overwrites values that cannot be changed
            entityModel.Id = id;
            entityModel.CustomerId = customerId;
            entityModel.Created = entityDb.Created;

            // entity filling with received entity properties as input
            AutoMapperHelper.Map<Contact>(entityDb, entityModel);

            // apply data validation to the model/entity with FluentValidation or DataAnnotation
            if (entityDb.Validate())
            {
                // apply changes to database
                int affectedRows = await ModifyAsync(entityDb, cancellationToken: cancellationToken);
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
