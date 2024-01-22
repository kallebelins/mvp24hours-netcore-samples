using CustomerAPI.Application.Brokers.Consumers;
using CustomerAPI.Application.Logic;
using CustomerAPI.Core.Contract.Logic;
using CustomerAPI.Core.Entities;
using CustomerAPI.Core.Validations.Customers;
using CustomerAPI.Infrastructure.Data;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mvp24Hours.Core.Enums.Infrastructure;
using Mvp24Hours.Extensions;
using Mvp24Hours.Infrastructure.RabbitMQ;
using Mvp24Hours.Infrastructure.RabbitMQ.Configuration;
using NLog;
using System;
using System.Linq;

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
            services.AddDbContext<EFDBContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("EFDBContext"))
            );
            services.AddMvp24HoursDbContext<EFDBContext>();
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
                    configuration.GetConnectionString("EFDBContext"),
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
        public static IServiceCollection AddMyTelemetry(this IServiceCollection services)
        {
            Logger logger = LogManager.GetCurrentClassLogger();
#if DEBUG
            services.AddMvp24HoursTelemetry(TelemetryLevels.Information | TelemetryLevels.Verbose,
                (name, state) =>
                {
                    if (name.EndsWith("-object"))
                    {
                        logger.Info($"{name}|body:{state.ToSerialize()}");
                    }
                    else
                    {
                        logger.Info($"{name}|{string.Join("|", state)}");
                    }
                }
            );
#endif
            services.AddMvp24HoursTelemetry(TelemetryLevels.Error,
                (name, state) =>
                {
                    if (name.EndsWith("-failure"))
                    {
                        logger.Error(state.ElementAtOrDefault(0) as Exception);
                    }
                    else
                    {
                        logger.Error($"{name}|{string.Join("|", state)}");
                    }
                }
            );
            services.AddMvp24HoursTelemetryIgnore("rabbitmq-consumer-basic");
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
        public static IServiceCollection AddMyRabbitMQ(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMvp24HoursRabbitMQ(
                typeof(CreateCustomerConsumer).Assembly,
                connectionOptions =>
                {
                    connectionOptions.ConnectionString = configuration.GetConnectionString("RabbitMQContext");
                    connectionOptions.DispatchConsumersAsync = true;
                    connectionOptions.RetryCount = 3;
                },
                clientOptions =>
                {
                    clientOptions.Exchange = "customer.direct";
                    clientOptions.MaxRedeliveredCount = 1;
                    clientOptions.QueueArguments = new System.Collections.Generic.Dictionary<string, object>
                    {
                        { "x-queue-mode", "lazy" },
                        { "x-dead-letter-exchange", "dead-letter-customer.direct" }
                    };

                    // dead letter exchanges enabled
                    clientOptions.DeadLetter = new RabbitMQOptions()
                    {
                        Exchange = "dead-letter-customer.direct",
                        QueueArguments = new System.Collections.Generic.Dictionary<string, object>
                        {
                            { "x-queue-mode", "lazy" }
                        }
                    };
                }
            );
            return services;
        }

        /// <summary>
        /// 
        /// </summary>
        public static IServiceCollection AddMyHostedService(this IServiceCollection services)
        {
            services.AddScoped<CreateCustomerConsumer>();
            services.AddScoped<DeleteCustomerConsumer>();
            services.AddScoped<UpdateCustomerConsumer>();

            var provider = services.BuildServiceProvider();

            services.AddMvp24HoursHostedService(options =>
            {
                options.Callback = e =>
                {
                    var client = provider.GetService<MvpRabbitMQClient>();
                    client?.Consume();
                };
            });
            return services;
        }


    }
}
