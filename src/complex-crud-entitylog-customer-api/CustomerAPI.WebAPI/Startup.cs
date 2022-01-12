using CustomerAPI.Infrastructure.Data;
using CustomerAPI.WebAPI.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Mvp24Hours.Infrastructure.Extensions;
using Mvp24Hours.WebAPI.Extensions;
using System.Reflection;

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
            services.AddMvp24Hours(Configuration);
            services.AddMvp24HoursService();
            services.AddMvp24HoursLogging();
            services.AddMvp24HoursMapService(Assembly.GetExecutingAssembly());
            services.AddMvp24HoursNotification();
            // services.AddMvp24HoursJson();
            services.AddMvp24HoursSwagger("Customer API", xmlCommentsFileName: "CustomerAPI.WebAPI.xml");
            services.AddMvp24HoursZipService();
            #endregion

            services.AddMyDbContext(Configuration);
            services.AddMyServices();

            services.AddControllers();
            services.AddMvc();
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, CustomerDBContext db)
        {
            // check environment
            app.UseMvp24HoursExceptionHandling();

            db.Database.EnsureCreated();

            app.UseStaticFiles();
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            if (!env.IsProduction())
            {
                app.UseMvp24HoursSwagger("Customer API");
            }

            app.UseMvp24Hours();
        }
    }
}
