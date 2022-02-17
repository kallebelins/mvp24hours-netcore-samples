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
    public class CreateCustomerConsumer : MvpRabbitMQConsumerAsync<CustomerCreate>
    {
        public CreateCustomerConsumer(IOptions<RabbitMQOptions> options, ILoggingService logging)
            : base(options, logging) { }

        public override async Task ReceivedAsync(object message)
        {
            try
            {
                var dto = message as CustomerCreate;

                if (dto == null)
                {
                    Trace.WriteLine($"Received customer create null.");
                    return;
                }
                Trace.WriteLine($"Received customer create {dto.Name}");
                var result = await FacadeService.CustomerService.Create(dto);
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
