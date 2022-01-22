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
            services.AddMvp24HoursMongoDbAsync<CustomerDBContext>(
                "DbCustomers", configuration.GetConnectionString("MongoDbContext"));
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
