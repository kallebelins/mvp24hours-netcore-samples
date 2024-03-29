using CustomerAPI.Core.Entities;
using CustomerAPI.Infrastructure.Data;
using CustomerAPI.WebAPI.Extensions;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Mvp24Hours.Extensions;
using Mvp24Hours.WebAPI.Extensions;

namespace CustomerAPI.WebAPI
{
    /// <summary>
    /// 
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// 
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        public void ConfigureServices(IServiceCollection services)
        {
            #region [ Mvp24Hours ]
            services.AddMvp24HoursWebEssential();
            services.AddMvp24HoursMapService(assemblyMap: typeof(Customer).Assembly);
            services.AddMvp24HoursWebJson();
            services.AddMvp24HoursWebSwagger("Customer EF API with Broker", xmlCommentsFileName: "CustomerAPI.WebAPI.xml", enableExample: true);
            services.AddMvp24HoursWebGzip();
            #endregion

            services.AddMyTelemetry();
            services.AddMyServices();
            services.AddMyDbContext(Configuration);
            services.AddMyRabbitMQ(Configuration);
            services.AddMyHealthChecks(Configuration);

            services.AddControllers();
            services.AddMvc();
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, EFDBContext db)
        {
            // check environment
            app.UseMvp24HoursExceptionHandling();

            // automatic migration
            db.Database.EnsureCreated();

            app.UseStaticFiles();
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/hc", new HealthCheckOptions
                {
                    Predicate = _ => true,
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });
            });

            if (!env.IsProduction())
            {
                app.UseMvp24HoursSwagger("Customer EF API with Broker");
            }
        }
    }
}
