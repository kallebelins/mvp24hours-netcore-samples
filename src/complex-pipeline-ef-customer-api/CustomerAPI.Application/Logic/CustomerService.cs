using CustomerAPI.Application.Pipe.Operations.Customers;
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
using Mvp24Hours.Core.Contract.Infrastructure.Pipe;
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
        #region [ Fields / Properties ]

        private readonly IPipelineAsync pipeline;

        #endregion

        #region [ Ctors ]
        public CustomerService(IUnitOfWorkAsync unitOfWork, ILoggingService logging, INotificationContext notification, IPipelineAsync pipeline)
            : base(unitOfWork, logging, notification)
        {
            this.pipeline = pipeline;
        }
        #endregion

        #region [ Queries ]

        public async Task<IPagingResult<IList<CustomerResult>>> GetBy(CustomerQuery filter, IPagingCriteria criteria, CancellationToken cancellationToken = default)
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

            // apply filter with pagination
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

        public async Task<IBusinessResult<CustomerIdResult>> GetById(int id, CancellationToken cancellationToken = default)
        {
            // create criteria to load navigation (contact)
            var paging = new PagingCriteriaExpression<Customer>(3, 0);
            paging.NavigationExpr.Add(x => x.Contacts);

            // try to retrieve identifier with navigation property
            return await GetByIdAsync(id, paging, cancellationToken)
                .MapBusinessToAsync<Customer, CustomerIdResult>();
        }

        #endregion

        #region [ Pipelines ]

        public async Task<IBusinessResult<int>> RunDataSeed(CancellationToken cancellationToken = default)
        {
            // add operations/steps
            pipeline
                .Add<ValidateCustomerRepositoryStep>()
                .Add<GetCustomerClientStep>()
                .Add<GetByCustomerMapperResponseStep>()
                .Add<CreateCustomerRepositoryStep>();

            // run pipeline
            await pipeline.ExecuteAsync();

            // checks if there is a failure record in context
            if (NotificationContext.HasErrorNotifications)
            {
                return NotificationContext
                    .ToBusiness<int>(
                        defaultMessage: Messages.OPERATION_FAIL
                            .ToMessageResult(MessageType.Error)
                );
            }

            // try to get response content
            var numberOfRecords = pipeline.GetMessage()
                .GetContent<int>("numberOfRecords");

            // checks if there are any records
            if (numberOfRecords == 0)
            {
                // reply with standard message for record not found
                return Messages.RECORD_NOT_FOUND
                    .ToMessageResult(MessageType.Error)
                        .ToBusiness<int>();
            }

            return numberOfRecords.ToBusiness(
                Messages.OPERATION_SUCCESS
                    .ToMessageResult("RunDataSeed", MessageType.Success));
        }

        #endregion
    }
}
