using CustomerAPI.Core.Entities;
using FluentValidation;
using Mvp24Hours.Extensions;

namespace CustomerAPI.Core.Validations.Customers
{
    public class CustomerValidator : AbstractValidator<CustomerDto>
    {
        public CustomerValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Customer name is required.");

            // validate email
            When(x => x.Email.HasValue(), () =>
            {
                RuleFor(y => y.Email)
                    .EmailAddress()
                    .WithMessage("Incorrect email.");
            });
        }
    }
}
