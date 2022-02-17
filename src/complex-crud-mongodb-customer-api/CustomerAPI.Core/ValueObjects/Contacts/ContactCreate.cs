using AutoMapper;
using CustomerAPI.Core.Entities;
using CustomerAPI.Core.Enums;
using Mvp24Hours.Core.Contract.Mappings;
using System;

namespace CustomerAPI.Core.ValueObjects.Contacts
{
    public class ContactCreate : IMapFrom<Contact>
    {
        public ContactType Type { get; set; }
        public string Description { get; set; }

        public virtual void Mapping(Profile profile)
        {
            profile.CreateMap<ContactCreate, Contact>()
                .ForMember(x => x.Id, opt => opt.MapFrom(y => Guid.NewGuid().ToString()))
                .ForMember(x => x.Created, opt => opt.MapFrom(y => DateTime.Now))
                .ForMember(x => x.Active, opt => opt.MapFrom(y => true));
        }
    }
}
