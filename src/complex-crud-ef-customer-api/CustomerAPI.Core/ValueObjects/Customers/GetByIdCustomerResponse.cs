using AutoMapper;
using CustomerAPI.Core.Entities;
using CustomerAPI.Core.ValueObjects.Contacts;
using Mvp24Hours.Core.Contract.Mappings;
using System;
using System.Collections.Generic;

namespace CustomerAPI.Core.ValueObjects.Customers
{
    public class GetByIdCustomerResponse : GetByCustomerResponse, IMapFrom<Customer>
    {
        public string Note { get; set; }

        public ICollection<GetByIdContactResponse> Contacts { get; set; }

        public override void Mapping(Profile profile)
        {
            profile.CreateMap<Customer, GetByIdCustomerResponse>()
                .ForMember(x => x.Created, opt => opt.MapFrom(y => y.Created));
        }
    }
}
