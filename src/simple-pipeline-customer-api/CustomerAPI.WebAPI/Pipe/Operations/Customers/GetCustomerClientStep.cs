using Microsoft.Extensions.Configuration;
using Mvp24Hours.Core.Contract.Infrastructure.Pipe;
using Mvp24Hours.Extensions;
using Mvp24Hours.Helpers;
using Mvp24Hours.Infrastructure.Pipe.Operations;
using System.Threading.Tasks;

namespace CustomerAPI.WebAPI.Pipe.Operations.Customers
{
    /// <summary>
    /// 
    /// </summary>
    public class GetCustomerClientStep : OperationBaseAsync
    {
        private readonly IConfiguration configuration;

        /// <summary>
        /// 
        /// </summary>
        public GetCustomerClientStep(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        /// <summary>
        /// 
        /// </summary>
        public override async Task ExecuteAsync(IPipelineMessage input)
        {
            string url = configuration.GetSection("Settings:TypicodeCustomerUrl").Value;

            if (!url.HasValue())
            {
                input.Messages.AddMessage("GetCustomerClientStep", "Typicode service url not found in configuration (appsettings).", Mvp24Hours.Core.Enums.MessageType.Error);
                return;
            }

            string response = await WebRequestHelper.GetAsync(url);

            // json definition for dynamic type
            var def = new[] {
                new {
                    id = 0,
                    name = string.Empty,
                    username = string.Empty,
                    email = string.Empty,
                    phone = string.Empty,
                    website = string.Empty
                }
            };

            var result = response.ToDeserializeAnonymous(def);

            if (result != null)
            {
                input.AddContent("customers", result);
            }
        }
    }
}
