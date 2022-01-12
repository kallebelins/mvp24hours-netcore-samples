using AutoMapper;
using CustomerAPI.Core.Entities;
using CustomerAPI.Core.Enums;
using Mvp24Hours.Core.Contract.Mappings;
using System;

namespace CustomerAPI.Core.ValueObjects.Contacts
{
    public class CreateContactRequest : IMapFrom<Contact>
    {
        public ContactType Type { get; set; }
        public string Description { get; set; }

        public virtual void Mapping(Profile profile)
        {
            profile.CreateMap<CreateContactRequest, Contact>()
                .ForMember(x => x.Created, opt => opt.MapFrom(y => DateTime.Now));
        }
    }
}
