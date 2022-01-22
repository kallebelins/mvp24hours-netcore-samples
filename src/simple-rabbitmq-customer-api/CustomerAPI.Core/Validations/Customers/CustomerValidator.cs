using CustomerAPI.Core.Entities;
using FluentValidation;
using Mvp24Hours.Extensions;

namespace CustomerAPI.Core.Validations.Customers
{
    public class CustomerValidator : AbstractValidator<Customer>
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

            // validate telefone
            When(x => x.CellPhone.HasValue(), () =>
            {
                RuleFor(y => y.CellPhone)
                    .Must(m => m.IsValidPhoneNumber())
                    .WithMessage("Incorrect phone number.");
            });
        }
    }
}
