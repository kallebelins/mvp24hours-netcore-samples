using CustomerAPI.Application.Logic;
using CustomerAPI.Core.Contract.Logic;
using CustomerAPI.Core.Contract.Pipe.Builders;
using CustomerAPI.Typicode.Application.Pipe.Builders;
using Microsoft.Extensions.DependencyInjection;

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
        public static IServiceCollection AddMyServices(this IServiceCollection services)
        {
            services.AddScoped<ICustomerService, CustomerService>();

            // pipeline - builders
            services.AddScoped<IGetByCustomerBuilder, GetByCustomerBuilder>();
            services.AddScoped<IGetByIdCustomerBuilder, GetByIdCustomerBuilder>();

            return services;
        }
    }
}
