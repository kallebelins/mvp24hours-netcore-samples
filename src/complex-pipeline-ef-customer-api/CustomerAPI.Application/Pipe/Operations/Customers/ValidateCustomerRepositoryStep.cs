using CustomerAPI.Core.Entities;
using CustomerAPI.Core.Resources;
using Mvp24Hours.Core.Contract.Data;
using Mvp24Hours.Core.Contract.Infrastructure.Pipe;
using Mvp24Hours.Helpers;
using Mvp24Hours.Infrastructure.Pipe.Operations;
using System.Threading.Tasks;

namespace CustomerAPI.Application.Pipe.Operations.Customers
{
    /// <summary>
    /// Checks if records already exist in the database.
    /// </summary>
    public class ValidateCustomerRepositoryStep : OperationBaseAsync
    {
        private readonly IUnitOfWorkAsync unitOfWorkAsync;

        public ValidateCustomerRepositoryStep()
        {
            /*
                // you can inject to get through the constructor
                services.AddScoped(x =>
                    new CreateCustomerRepositoryStep(x.GetRequiredService<IUnitOfWorkAsync>())
                );
            */

            this.unitOfWorkAsync = ServiceProviderHelper.GetService<IUnitOfWorkAsync>();
        }

        public override async Task ExecuteAsync(IPipelineMessage input)
        {
            var repo = unitOfWorkAsync.GetRepository<Customer>();

            if (await repo.ListAnyAsync())
            {
                NotificationContext.Add("ValidateCustomerRepositoryStep", Messages.RECORD_NOT_SEED_DATA, Mvp24Hours.Core.Enums.MessageType.Error);
                input.SetLock();
            }
        }
    }
}
