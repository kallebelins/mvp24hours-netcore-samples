using CustomerAPI.Core.Entities;
using Mvp24Hours.Core.Contract.Domain.Specifications;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace CustomerAPI.Core.Specifications.Customers
{
    /// <summary>
    /// 
    /// </summary>
    public class CustomerHasNoContactSpec : ISpecificationQuery<Customer>
    {
        public Expression<Func<Customer, bool>> IsSatisfiedByExpression => x => !x.Contacts.Any(y => x.Active);
    }
}
