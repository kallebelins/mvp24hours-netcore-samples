using AutoMapper;
using CustomerAPI.Core.Entities;
using CustomerAPI.Core.ValueObjects.Contacts;
using Mvp24Hours.Core.Contract.Mappings;
using System.Collections.Generic;

namespace CustomerAPI.Core.ValueObjects.Customers
{
    public class CustomerIdResult : CustomerResult, IMapFrom<Customer>
    {
        public string Note { get; set; }

        public ICollection<ContactIdResult> Contacts { get; set; }

        public override void Mapping(Profile profile)
        {
            profile.CreateMap<Customer, CustomerIdResult>()
                .ForMember(x => x.Created, opt => opt.MapFrom(y => y.Created));
        }
    }
}
