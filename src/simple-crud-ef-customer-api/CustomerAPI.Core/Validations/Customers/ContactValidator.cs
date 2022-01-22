using CustomerAPI.Core.Entities;
using CustomerAPI.Core.Enums;
using FluentValidation;
using Mvp24Hours.Extensions;

namespace CustomerAPI.Core.Validations.Customers
{
    public class ContactValidator : AbstractValidator<Contact>
    {
        public ContactValidator()
        {
            RuleFor(x => x.Description)
                .NotEmpty()
                .WithMessage("Contact {PropertyName} is required.");

            // validate email
            When(x => x.Type == ContactType.Email, () =>
            {
                RuleFor(y => y.Description)
                    .EmailAddress()
                    .WithMessage("Incorrect email.");
            });

            // validate telefone
            When(x => x.Type == ContactType.CellPhone || x.Type == ContactType.HomePhone || x.Type == ContactType.CommercialPhone, () =>
            {
                RuleFor(y => y.Description)
                    .Must(m => m.IsValidPhoneNumber())
                    .WithMessage("Incorrect phone number.");
            });
        }
    }
}
