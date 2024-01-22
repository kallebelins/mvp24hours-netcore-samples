using CustomerAPI.Core.ValueObjects.Customers;
using Mvp24Hours.Core.Enums.Infrastructure;
using Mvp24Hours.Helpers;
using Mvp24Hours.Infrastructure.RabbitMQ.Core.Contract;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerAPI.Application.Brokers.Consumers
{
    public class CreateCustomerConsumer : IMvpRabbitMQConsumerAsync
    {
        private readonly FacadeService facade;

        public CreateCustomerConsumer(FacadeService facade)
        {
            this.facade = facade;
        }

        public string RoutingKey => typeof(CustomerCreate).Name;

        public string QueueName => typeof(CustomerCreate).Name;

        public async Task ReceivedAsync(object message, string token)
        {
            if (message is not CustomerCreate dto)
            {
                TelemetryHelper.Execute(TelemetryLevels.Verbose, "create-customer-consumer-received-null", "Received customer create null.");
                return;
            }
            TelemetryHelper.Execute(TelemetryLevels.Verbose, "create-customer-consumer-received", $"Received customer create {dto.Name}");
            var result = await facade.CustomerService.Create(dto);
            if (result.HasErrors)
            {
                throw new System.InvalidOperationException(string.Join(" | ", result.Messages.Select(x => x.Message).ToArray()));
            }
        }
    }
}
