using AutoMapper;
using CustomerAPI.Core.Entities;
using CustomerAPI.Core.ValueObjects.Contacts;
using Mvp24Hours.Core.Contract.Mappings;
using System;
using System.Collections.Generic;

namespace CustomerAPI.Core.ValueObjects.Customers
{
    public class CreateCustomerRequest : IMapFrom<Customer>
    {
        public string Name { get; set; }
        public string Note { get; set; }

        public IList<CreateContactRequest> Contacts { get; set; }

        public virtual void Mapping(Profile profile)
        {
            profile.CreateMap<CreateCustomerRequest, Customer>()
                .ForMember(x => x.Created, opt => opt.MapFrom(y => DateTime.Now))
                .ForMember(x => x.Active, opt => opt.MapFrom(y => true));
        }
    }
}
