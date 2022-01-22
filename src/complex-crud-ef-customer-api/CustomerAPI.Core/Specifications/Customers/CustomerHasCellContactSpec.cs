using CustomerAPI.Core.Entities;
using CustomerAPI.Core.Enums;
using Mvp24Hours.Core.Contract.Domain.Specifications;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace CustomerAPI.Core.Specifications.Customers
{
    /// <summary>
    /// 
    /// </summary>
    public class CustomerHasCellContactSpec : ISpecificationQuery<Customer>
    {
        public Expression<Func<Customer, bool>> IsSatisfiedByExpression => x => x.Contacts.Any(y => y.Type == ContactType.CellPhone) && x.Active;
    }
}
