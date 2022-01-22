using AutoMapper;
using CustomerAPI.Core.Entities;
using Mvp24Hours.Core.Contract.Mappings;

namespace CustomerAPI.Core.ValueObjects.Customers
{
    public class UpdateCustomerRequest : IMapFrom<Customer>
    {
        public string Name { get; set; }
        public string Note { get; set; }

        public virtual void Mapping(Profile profile)
        {
            profile.CreateMap<UpdateCustomerRequest, Customer>();
        }
    }
}
