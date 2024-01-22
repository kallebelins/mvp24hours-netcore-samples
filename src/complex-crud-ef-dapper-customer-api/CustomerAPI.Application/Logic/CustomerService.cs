using AutoMapper;
using CustomerAPI.Core.Contract.Logic;
using CustomerAPI.Core.Entities;
using CustomerAPI.Core.Resources;
using CustomerAPI.Core.ValueObjects.Customers;
using CustomerAPI.Infrastructure.Extensions;
using Dapper;
using FluentValidation;
using Mvp24Hours.Application.Logic;
using Mvp24Hours.Core.Contract.Data;
using Mvp24Hours.Core.Contract.ValueObjects.Logic;
using Mvp24Hours.Core.Enums;
using Mvp24Hours.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CustomerAPI.Application.Logic
{
    public class CustomerService : RepositoryPagingServiceAsync<Customer, IUnitOfWorkAsync>, ICustomerService
    {
        #region [ Fields ]
        private readonly IMapper mapper;
        #endregion

        #region [ Ctor ]

        public CustomerService(IUnitOfWorkAsync unitOfWork, IValidator<Customer> validator, IMapper mapper)
            : base(unitOfWork, validator)
        {
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        #endregion

        #region [ Queries ]

        public async Task<IPagingResult<IEnumerable<CustomerResult>>> GetBy(CustomerQuery filter, IPagingCriteria criteria, CancellationToken cancellationToken = default)
        {
            var filterList = new List<string>
                {
                    "(@Active is null or Active = @Active) and (@Name is null or Name like CONCAT('%',@Name,'%'))"
                };

            // has cell
            if (filter.HasCellContact)
            {
                filterList.Add("((select count(0) from Contact where Contact.CustomerId = Customer.Id and Contact.Type = 0 and Contact.Active = 1) > 0 and Active = 1)");
            }

            // has email
            if (filter.HasEmailContact)
            {
                filterList.Add("((select count(0) from Contact where Contact.CustomerId = Customer.Id and Contact.Type = 3 and Contact.Active = 1) > 0 and Active = 1)");
            }

            // has no
            if (filter.HasNoContact)
            {
                filterList.Add("((select count(0) from Contact where Contact.CustomerId = Customer.Id and Contact.Active = 1) = 0)");
            }

            // is prospect
            if (filter.IsProspect)
            {
                filterList.Add("(Customer.Note like '%prospect%')");
            }

            var result = await UnitOfWork
                .GetConnection()
                .QueryPagingResultAsync<Customer>(criteria, string.Join(" and ", filterList), new { filter.Name, filter.Active });

            // checks if there are any records in the database from the filter
            if (result == null || result.Summary.TotalCount == 0)
            {
                // reply with standard message for record not found
                return Messages.RECORD_NOT_FOUND
                    .ToMessageResult(nameof(Messages.RECORD_NOT_FOUND), MessageType.Error)
                    .ToBusinessPaging<IEnumerable<CustomerResult>>();
            }

            return mapper
                .MapPagingTo<IEnumerable<Customer>, IEnumerable<CustomerResult>>(result);
        }

        public async Task<IBusinessResult<CustomerIdResult>> GetById(int id, CancellationToken cancellationToken = default)
        {
            // create projection for customer and contact by customer id
            string query = @"
                    select * from Customer where Id = @id;
                    select * from Contact where CustomerId = @id;
                ";

            Customer model = null;

            // apply multiple queries to load customer and contacts
            using (var result = await UnitOfWork
                .GetConnection()
                .QueryMultipleAsync(query, new { id }))
            {
                model = await result.ReadSingleOrDefaultAsync<Customer>();
                if (model != null)
                    model.Contacts = (await result.ReadAsync<Contact>())?.ToList();
            }

            if (model == null)
            {
                // reply with standard message for record not found
                return Messages.RECORD_NOT_FOUND
                    .ToMessageResult(nameof(Messages.RECORD_NOT_FOUND), MessageType.Error)
                    .ToBusiness<CustomerIdResult>();
            }

            // map result
            return mapper
                .Map<CustomerIdResult>(model)
                .ToBusiness();
        }

        #endregion

        #region [ Commands ]

        public async Task<IBusinessResult<int>> Create(CustomerCreate dto, CancellationToken cancellationToken = default)
        {
            var entity = mapper.Map<Customer>(dto);

            // apply data validation to the model/entity with FluentValidation or DataAnnotation
            var errors = entity.TryValidate(Validator);
            if (errors.AnySafe())
            {
                return errors.ToBusiness<int>();
            }

            // perform create action on the database
            await Repository.AddAsync(entity, cancellationToken: cancellationToken);
            if (await UnitOfWork.SaveChangesAsync(cancellationToken: cancellationToken) > 0)
            {
                return entity.Id.ToBusiness(
                    Messages.OPERATION_SUCCESS
                        .ToMessageResult(nameof(Messages.OPERATION_SUCCESS), MessageType.Success));
            }

            // unknown error
            return Messages.OPERATION_FAIL
                .ToMessageResult(nameof(Messages.OPERATION_FAIL), MessageType.Error)
                .ToBusiness<int>();
        }

        public async Task<IBusinessResult<int>> Update(int id, CustomerUpdate dto, CancellationToken cancellationToken = default)
        {
            // gets entity through the identifier informed in the resource
            var entity = await Repository.GetByIdAsync(id, cancellationToken: cancellationToken);
            if (entity == null)
            {
                return Messages.RECORD_NOT_FOUND_FOR_ID
                    .ToMessageResult(nameof(Messages.RECORD_NOT_FOUND_FOR_ID), MessageType.Error)
                        .ToBusiness<int>();
            }

            // entity populating with DTO properties
            mapper.Map(dto, entity);

            // apply data validation to the model/entity with FluentValidation or DataAnnotation
            var errors = entity.TryValidate(Validator);
            if (errors.AnySafe())
            {
                return errors.ToBusiness<int>();
            }

            // apply changes to database
            await Repository.ModifyAsync(entity, cancellationToken: cancellationToken);
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
