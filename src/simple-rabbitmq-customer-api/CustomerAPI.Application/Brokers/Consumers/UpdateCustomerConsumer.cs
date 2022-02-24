using CustomerAPI.Core.ValueObjects.Customers;
using Mvp24Hours.Core.Enums.Infrastructure;
using Mvp24Hours.Helpers;
using Mvp24Hours.Infrastructure.RabbitMQ.Core.Contract;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerAPI.Application.Brokers.Consumers
{
    public class UpdateCustomerConsumer : IMvpRabbitMQConsumerAsync
    {
        public string RoutingKey => typeof(CustomerUpdate).Name;

        public string QueueName => typeof(CustomerUpdate).Name;

        public async Task ReceivedAsync(object message, string token)
        {
            if (message is not CustomerUpdate dto)
            {
                TelemetryHelper.Execute(TelemetryLevel.Verbose, "update-customer-consumer-received-null", "Received customer update null.");
                return;
            }
            TelemetryHelper.Execute(TelemetryLevel.Verbose, "update-customer-consumer-received", $"Received customer update {dto.Name}.");
            var result = await FacadeService.CustomerService.Update(dto.Id, dto);
            if (result.HasErrors)
            {
                throw new System.InvalidOperationException($"token:{token}|messages:{string.Join(" ; ", result.Messages.Select(x => x.Message).ToArray())}");
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
