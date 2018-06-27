using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using ShopApplicationV.Models;

namespace ShopApplicationV.Validators
{
    class CustomerValidator : AbstractValidator<Customer>
    {
        public static Lazy<CustomerValidator> Singleton = new Lazy<CustomerValidator>(() => new CustomerValidator());

        public CustomerValidator()
        {

            RuleFor(x => x.Name)
               .NotEmpty().WithMessage("First name is required.")
               .Length(3, 15).WithMessage($"First name have to be longer like 3 characters and shorter like 15 characters.")
               .Matches(@"^[a-zA-Z\s]+$").WithMessage("Contains prohibited symbols");
            RuleFor(x => x.SurName)
                .NotEmpty().WithMessage("Last name is required.")
                .Length(3, 15).WithMessage($"Last name have to be longer like 3 characters and shorter like 15 characters.")
                .Matches(@"^[a-zA-Z\s]+$").WithMessage("Contains prohibited symbols");
            RuleFor(x => x.Phone).NotNull().MinimumLength(7).Matches(@"^(1[ \-\+]{0,3}|\+1[ -\+]{0,3}|\+1|\+)?((\(\+?1-[2-9][0-9]{1,2}\))|(\(\+?[2-8][0-9][0-9]\))|(\(\+?[1-9][0-9]\))|(\(\+?[17]\))|(\([2-9][2-9]\))|([ \-\.]{0,3}[0-9]{2,4}))?([ \-\.][0-9])?([ \-\.]{0,3}[0-9]{2,4}){2,3}$");
            RuleFor(x => x.Email).NotNull().EmailAddress();
            RuleFor(x => x.BirthDate).Must(ValidationHelpers.BeAValidDate).WithMessage("Birth Date is required")
                .Must(ValidationHelpers.BeAValidBirthDate).WithMessage("Age must be in range of 12 to 100");
        }
    }
}
