using CustomerAPI.Core.Contract.Logic;
using CustomerAPI.Core.Entities;
using CustomerAPI.Core.Resources;
using CustomerAPI.Core.Specifications.Customers;
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

        public async Task<IPagingResult<IList<Customer>>> GetBy(CustomerFilterModel model, IPagingCriteria criteria, CancellationToken cancellationToken = default)
        {
            try
            {
                // apply filter default
                Expression<Func<Customer, bool>> clause =
                    x => (string.IsNullOrEmpty(model.Name) || x.Name.Contains(model.Name))
                        && (model.Active == null || model.Active.Value);

                // has cell
                if (model.HasCellContact)
                {
                    clause = clause.And<Customer, CustomerHasCellContactSpec>();
                }

                // has email
                if (model.HasEmailContact)
                {
                    clause = clause.And<Customer, CustomerHasEmailContactSpec>();
                }

                // has no
                if (model.HasNoContact)
                {
                    clause = clause.And<Customer, CustomerHasNoContactSpec>();
                }

                // is prospect
                if (model.IsProspect)
                {
                    clause = clause.And<Customer, CustomerIsPropectSpec>();
                }

                // checks if there are any records in the database from the filter
                if (!await Repository.GetByAnyAsync(clause, cancellationToken: cancellationToken))
                {
                    // reply with standard message for record not found
                    return Messages.RECORD_NOT_FOUND.ToMessageResult(MessageType.Error)
                        .ToBusinessPaging<IList<Customer>>();
                }

                // apply filter with pagination
                return await GetByWithPaginationAsync(clause, criteria, cancellationToken: cancellationToken);
            }
            catch (Exception ex)
            {
                Logging.Error(ex);
                throw ex;
            }
        }

        public async Task<IBusinessResult<Customer>> GetById(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                // create criteria to load navigation (contact)
                var paging = new PagingCriteriaExpression<Customer>(3, 0);
                paging.NavigationExpr.Add(x => x.Contacts);

                // try to retrieve identifier with navigation property
                return await GetByIdAsync(id, paging, cancellationToken);
            }
            catch (Exception ex)
            {
                Logging.Error(ex);
                throw ex;
            }
        }

        #endregion

        #region [ Commands ]

        public async Task<IBusinessResult<int>> Create(Customer entityModel, CancellationToken cancellationToken = default)
        {
            // sets default values
            entityModel.Active = true;
            entityModel.Created = DateTime.Now;

            // apply data validation to the model/entity with FluentValidation or DataAnnotation
            if (entityModel.Validate())
            {
                if (await AddAsync(entityModel, cancellationToken: cancellationToken) > 0)
                {
                    return entityModel.Id.ToBusiness(
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

        public async Task<IBusinessResult<int>> Update(int id, Customer entityModel, CancellationToken cancellationToken = default)
        {
            // gets entity through the identifier informed in the resource
            var entityDb = await Repository.GetByIdAsync(id, cancellationToken: cancellationToken);
            if (entityDb == null)
            {
                return Messages.RECORD_NOT_FOUND_FOR_ID
                    .ToMessageResult("NotFound", MessageType.Error)
                        .ToBusiness<int>();
            }

            // overwrites values that cannot be changed
            entityModel.Id = id;
            entityModel.Created = entityDb.Created;

            // entity filling with received entity properties as input
            AutoMapperHelper.Map<Customer>(entityDb, entityModel);

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
