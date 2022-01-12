using AutoMapper;
using CustomerAPI.Core.Entities;
using CustomerAPI.Core.Enums;
using Mvp24Hours.Core.Contract.Mappings;
using System;

namespace CustomerAPI.Core.ValueObjects.Contacts
{
    public class GetByIdContactResponse : GetByContactResponse, IMapFrom<Contact>
    {
        public DateTime Created { get; set; }
        public int ContactId { get; set; }
        public ContactType Type { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }

        public override void Mapping(Profile profile)
        {
            profile.CreateMap<Contact, GetByIdContactResponse>()
                .ForMember(x => x.Created, opt => opt.MapFrom(y => y.Created));
        }
    }
}
