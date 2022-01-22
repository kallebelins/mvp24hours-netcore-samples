using CustomerAPI.Core.Contract.Logic;
using CustomerAPI.Core.Entities;
using CustomerAPI.Core.Resources;
using CustomerAPI.Core.ValueObjects.Customers;
using CustomerAPI.Infrastructure.Extensions;
using Dapper;
using Mvp24Hours.Application.Logic;
using Mvp24Hours.Core.Contract.Data;
using Mvp24Hours.Core.Contract.ValueObjects.Logic;
using Mvp24Hours.Core.Enums;
using Mvp24Hours.Extensions;
using Mvp24Hours.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CustomerAPI.Application.Logic
{
    public class CustomerService : RepositoryPagingServiceAsync<Customer, IUnitOfWorkAsync>, ICustomerService
    {
        #region [ Queries ]

        public async Task<IPagingResult<IEnumerable<GetByCustomerResponse>>> GetBy(GetByCustomerRequest filter, IPagingCriteria criteria, CancellationToken cancellationToken = default)
        {
            try
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
                    return Messages.RECORD_NOT_FOUND.ToMessageResult(MessageType.Error)
                        .ToBusinessPaging<IEnumerable<GetByCustomerResponse>>();
                }

                return result
                    .MapPagingTo<IEnumerable<Customer>, IEnumerable<GetByCustomerResponse>>();
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
                    model.Contacts = (await result.ReadAsync<Contact>())?.ToList();
                }

                if (model == null)
                {
                    // reply with standard message for record not found
                    return Messages.RECORD_NOT_FOUND.ToMessageResult(MessageType.Error)
                        .ToBusiness<GetByIdCustomerResponse>();
                }

                // map result
                return model.MapTo<GetByIdCustomerResponse>()
                    .ToBusiness();
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
