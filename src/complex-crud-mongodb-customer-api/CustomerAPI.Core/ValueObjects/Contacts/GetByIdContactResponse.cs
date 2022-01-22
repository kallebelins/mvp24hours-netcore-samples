using AutoMapper;
using CustomerAPI.Core.Entities;
using Mvp24Hours.Core.Contract.Mappings;

namespace CustomerAPI.Core.ValueObjects.Contacts
{
    public class GetByIdContactResponse : GetByContactResponse, IMapFrom<Contact>
    {
        public string Id { get; set; }

        public override void Mapping(Profile profile)
        {
            profile.CreateMap<Contact, GetByIdContactResponse>()
                .ForMember(x => x.Created, opt => opt.MapFrom(y => y.Created));
        }
    }
}
