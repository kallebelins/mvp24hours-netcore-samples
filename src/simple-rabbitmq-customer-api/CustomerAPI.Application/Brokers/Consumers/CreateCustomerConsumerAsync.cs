using CustomerAPI.Core.ValueObjects.Customers;
using Mvp24Hours.Infrastructure.RabbitMQ;
using RabbitMQ.Client.Events;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace CustomerAPI.Application.Brokers.Consumers
{
    public class CreateCustomerConsumerAsync : MvpRabbitMQConsumerAsync<CreateCustomerRequest>
    {
        public override async Task ReceivedAsync(CreateCustomerRequest message)
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
