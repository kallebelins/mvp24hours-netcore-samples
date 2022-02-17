using CustomerAPI.Core.ValueObjects.Customers;
using Microsoft.Extensions.Options;
using Mvp24Hours.Core.Contract.Infrastructure.Logging;
using Mvp24Hours.Infrastructure.RabbitMQ;
using Mvp24Hours.Infrastructure.RabbitMQ.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerAPI.Application.Brokers.Consumers
{
    public class DeleteCustomerConsumer : MvpRabbitMQConsumerAsync<CustomerDelete>
    {
        public DeleteCustomerConsumer(IOptions<RabbitMQOptions> options, ILoggingService logging)
            : base(options, logging) { }
        
        public override async Task ReceivedAsync(object message)
        {
            try
            {
                var dto = message as CustomerDelete;

                if (dto == null)
                {
                    Trace.WriteLine($"Received customer delete null.");
                    return;
                }
                Trace.WriteLine($"Received customer delete {dto.Id}.");
                var result = await FacadeService.CustomerService.Delete(dto.Id);
                if (result.HasErrors)
                    throw new System.InvalidOperationException(string.Join(" | ", result.Messages.Select(x => x.Message).ToArray()));
            }
            catch (System.Exception ex)
            {
                Logging.Error(ex);
            }
        }
    }
}
