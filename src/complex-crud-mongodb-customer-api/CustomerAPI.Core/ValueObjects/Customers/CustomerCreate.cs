using AutoMapper;
using CustomerAPI.Core.Entities;
using Mvp24Hours.Core.Contract.Mappings;
using Mvp24Hours.Extensions;
using System;

namespace CustomerAPI.Core.ValueObjects.Customers
{
    public class CustomerCreate : IMapFrom
    {
        public string Name { get; set; }
        public string Note { get; set; }

        public virtual void Mapping(Profile profile)
        {
            profile.CreateMap<CustomerCreate, Customer>()
                .MapProperty(x => Guid.NewGuid().ToString(), x => x.Id)
                .MapProperty(x => DateTime.Now, x => x.Created)
                .MapProperty(x => true, x => x.Active);
        }
    }
}
