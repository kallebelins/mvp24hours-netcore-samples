using CustomerAPI.Core.ValueObjects.Customers;
using Mvp24Hours.Core.Enums.Infrastructure;
using Mvp24Hours.Helpers;
using Mvp24Hours.Infrastructure.RabbitMQ.Core.Contract;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerAPI.Application.Brokers.Consumers
{
    public class DeleteCustomerConsumer : IMvpRabbitMQConsumerAsync
    {
        private readonly FacadeService facade;

        public DeleteCustomerConsumer(FacadeService facade)
        {
            this.facade = facade;
        }

        public string RoutingKey => typeof(CustomerDelete).Name;

        public string QueueName => typeof(CustomerDelete).Name;

        public async Task ReceivedAsync(object message, string token)
        {
            if (message is not CustomerDelete dto)
            {
                TelemetryHelper.Execute(TelemetryLevels.Verbose, "delete-customer-consumer-received-null", "Received customer delete null.");
                return;
            }
            TelemetryHelper.Execute(TelemetryLevels.Verbose, "delete-customer-consumer-received", $"Received customer delete {dto.Id}.");
            var result = await facade.CustomerService.Delete(dto.Id);
            if (result.HasErrors)
            {
                throw new System.InvalidOperationException($"token:{token}|messages:{string.Join(" ; ", result.Messages.Select(x => x.Message).ToArray())}");
            }
        }
    }
}
