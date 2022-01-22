using AutoMapper;
using CustomerAPI.Core.Entities;
using CustomerAPI.Core.Enums;
using Mvp24Hours.Core.Contract.Mappings;
using System;
using System.Collections.Generic;

namespace CustomerAPI.Core.ValueObjects.Contacts
{
    public class GetByContactResponse : IMapFrom<Contact>
    {
        public DateTime Created { get; set; }
        public ContactType Type { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }

        public virtual void Mapping(Profile profile)
        {
            profile.CreateMap<Contact, GetByContactResponse>()
                .ForMember(x => x.Created, opt => opt.MapFrom(y => y.Created));
            profile.CreateMap<List<Contact>, List<GetByContactResponse>>();
        }
    }
}
