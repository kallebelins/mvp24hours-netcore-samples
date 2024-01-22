using AutoMapper;
using CustomerAPI.Core.Entities;
using Mvp24Hours.Core.Contract.Mappings;

namespace CustomerAPI.Core.ValueObjects.Contacts
{
    public class ContactIdResult : ContactResult, IMapFrom
    {
        public string Id { get; set; }

        public override void Mapping(Profile profile)
        {
            profile.CreateMap<Contact, ContactIdResult>();
        }
    }
}
