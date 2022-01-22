using AutoMapper;
using CustomerAPI.Core.Entities;
using Mvp24Hours.Core.Contract.Mappings;

namespace CustomerAPI.Core.ValueObjects.Customers
{
    public class GetByIdCustomerResponse : GetByCustomerResponse, IMapFrom<Customer>
    {
        public string CellPhone { get; set; }
        public string Email { get; set; }
        public string Note { get; set; }

        public override void Mapping(Profile profile)
        {
            profile.CreateMap<Customer, GetByIdCustomerResponse>();
        }
    }
}
