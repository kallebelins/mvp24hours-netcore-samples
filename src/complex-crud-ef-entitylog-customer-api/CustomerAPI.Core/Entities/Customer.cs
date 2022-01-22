using Mvp24Hours.Core.Entities;
using System;
using System.Collections.Generic;

namespace CustomerAPI.Core.Entities
{
    public class Customer : EntityBaseLog<Contact, int, string>
    {
        public string Name { get; set; }
        public string Note { get; set; }
        public bool Active { get; set; }

        public ICollection<Contact> Contacts { get; set; }
    }
}
