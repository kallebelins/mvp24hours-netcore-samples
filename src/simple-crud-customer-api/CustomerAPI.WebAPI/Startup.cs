using CustomerAPI.Infrastructure.Data;
using CustomerAPI.WebAPI.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Mvp24Hours.Infrastructure.Extensions;
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
            services.AddMvp24HoursWeb(Configuration);
            services.AddMvp24HoursWebFilters();
            services.AddMvp24HoursWebJson();
            services.AddMvp24HoursWebSwagger("Customer API", xmlCommentsFileName: "CustomerAPI.WebAPI.xml");
            services.AddMvp24HoursWebGzip();
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
                app.UseMvp24HoursSwagger("Customer API");
            }

            app.UseMvp24Hours();
        }
    }
}
