using CustomerAPI.Core.ValueObjects.Customers;
using Mvp24Hours.Core.Enums.Infrastructure;
using Mvp24Hours.Helpers;
using Mvp24Hours.Infrastructure.RabbitMQ.Core.Contract;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerAPI.Application.Brokers.Consumers
{
    public class UpdateCustomerConsumer : IMvpRabbitMQConsumerAsync
    {
        private readonly FacadeService facade;

        public UpdateCustomerConsumer(FacadeService facade)
        {
            this.facade = facade;
        }

        public string RoutingKey => typeof(CustomerUpdate).Name;

        public string QueueName => typeof(CustomerUpdate).Name;

        public async Task ReceivedAsync(object message, string token)
        {
            if (message is not CustomerUpdate dto)
            {
                TelemetryHelper.Execute(TelemetryLevels.Verbose, "update-customer-consumer-received-null", "Received customer update null.");
                return;
            }
            TelemetryHelper.Execute(TelemetryLevels.Verbose, "update-customer-consumer-received", $"Received customer update {dto.Name}.");
            var result = await facade.CustomerService.Update(dto.Id, dto);
            if (result.HasErrors)
            {
                throw new System.InvalidOperationException($"token:{token}|messages:{string.Join(" ; ", result.Messages.Select(x => x.Message).ToArray())}");
            }
        }
    }
}
