using CustomerAPI.Infrastructure.Data;
using CustomerAPI.WebAPI.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Mvp24Hours.Extensions;
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
            services.AddMvp24HoursWebEssential(Configuration);
            services.AddMvp24HoursMapService(assemblyMap: Assembly.GetExecutingAssembly());
            services.AddMvp24HoursWebJson();
            services.AddMvp24HoursWebSwagger("Customer EF API", xmlCommentsFileName: "CustomerAPI.WebAPI.xml", enableExample: true);
            services.AddMvp24HoursWebGzip();
            #endregion

            services.AddMyServices(Configuration);
            services.AddMyDbContext(Configuration);

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

            // automatic migration
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
                app.UseMvp24HoursSwagger("Customer EF API");
            }

            app.UseMvp24Hours();
        }
    }
}