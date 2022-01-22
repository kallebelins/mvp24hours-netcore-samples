using CustomerAPI.Core.ValueObjects.Customers;
using Mvp24Hours.Infrastructure.RabbitMQ;

namespace CustomerAPI.Application.Brokers.Producers
{
    public class UpdateCustomerProducer : MvpRabbitMQProducer<UpdateCustomerRequest>
    {
    }
}
