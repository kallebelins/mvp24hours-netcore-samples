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
                options.UseMySQL(configuration.GetConnectionString("CustomerDbContext")));
            services.AddMvp24HoursDbServiceAsync<CustomerDBContext>();
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
            services.AddScoped<IMvpRabbitMQProducer<CreateCustomerRequest>, CreateCustomerProducer>();
            services.AddScoped<IMvpRabbitMQProducer<UpdateCustomerRequest>, UpdateCustomerProducer>();
            services.AddScoped<IMvpRabbitMQProducer<DeleteCustomerRequest>, DeleteCustomerProducer>();
            return services;
        }

        /// <summary>
        /// 
        /// </summary>
        public static IServiceCollection AddMyRabbitConsumer(this IServiceCollection services)
        {
            services.AddScoped<IMvpRabbitMQConsumer<CreateCustomerRequest>, CreateCustomerConsumer>();
            services.AddScoped<IMvpRabbitMQConsumer<UpdateCustomerRequest>, UpdateCustomerConsumer>();
            services.AddScoped<IMvpRabbitMQConsumer<DeleteCustomerRequest>, DeleteCustomerConsumer>();
            return services;
        }

        /// <summary>
        /// 
        /// </summary>
        public static IServiceCollection AddMyHostedService(this IServiceCollection services)
        {
            services.AddMvp24HoursHostedService((e) =>
            {
                var createConsumer = ServiceProviderHelper.GetService<IMvpRabbitMQConsumer<CreateCustomerRequest>>();
                createConsumer?.Consume();

                var updateConsumer = ServiceProviderHelper.GetService<IMvpRabbitMQConsumer<UpdateCustomerRequest>>();
                updateConsumer?.Consume();

                var deteleConsumer = ServiceProviderHelper.GetService<IMvpRabbitMQConsumer<DeleteCustomerRequest>>();
                deteleConsumer?.Consume();
            });
            return services;
        }


    }
}
