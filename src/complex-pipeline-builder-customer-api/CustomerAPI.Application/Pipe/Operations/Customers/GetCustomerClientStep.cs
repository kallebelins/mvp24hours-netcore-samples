using Mvp24Hours.Core.Contract.Infrastructure.Pipe;
using Mvp24Hours.Extensions;
using Mvp24Hours.Helpers;
using Mvp24Hours.Infrastructure.Pipe.Operations;
using System.Threading.Tasks;

namespace CustomerAPI.Application.Pipe.Operations.Customers
{
    public class GetCustomerClientStep : OperationBaseAsync
    {
        public override async Task<IPipelineMessage> ExecuteAsync(IPipelineMessage input)
        {
            string url = ConfigurationHelper.GetSettings("Settings:TypicodeCustomerUrl");

            if (!url.HasValue())
            {
                NotificationContext.Add("GetCustomerClientStep", "Typicode service url not found in configuration (appsettings).", Mvp24Hours.Core.Enums.MessageType.Error);
                input.SetLock();
                return input;
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

            return input;
        }
    }
}
