using CustomerAPI.Application.Logic;
using CustomerAPI.Core.Contract.Logic;
using CustomerAPI.Core.Entities;
using CustomerAPI.Core.Validations.Customers;
using CustomerAPI.Infrastructure.Data;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mvp24Hours.Infrastructure.Extensions;

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
            // customer
            services.AddScoped<ICustomerService, CustomerService>();

            services.AddSingleton<IValidator<Customer>, CustomerValidator>();
            services.AddSingleton<IValidator<Contact>, ContactValidator>();

            return services;
        }
    }
}
