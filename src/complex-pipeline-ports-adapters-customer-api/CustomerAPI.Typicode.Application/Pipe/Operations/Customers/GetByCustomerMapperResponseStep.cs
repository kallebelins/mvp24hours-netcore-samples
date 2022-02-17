using CustomerAPI.Core.Resources;
using CustomerAPI.Core.ValueObjects.Customers;
using Mvp24Hours.Core.Contract.Infrastructure.Pipe;
using Mvp24Hours.Extensions;
using Mvp24Hours.Infrastructure.Pipe.Operations.Custom;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CustomerAPI.Typicode.Application.Pipe.Operations.Customers
{
    public class GetByCustomerMapperResponseStep : OperationMapperAsync<IList<CustomerResult>>
    {
        public override async Task<IList<CustomerResult>> MapperAsync(IPipelineMessage input)
        {
            if (!input.HasContent("customers"))
            {
                NotificationContext.Add("GetByCustomerMapperResponseStep", Messages.RECORD_NOT_FOUND, Mvp24Hours.Core.Enums.MessageType.Error);
                input.SetLock();
                return await Task.FromResult<IList<CustomerResult>>(default);
            }

            var customers = input.GetContent<dynamic>("customers");
            var filter = input.GetContent<CustomerQuery>();

            IList<CustomerResult> result = new List<CustomerResult>();

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

                result.Add(new CustomerResult
                {
                    Id = customer.id,
                    Name = customer.name
                });
            }

            return await result.TaskResult();
        }
    }
}
