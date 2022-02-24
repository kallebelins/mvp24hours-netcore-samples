using CustomerAPI.Core.Resources;
using CustomerAPI.Core.ValueObjects.Customers;
using Mvp24Hours.Core.Contract.Infrastructure.Pipe;
using Mvp24Hours.Extensions;
using Mvp24Hours.Infrastructure.Pipe.Operations.Custom;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CustomerAPI.Application.Pipe.Operations.Customers
{
    public class GetByIdCustomerMapperResponseStep : OperationMapperAsync<CustomerIdResult>
    {
        public override async Task<CustomerIdResult> MapperAsync(IPipelineMessage input)
        {
            if (!input.HasContent("customers"))
            {
                input.Messages.AddMessage("GetByCustomerMapperResponseStep", Messages.RECORD_NOT_FOUND, Mvp24Hours.Core.Enums.MessageType.Error);
            }

            if (!input.HasContent("id"))
            {
                input.Messages.AddMessage("GetByCustomerMapperResponseStep", Messages.PARAMETER_ID_REQUIRED, Mvp24Hours.Core.Enums.MessageType.Error);
            }

            if (input.IsFaulty)
            {
                return await Task.FromResult<CustomerIdResult>(default);
            }

            var id = input.GetContent<int>("id");
            var customers = input.GetContent<dynamic>("customers");

            CustomerIdResult result = null;

            foreach (var customer in customers)
            {
                if (customer.id == id)
                {
                    result = new CustomerIdResult
                    {
                        Id = customer.id,
                        Name = customer.name,
                        Contacts = new List<ContactIdResult>()
                    };

                    if (customer.email != null)
                    {
                        result.Contacts.Add(new ContactIdResult
                        {
                            Description = customer.email,
                            Type = Core.Enums.ContactType.Email
                        });
                    }

                    if (customer.phone != null)
                    {
                        result.Contacts.Add(new ContactIdResult
                        {
                            Description = customer.phone,
                            Type = Core.Enums.ContactType.CellPhone
                        });
                    }

                    if (customer.website != null)
                    {
                        result.Contacts.Add(new ContactIdResult
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
