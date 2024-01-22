using AutoMapper;
using CustomerAPI.Application;
using CustomerAPI.Application.Logic;
using CustomerAPI.Application.Pipe.Operations.Customers;
using CustomerAPI.Core.Contract.Logic;
using CustomerAPI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mvp24Hours.Core.Contract.Data;
using Mvp24Hours.Core.Enums.Infrastructure;
using Mvp24Hours.Extensions;
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
        public static void AddMyServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<FacadeService>();

            services.AddScoped<ICustomerService, CustomerService>();

            services.AddScoped<GetCustomerClientStep>(sp =>
            {
                return new GetCustomerClientStep(configuration);
            });

            services.AddScoped<CreateCustomerRepositoryStep>(sp =>
            {
                return new CreateCustomerRepositoryStep(sp.GetRequiredService<IUnitOfWorkAsync>(), sp.GetService<IMapper>());
            });

            services.AddScoped<ValidateCustomerRepositoryStep>(sp =>
            {
                return new ValidateCustomerRepositoryStep(sp.GetRequiredService<IUnitOfWorkAsync>());
            });
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
            return services;
        }
    }
}
