using CustomerAPI.Core.Entities;
using CustomerAPI.Core.ValueObjects.Customers;
using Mvp24Hours.Core.Contract.Data;
using Mvp24Hours.Core.Contract.Infrastructure.Pipe;
using Mvp24Hours.Extensions;
using Mvp24Hours.Helpers;
using Mvp24Hours.Infrastructure.Pipe.Operations;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CustomerAPI.Application.Pipe.Operations.Customers
{
    /// <summary>
    /// Create client in data store
    /// </summary>
    public class CreateCustomerRepositoryStep : OperationBaseAsync
    {
        private readonly IUnitOfWorkAsync unitOfWorkAsync;

        public CreateCustomerRepositoryStep()
        {
            /*
                // you can inject to get through the constructor
                services.AddScoped(x =>
                    new CreateCustomerRepositoryStep(x.GetRequiredService<IUnitOfWorkAsync>())
                );
            */

            this.unitOfWorkAsync = ServiceProviderHelper.GetService<IUnitOfWorkAsync>();
        }

        public override async Task<IPipelineMessage> ExecuteAsync(IPipelineMessage input)
        {
            if (!input.HasContent("model-customers"))
            {
                input.SetLock();
                return input;
            }

            var customers = input.GetContent<IList<CreateCustomerRequest>>("model-customers");

            var repo = unitOfWorkAsync.GetRepository<Customer>();

            foreach (var c in customers)
            {
                await repo.AddAsync(c.MapTo<Customer>());
            }

            int numberOfRecords = await unitOfWorkAsync.SaveChangesAsync();
            input.AddContent("numberOfRecords", numberOfRecords);

            return input;
        }
    }
}
