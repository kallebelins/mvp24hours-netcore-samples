using CustomerAPI.Application.Pipe.Operations.Customers;
using CustomerAPI.Core.Contract.Logic;
using CustomerAPI.Core.Resources;
using CustomerAPI.Core.ValueObjects.Customers;
using Mvp24Hours.Core.Contract.Infrastructure.Pipe;
using Mvp24Hours.Core.Contract.ValueObjects.Logic;
using Mvp24Hours.Core.Enums;
using Mvp24Hours.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CustomerAPI.Application.Logic
{
    public class CustomerService : ICustomerService
    {
        #region [ Fields / Properties ]

        private readonly IPipelineAsync pipeline;

        #endregion

        #region [ Ctors ]

        /// <summary>
        /// 
        /// </summary>
        public CustomerService(IPipelineAsync _pipeline)
        {
            pipeline = _pipeline;
        }

        #endregion

        #region [ Actions ]

        public async Task<IBusinessResult<IList<CustomerResult>>> GetBy(CustomerQuery filter)
        {
            // add operations/steps
            pipeline.Add<GetCustomerClientStep>();
            pipeline.Add<GetByCustomerMapperResponseStep>();

            // run pipeline with package with content (int -> id)
            await pipeline.ExecuteAsync(filter.ToMessage());

            // try to get response content
            IList<CustomerResult> result = pipeline.GetMessage()
                .GetContent<List<CustomerResult>>();

            // checks if there are any records
            if (!result.AnyOrNotNull())
            {
                // reply with standard message for record not found
                return Messages.RECORD_NOT_FOUND
                    .ToMessageResult(MessageType.Error)
                        .ToBusiness<IList<CustomerResult>>();
            }

            return result.ToBusiness();
        }

        public async Task<IBusinessResult<CustomerIdResult>> GetById(int id)
        {
            // add operations/steps
            pipeline.Add<GetCustomerClientStep>();
            pipeline.Add<GetByIdCustomerMapperResponseStep>();

            // run pipeline with package with content (int -> id)
            await pipeline.ExecuteAsync(id.ToMessage("id"));

            // try to get response content
            var result = pipeline.GetMessage()
                .GetContent<CustomerIdResult>();

            // checks if there are any records
            if (result == null)
            {
                // reply with standard message for record not found
                return Messages.RECORD_NOT_FOUND_FOR_ID
                    .ToMessageResult(MessageType.Error)
                        .ToBusiness<CustomerIdResult>();
            }
            return result.ToBusiness();
        }

        #endregion
    }
}
