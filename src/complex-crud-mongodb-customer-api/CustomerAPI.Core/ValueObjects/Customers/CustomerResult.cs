using AutoMapper;
using CustomerAPI.Core.Entities;
using Mvp24Hours.Core.Contract.Mappings;
using System;
using System.Collections.Generic;

namespace CustomerAPI.Core.ValueObjects.Customers
{
    public class CustomerResult : IMapFrom<Customer>
    {
        public string Id { get; set; }
        public DateTime Created { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }

        public virtual void Mapping(Profile profile)
        {
            profile.CreateMap<Customer, CustomerResult>()
                .ForMember(x => x.Created, opt => opt.MapFrom(y => y.Created));
            profile.CreateMap<List<Customer>, List<CustomerResult>>();
        }
    }
}
