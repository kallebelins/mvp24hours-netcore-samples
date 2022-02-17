using AutoMapper;
using CustomerAPI.Core.Entities;
using Mvp24Hours.Core.Contract.Mappings;
using System;

namespace CustomerAPI.Core.ValueObjects.Customers
{
    public class CustomerCreate : IMapFrom<Customer>
    {
        public string Name { get; set; }
        public string Note { get; set; }

        public virtual void Mapping(Profile profile)
        {
            profile.CreateMap<CustomerCreate, Customer>()
                .ForMember(x => x.Created, opt => opt.MapFrom(y => DateTime.Now))
                .ForMember(x => x.Active, opt => opt.MapFrom(y => true));
        }
    }
}
