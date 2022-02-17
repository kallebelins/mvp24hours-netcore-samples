using CustomerAPI.Application.Logic;
using CustomerAPI.Core.Contract.Logic;
using CustomerAPI.Core.Entities;
using CustomerAPI.Core.Validations.Customers;
using CustomerAPI.Infrastructure.Data;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
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
            services.AddDbContext<CustomerDBContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("CustomerDbContext"))
            );
            services.AddMvp24HoursDbContext<CustomerDBContext>();
            services.AddMvp24HoursRepositoryAsync(options: options =>
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
                    failureStatus: Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Degraded);
            return services;
        }

        /// <summary>
        /// 
        /// </summary>
        public static IServiceCollection AddMyServices(this IServiceCollection services)
        {
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IContactService, ContactService>();

            services.AddSingleton<IValidator<Customer>, CustomerValidator>();
            services.AddSingleton<IValidator<Contact>, ContactValidator>();
            return services;
        }
    }
}
