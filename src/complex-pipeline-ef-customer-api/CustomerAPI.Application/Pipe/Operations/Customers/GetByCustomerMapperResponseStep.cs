using CustomerAPI.Core.Resources;
using CustomerAPI.Core.ValueObjects.Contacts;
using CustomerAPI.Core.ValueObjects.Customers;
using Mvp24Hours.Core.Contract.Infrastructure.Pipe;
using Mvp24Hours.Extensions;
using Mvp24Hours.Infrastructure.Pipe.Operations.Custom;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CustomerAPI.Application.Pipe.Operations.Customers
{
    /// <summary>
    /// Performs mapping of data obtained through the service
    /// </summary>
    public class GetByCustomerMapperResponseStep : OperationMapperAsync<IList<CreateCustomerRequest>>
    {
        public override string ContentKey => "model-customers";

        public override async Task<IList<CreateCustomerRequest>> MapperAsync(IPipelineMessage input)
        {
            if (!input.HasContent("customers"))
            {
                NotificationContext.Add("GetByCustomerMapperResponseStep", Messages.RECORD_NOT_FOUND, Mvp24Hours.Core.Enums.MessageType.Error);
                input.SetLock();
                return await Task.FromResult<IList<CreateCustomerRequest>>(default);
            }

            var customers = input.GetContent<dynamic>("customers");

            IList<CreateCustomerRequest> result = new List<CreateCustomerRequest>();

            foreach (var customer in customers)
            {
                var model = new CreateCustomerRequest
                {
                    Name = customer.name,
                    Contacts = new List<CreateContactRequest>()
                };

                if (customer.email != null)
                {
                    model.Contacts.Add(new CreateContactRequest
                    {
                        Description = customer.email,
                        Type = Core.Enums.ContactType.Email
                    });
                }

                if (customer.phone != null)
                {
                    model.Contacts.Add(new CreateContactRequest
                    {
                        Description = customer.phone,
                        Type = Core.Enums.ContactType.CellPhone
                    });
                }

                if (customer.website != null)
                {
                    model.Contacts.Add(new CreateContactRequest
                    {
                        Description = customer.website,
                        Type = Core.Enums.ContactType.Other
                    });
                }

                result.Add(model);
            }

            return await result.TaskResult();
        }
    }
}
