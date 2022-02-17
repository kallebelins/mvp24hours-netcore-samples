using CustomerAPI.Core.ValueObjects.Customers;
using Microsoft.Extensions.Options;
using Mvp24Hours.Core.Contract.Infrastructure.Logging;
using Mvp24Hours.Infrastructure.RabbitMQ;
using Mvp24Hours.Infrastructure.RabbitMQ.Configuration;

namespace CustomerAPI.Application.Brokers.Producers
{
    public class DeleteCustomerProducer : MvpRabbitMQProducer<CustomerDelete>
    {
        public DeleteCustomerProducer(IOptions<RabbitMQOptions> options, ILoggingService logging)
            : base(options, logging) { }
    }
}
