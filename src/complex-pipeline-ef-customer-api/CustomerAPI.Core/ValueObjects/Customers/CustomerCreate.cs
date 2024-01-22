using AutoMapper;
using CustomerAPI.Core.Entities;
using CustomerAPI.Core.ValueObjects.Contacts;
using Mvp24Hours.Core.Contract.Mappings;
using Mvp24Hours.Extensions;
using System;
using System.Collections.Generic;

namespace CustomerAPI.Core.ValueObjects.Customers
{
    public class CustomerCreate : IMapFrom
    {
        public string Name { get; set; }
        public string Note { get; set; }

        public IList<ContactCreate> Contacts { get; set; }

        public virtual void Mapping(Profile profile)
        {
            profile.CreateMap<CustomerCreate, Customer>()
                .MapProperty(x => DateTime.Now, x => x.Created)
                .MapProperty(x => true, x => x.Active);
        }
    }
}
