using AutoMapper;
using CustomerAPI.Core.Entities;
using Mvp24Hours.Core.Contract.Mappings;

namespace ContactAPI.Core.ValueObjects.Contacts
{
    public class CreateContactResponse : IMapFrom<Contact>
    {
        public int Id { get; set; }

        public virtual void Mapping(Profile profile)
        {
            profile.CreateMap<Contact, CreateContactResponse>();
        }
    }
}
