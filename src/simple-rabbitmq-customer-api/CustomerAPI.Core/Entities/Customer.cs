using Mvp24Hours.Core.Entities;
using System;

namespace CustomerAPI.Core.Entities
{
    public class Customer : EntityBase<int>
    {
        public DateTime Created { get; set; }
        public string Name { get; set; }
        public string CellPhone { get; set; }
        public string Email { get; set; }
        public string Note { get; set; }
        public bool Active { get; set; }
    }
}
