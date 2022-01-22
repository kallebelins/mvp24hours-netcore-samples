using CustomerAPI.Core.Resources;
using CustomerAPI.Core.ValueObjects.Customers;
using Mvp24Hours.Core.Contract.Infrastructure.Pipe;
using Mvp24Hours.Extensions;
using Mvp24Hours.Infrastructure.Pipe.Operations.Custom;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CustomerAPI.Application.Pipe.Operations.Customers
{
    public class GetByCustomerMapperResponseStep : OperationMapperAsync<IList<GetByCustomerResponse>>
    {
        public override async Task<IList<GetByCustomerResponse>> MapperAsync(IPipelineMessage input)
        {
            if (!input.HasContent("customers"))
            {
                NotificationContext.Add("GetByCustomerMapperResponseStep", Messages.RECORD_NOT_FOUND, Mvp24Hours.Core.Enums.MessageType.Error);
                input.SetLock();
                return await Task.FromResult<IList<GetByCustomerResponse>>(default);
            }

            var customers = input.GetContent<dynamic>("customers");
            var filter = input.GetContent<GetByCustomerFilterRequest>();

            IList<GetByCustomerResponse> result = new List<GetByCustomerResponse>();

            foreach (var customer in customers)
            {
                if (filter != null)
                {
                    if (filter.HasCellContact && customer.phone == null)
                    {
                        continue;
                    }
                    if (filter.HasEmailContact && customer.email == null)
                    {
                        continue;
                    }
                    if (filter.Name.HasValue()
                        && customer.name != null
                        && !((string)customer.name).Contains(filter.Name, System.StringComparison.InvariantCultureIgnoreCase))
                    {
                        continue;
                    }
                }

                result.Add(new GetByCustomerResponse
                {
                    Id = customer.id,
                    Name = customer.name
                });
            }

            return await result.TaskResult();
        }
    }
}
