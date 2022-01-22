using CustomerAPI.Core.Entities;
using FluentValidation;

namespace CustomerAPI.Core.Validations.Customers
{
    public class CustomerValidator : AbstractValidator<Customer>
    {
        public CustomerValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Customer {PropertyName} is required.");

            RuleForEach(x => x.Contacts)
                .SetValidator(new ContactValidator());
        }
    }
}
