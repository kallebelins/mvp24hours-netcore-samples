using AutoMapper;
using CustomerAPI.Core.Entities;
using CustomerAPI.Core.Enums;
using Mvp24Hours.Core.Contract.Mappings;
using Mvp24Hours.Extensions;
using System;

namespace CustomerAPI.Core.ValueObjects.Contacts
{
    public class ContactCreate : IMapFrom
    {
        public ContactType Type { get; set; }
        public string Description { get; set; }

        public virtual void Mapping(Profile profile)
        {
            profile.CreateMap<ContactCreate, Contact>()
                .MapProperty(x => DateTime.Now, x => x.Created)
                .MapProperty(x => true, x => x.Active);
        }
    }
}
