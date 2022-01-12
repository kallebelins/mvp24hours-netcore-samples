using AutoMapper;
using CustomerAPI.Core.Entities;
using Mvp24Hours.Core.Contract.Mappings;
using System;

namespace CustomerAPI.Core.ValueObjects.Customers
{
    public class GetByIdCustomerResponse : GetByCustomerResponse, IMapFrom<Customer>
    {
        public DateTime Created { get; set; }
        public string Note { get; set; }
        public bool Active { get; set; }

        public override void Mapping(Profile profile)
        {
            profile.CreateMap<Customer, GetByIdCustomerResponse>()
                .ForMember(x => x.Created, opt => opt.MapFrom(y => y.Created));
        }
    }
}
