using CustomerAPI.Application.Brokers.Consumers;
using CustomerAPI.Application.Brokers.Producers;
using CustomerAPI.Application.Logic;
using CustomerAPI.Core.Contract.Logic;
using CustomerAPI.Core.Entities;
using CustomerAPI.Core.Validations.Customers;
using CustomerAPI.Core.ValueObjects.Customers;
using CustomerAPI.Infrastructure.Data;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mvp24Hours.Extensions;
using Mvp24Hours.Helpers;
using Mvp24Hours.Infrastructure.RabbitMQ;
using Mvp24Hours.Infrastructure.RabbitMQ.Core.Contract;

namespace CustomerAPI.WebAPI.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class ServiceBuilderExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        public static IServiceCollection AddMyDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<CustomerDBContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("CustomerDbContext"))
            );
            services.AddMvp24HoursDbContext<CustomerDBContext>();
            services.AddMvp24HoursRepositoryAsync(options =>
            {
                options.MaxQtyByQueryPage = 100;
                options.TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
            });
            return services;
        }

        /// <summary>
        /// 
        /// </summary>
        public static IServiceCollection AddMyHealthChecks(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHealthChecks()
                .AddSqlServer(
                    configuration.GetConnectionString("CustomerDbContext"),
                    healthQuery: "SELECT 1;",
                    name: "SqlServer",
                    failureStatus: Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Degraded)
                .AddRabbitMQ(
                    configuration.GetConnectionString("RabbitMQContext"),
                    name: "RabbitMQ",
                    failureStatus: Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Degraded);
            return services;
        }

        /// <summary>
        /// 
        /// </summary>
        public static IServiceCollection AddMyServices(this IServiceCollection services)
        {
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddSingleton<IValidator<Customer>, CustomerValidator>();
            return services;
        }

        /// <summary>
        /// 
        /// </summary>
        public static IServiceCollection AddMyRabbitProducer(this IServiceCollection services)
        {
            services.AddScoped<MvpRabbitMQProducer<CustomerCreate>, CreateCustomerProducer>();
            services.AddScoped<MvpRabbitMQProducer<CustomerUpdate>, UpdateCustomerProducer>();
            services.AddScoped<MvpRabbitMQProducer<CustomerDelete>, DeleteCustomerProducer>();
            return services;
        }

        /// <summary>
        /// 
        /// </summary>
        public static IServiceCollection AddMyRabbitConsumer(this IServiceCollection services)
        {
            services.AddScoped<MvpRabbitMQConsumerAsync<CustomerCreate>, CreateCustomerConsumer>();
            services.AddScoped<MvpRabbitMQConsumerAsync<CustomerUpdate>, UpdateCustomerConsumer>();
            services.AddScoped<MvpRabbitMQConsumerAsync<CustomerDelete>, DeleteCustomerConsumer>();
            return services;
        }

        /// <summary>
        /// 
        /// </summary>
        public static IServiceCollection AddMyHostedService(this IServiceCollection services)
        {
            services.AddMvp24HoursHostedService(options =>
            {
                options.Callback = e =>
                {
                    if (ServiceProviderHelper.IsReady())
                    {
                        var createConsumer = ServiceProviderHelper.GetService<MvpRabbitMQConsumerAsync<CustomerCreate>>();
                        createConsumer?.Consume();

                        var updateConsumer = ServiceProviderHelper.GetService<MvpRabbitMQConsumerAsync<CustomerUpdate>>();
                        updateConsumer?.Consume();

                        var deteleConsumer = ServiceProviderHelper.GetService<MvpRabbitMQConsumerAsync<CustomerDelete>>();
                        deteleConsumer?.Consume();
                    }
                };
            });
            return services;
        }


    }
}
