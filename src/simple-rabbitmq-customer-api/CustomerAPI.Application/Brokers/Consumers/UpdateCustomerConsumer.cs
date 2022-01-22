using CustomerAPI.Core.ValueObjects.Customers;
using Mvp24Hours.Infrastructure.RabbitMQ;
using System.Diagnostics;
using System.Threading.Tasks;

namespace CustomerAPI.Application.Brokers.Consumers
{
    public class UpdateCustomerConsumer : MvpRabbitMQConsumer<UpdateCustomerRequest>
    {
        public override async Task Received(UpdateCustomerRequest message)
        {
            if (message == null)
            {
                Trace.WriteLine($"Received customer update null.");
                return;
            }
            Trace.WriteLine($"Received customer update {message.Name}");
            await FacadeService.CustomerService.Update(message.Id, message);
        }
    }
}
