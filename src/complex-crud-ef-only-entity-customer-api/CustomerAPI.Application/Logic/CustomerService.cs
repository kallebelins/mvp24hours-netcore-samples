﻿using CustomerAPI.Core.Contract.Logic;
using CustomerAPI.Core.Entities;
using CustomerAPI.Core.Resources;
using CustomerAPI.Core.Specifications.Customers;
using CustomerAPI.Core.ValueObjects.Customers;
using FluentValidation;
using Mvp24Hours.Application.Logic;
using Mvp24Hours.Core.Contract.Data;
using Mvp24Hours.Core.Contract.ValueObjects.Logic;
using Mvp24Hours.Core.Enums;
using Mvp24Hours.Core.ValueObjects.Logic;
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

        public CustomerService(IUnitOfWorkAsync unitOfWork, IValidator<Customer> validator)
            : base(unitOfWork, validator)
        {
        }

        #endregion

        #region [ Queries ]

        public async Task<IPagingResult<IList<Customer>>> GetBy(CustomerQuery model, IPagingCriteria criteria, CancellationToken cancellationToken = default)
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

            // try to get paginated data with criteria
            var result = await GetByWithPaginationAsync(clause, criteria, cancellationToken: cancellationToken);

            // checks if there are any records in the database from the filter
            if (!result.HasData())
            {
                // reply with standard message for record not found
                return Messages.RECORD_NOT_FOUND
                    .ToMessageResult(nameof(Messages.RECORD_NOT_FOUND), MessageType.Error)
                    .ToBusinessPaging<IList<Customer>>();
            }

            return result;
        }

        public async Task<IBusinessResult<Customer>> GetById(int id, CancellationToken cancellationToken = default)
        {
            // create criteria to load navigation (contact)
            var paging = new PagingCriteriaExpression<Customer>(3, 0);
            paging.NavigationExpr.Add(x => x.Contacts);

            // try to retrieve identifier with navigation property
            return await GetByIdAsync(id, paging, cancellationToken);
        }

        #endregion

        #region [ Commands ]

        public async Task<IBusinessResult<int>> Create(Customer entityModel, CancellationToken cancellationToken = default)
        {
            // sets default values
            entityModel.Active = true;
            entityModel.Created = DateTime.Now;

            // apply data validation to the model/entity with FluentValidation or DataAnnotation
            var errors = entityModel.TryValidate(Validator);
            if (errors.AnySafe())
            {
                return errors.ToBusiness<int>();
            }

            // perform create action on the database
            await Repository.AddAsync(entityModel, cancellationToken: cancellationToken);
            if (await UnitOfWork.SaveChangesAsync(cancellationToken: cancellationToken) > 0)
            {
                return entityModel.Id.ToBusiness(
                    Messages.OPERATION_SUCCESS
                        .ToMessageResult(nameof(Messages.OPERATION_SUCCESS), MessageType.Success));
            }

            // unknown error
            return Messages.OPERATION_FAIL
                .ToMessageResult(nameof(Messages.OPERATION_FAIL), MessageType.Error)
                .ToBusiness<int>();
        }

        public async Task<IBusinessResult<int>> Update(int id, Customer entityModel, CancellationToken cancellationToken = default)
        {
            // gets entity through the identifier informed in the resource
            var entityDb = await Repository.GetByIdAsync(id, cancellationToken: cancellationToken);
            if (entityDb == null)
            {
                return Messages.RECORD_NOT_FOUND_FOR_ID
                    .ToMessageResult(nameof(Messages.RECORD_NOT_FOUND_FOR_ID), MessageType.Error)
                        .ToBusiness<int>();
            }

            // overwrites values that cannot be changed
            entityModel.Id = id;
            entityModel.Created = entityDb.Created;

            // entity filling with received entity properties as input
            entityModel.CopyPropertiesTo(entityDb);

            // apply data validation to the model/entity with FluentValidation or DataAnnotation
            var errors = entityDb.TryValidate(Validator);
            if (errors.AnySafe())
            {
                return errors.ToBusiness<int>();
            }

            // apply changes to database
            await Repository.ModifyAsync(entityDb, cancellationToken: cancellationToken);
            int affectedRows = await UnitOfWork.SaveChangesAsync(cancellationToken: cancellationToken);
            if (affectedRows > 0)
            {
                return affectedRows.ToBusiness(
                    Messages.OPERATION_SUCCESS
                        .ToMessageResult(nameof(Messages.OPERATION_SUCCESS), MessageType.Success));
            }

            // unknown error
            return Messages.OPERATION_FAIL
                .ToMessageResult(nameof(Messages.OPERATION_FAIL), MessageType.Error)
                .ToBusiness<int>();
        }

        public async Task<IBusinessResult<int>> Delete(int id, CancellationToken cancellationToken = default)
        {
            // try to retrieve entity by identifier
            var entity = await Repository.GetByIdAsync(id);
            if (entity == null)
            {
                return Messages.RECORD_NOT_FOUND_FOR_ID
                    .ToMessageResult(nameof(Messages.RECORD_NOT_FOUND_FOR_ID), MessageType.Error)
                        .ToBusiness<int>();
            }

            // performs delete action on the database
            await Repository.RemoveAsync(entity, cancellationToken: cancellationToken);
            int affectedRows = await UnitOfWork.SaveChangesAsync(cancellationToken: cancellationToken);
            if (affectedRows > 0)
            {
                return affectedRows.ToBusiness(
                    Messages.OPERATION_SUCCESS
                        .ToMessageResult(nameof(Messages.OPERATION_SUCCESS), MessageType.Success));
            }

            // unknown error
            return Messages.OPERATION_FAIL
                .ToMessageResult(nameof(Messages.OPERATION_FAIL), MessageType.Error)
                .ToBusiness<int>();
        }

        #endregion
    }
}
