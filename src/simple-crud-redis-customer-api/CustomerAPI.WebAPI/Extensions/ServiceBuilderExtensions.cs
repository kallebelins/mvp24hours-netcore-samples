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
            services.AddMvp24HoursCaching(
                /* Remove item from cache after duration */
                AbsoluteExpiration: System.DateTimeOffset.Now.AddMinutes(30),
                /* Remove item from cache if unsued for the duration */
                SlidingExpiration: System.TimeSpan.FromMinutes(5)
            );
            services.AddMvp24HoursCachingRedis(configuration.GetConnectionString("RedisDbContext"), instanceName: "customerapi");
            services.AddScoped<IRepositoryCacheAsync<CustomerDto>, RepositoryCacheAsync<CustomerDto>>();
            return services;
        }

        /// <summary>
        /// 
        /// </summary>
        public static IServiceCollection AddMyHealthChecks(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHealthChecks()
                .AddRedis(
                    configuration.GetConnectionString("RedisDbContext"),
                    name: "Redis",
                    failureStatus: Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Degraded);
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
