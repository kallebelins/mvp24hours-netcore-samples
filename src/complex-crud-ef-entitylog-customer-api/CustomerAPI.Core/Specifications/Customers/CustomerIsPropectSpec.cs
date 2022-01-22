using CustomerAPI.Core.Entities;
using Mvp24Hours.Core.Contract.Domain.Specifications;
using System;
using System.Linq.Expressions;

namespace CustomerAPI.Core.Specifications.Customers
{
    /// <summary>
    /// 
    /// </summary>
    public class CustomerIsPropectSpec : ISpecificationQuery<Customer>
    {
        public Expression<Func<Customer, bool>> IsSatisfiedByExpression => x => x.Note.Contains("prospect", StringComparison.InvariantCultureIgnoreCase);
    }
}
