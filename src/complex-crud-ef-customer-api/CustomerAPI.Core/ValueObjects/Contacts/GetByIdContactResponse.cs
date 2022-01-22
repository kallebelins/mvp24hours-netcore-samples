using AutoMapper;
using CustomerAPI.Core.Entities;
using CustomerAPI.Core.Enums;
using Mvp24Hours.Core.Contract.Mappings;
using System;

namespace CustomerAPI.Core.ValueObjects.Contacts
{
    public class GetByIdContactResponse : GetByContactResponse, IMapFrom<Contact>
    {
        public int Id { get; set; }

        public override void Mapping(Profile profile)
        {
            profile.CreateMap<Contact, GetByIdContactResponse>()
                .ForMember(x => x.Created, opt => opt.MapFrom(y => y.Created));
        }
    }
}
