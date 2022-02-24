using CustomerAPI.Core.Entities;
using CustomerAPI.Core.Resources;
using Mvp24Hours.Core.Contract.Data;
using Mvp24Hours.Core.Contract.Infrastructure.Pipe;
using Mvp24Hours.Extensions;
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

        public ValidateCustomerRepositoryStep(IUnitOfWorkAsync unitOfWorkAsync)
        {
            this.unitOfWorkAsync = unitOfWorkAsync;
        }

        public override async Task ExecuteAsync(IPipelineMessage input)
        {
            var repo = unitOfWorkAsync.GetRepository<Customer>();

            if (await repo.ListAnyAsync())
            {
                input.Messages.AddMessage("ValidateCustomerRepositoryStep", Messages.RECORD_NOT_SEED_DATA, Mvp24Hours.Core.Enums.MessageType.Error);
            }
        }
    }
}
