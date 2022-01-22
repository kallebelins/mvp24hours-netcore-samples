using CustomerAPI.Core.ValueObjects.Customers;
using Mvp24Hours.Infrastructure.RabbitMQ;
using System.Diagnostics;
using System.Threading.Tasks;

namespace CustomerAPI.Application.Brokers.Consumers
{
    public class DeleteCustomerConsumer : MvpRabbitMQConsumer<DeleteCustomerRequest>
    {
        public override async Task Received(DeleteCustomerRequest message)
        {
            if (message == null)
            {
                Trace.WriteLine($"Received customer delete null.");
                return;
            }
            Trace.WriteLine($"Received customer delete {message.Id}.");
            await FacadeService.CustomerService.Delete(message.Id);
        }
    }
}
