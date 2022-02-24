using CustomerAPI.Core.ValueObjects.Customers;
using Mvp24Hours.Core.Enums.Infrastructure;
using Mvp24Hours.Helpers;
using Mvp24Hours.Infrastructure.RabbitMQ.Core.Contract;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerAPI.Application.Brokers.Consumers
{
    public class CreateCustomerConsumer : IMvpRabbitMQConsumerAsync
    {
        public string RoutingKey => typeof(CustomerCreate).Name;

        public string QueueName => typeof(CustomerCreate).Name;

        public async Task ReceivedAsync(object message, string token)
        {
            if (message is not CustomerCreate dto)
            {
                TelemetryHelper.Execute(TelemetryLevel.Verbose, "create-customer-consumer-received-null", "Received customer create null.");
                return;
            }
            TelemetryHelper.Execute(TelemetryLevel.Verbose, "create-customer-consumer-received", $"Received customer create {dto.Name}");
            var result = await FacadeService.CustomerService.Create(dto);
            if (result.HasErrors)
            {
                throw new System.InvalidOperationException(string.Join(" | ", result.Messages.Select(x => x.Message).ToArray()));
            }
        }

        public async Task FailureAsync(Exception exception, string token)
        {
            // perform handling for integration failures in RabbitMQ
            // write to temp table, send email, create specialized log, etc.
            await Task.CompletedTask;
        }

        public async Task RejectedAsync(object message, string token)
        {
            // we tried to consume the resource for 3 times, in this case as we did not treat it, we will disregard
            await Task.CompletedTask;
        }
    }
}
