using CustomerAPI.Core.Entities;
using CustomerAPI.Core.Validations.Customers;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mvp24Hours.Core.Contract.Data;
using Mvp24Hours.Extensions;
using Mvp24Hours.Infrastructure.Caching;

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
        public static IServiceCollection AddMyCaching(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMvp24HoursRedisCache("RedisDbContext", instanceName: "customerapi", configuration: configuration);
            services.AddScoped<IRepositoryCacheAsync<CustomerDto>, RepositoryCacheAsync<CustomerDto>>();
            return services;
        }

        /// <summary>
        /// 
        /// </summary>
        public static IServiceCollection AddMyValidators(this IServiceCollection services)
        {
            services.AddSingleton<IValidator<CustomerDto>, CustomerValidator>();
            return services;
        }
    }
}
