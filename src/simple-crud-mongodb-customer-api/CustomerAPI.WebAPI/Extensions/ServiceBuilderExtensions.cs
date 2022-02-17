using CustomerAPI.Core.Entities;
using CustomerAPI.Core.Validations.Customers;
using CustomerAPI.Infrastructure.Data;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mvp24Hours.Extensions;

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
            services.AddMvp24HoursDbContext<CustomerDBContext>(options =>
            {
                options.DatabaseName = "simplecustomers";
                options.ConnectionString = configuration.GetConnectionString("MongoDbContext");
            });
            services.AddMvp24HoursRepositoryAsync();
            return services;
        }

        /// <summary>
        /// 
        /// </summary>
        public static IServiceCollection AddMyHealthChecks(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHealthChecks()
                .AddMongoDb(
                    configuration.GetConnectionString("MongoDbContext"),
                    name: "MongoDb",
                    failureStatus: Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Degraded);
            return services;
        }

        /// <summary>
        /// 
        /// </summary>
        public static IServiceCollection AddMyServices(this IServiceCollection services)
        {
            services.AddSingleton<IValidator<Customer>, CustomerValidator>();
            services.AddSingleton<IValidator<Contact>, ContactValidator>();
            return services;
        }
    }
}
