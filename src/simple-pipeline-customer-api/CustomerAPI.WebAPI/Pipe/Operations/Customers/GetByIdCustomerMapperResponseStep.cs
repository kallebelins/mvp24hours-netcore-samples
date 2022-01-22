using CustomerAPI.Core.Resources;
using CustomerAPI.Core.ValueObjects.Customers;
using Mvp24Hours.Core.Contract.Infrastructure.Pipe;
using Mvp24Hours.Extensions;
using Mvp24Hours.Infrastructure.Pipe.Operations.Custom;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CustomerAPI.WebAPI.Pipe.Operations.Customers
{
    /// <summary>
    /// 
    /// </summary>
    public class GetByIdCustomerMapperResponseStep : OperationMapperAsync<GetByIdCustomerResponse>
    {
        /// <summary>
        /// 
        /// </summary>
        public override async Task<GetByIdCustomerResponse> MapperAsync(IPipelineMessage input)
        {
            if (!input.HasContent("customers"))
            {
                NotificationContext.Add("GetByCustomerMapperResponseStep", Messages.RECORD_NOT_FOUND, Mvp24Hours.Core.Enums.MessageType.Error);
            }

            if (!input.HasContent("id"))
            {
                NotificationContext.Add("GetByCustomerMapperResponseStep", Messages.PARAMETER_ID_REQUIRED, Mvp24Hours.Core.Enums.MessageType.Error);
            }

            if (NotificationContext.HasErrorNotifications)
            {
                input.SetLock();
                return await Task.FromResult<GetByIdCustomerResponse>(default);
            }

            var id = input.GetContent<int>("id");
            var customers = input.GetContent<dynamic>("customers");

            GetByIdCustomerResponse result = null;

            foreach (var customer in customers)
            {
                if (customer.id == id)
                {
                    result = new GetByIdCustomerResponse
                    {
                        Id = customer.id,
                        Name = customer.name,
                        Contacts = new List<GetByIdContactResponse>()
                    };

                    if (customer.email != null)
                    {
                        result.Contacts.Add(new GetByIdContactResponse
                        {
                            Description = customer.email,
                            Type = Core.Enums.ContactType.Email
                        });
                    }

                    if (customer.phone != null)
                    {
                        result.Contacts.Add(new GetByIdContactResponse
                        {
                            Description = customer.phone,
                            Type = Core.Enums.ContactType.CellPhone
                        });
                    }

                    if (customer.website != null)
                    {
                        result.Contacts.Add(new GetByIdContactResponse
                        {
                            Description = customer.website,
                            Type = Core.Enums.ContactType.Other
                        });
                    }
                }
            }

            return await result.TaskResult();
        }
    }
}
