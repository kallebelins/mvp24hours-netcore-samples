using CustomerAPI.Core.ValueObjects.Customers;
using Mvp24Hours.Infrastructure.RabbitMQ;
using System.Diagnostics;
using System.Threading.Tasks;

namespace CustomerAPI.Application.Brokers.Consumers
{
    public class CreateCustomerConsumer : MvpRabbitMQConsumer<CreateCustomerRequest>
    {
        public override async Task Received(CreateCustomerRequest message)
        {
            if (message == null)
            {
                Trace.WriteLine($"Received customer create null.");
                return;
            }
            Trace.WriteLine($"Received customer create {message.Name}");
            await FacadeService.CustomerService.Create(message);
        }
    }
}
