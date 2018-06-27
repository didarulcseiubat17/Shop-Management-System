using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using ShopApplicationV.Models;
using ShopApplicationV.ViewModels;

namespace ShopApplicationV.Validators 
{
    class LoginValidation : AbstractValidator<LoginViewModel>
    {
        public static Lazy<LoginValidation> Singleton = new Lazy<LoginValidation>(() => new LoginValidation());

        public LoginValidation()
        {
            RuleFor(x => x.Name)
               .NotEmpty().WithMessage("First name is required.")
               .Length(3, 15).WithMessage($"First name have to be longer like 3 characters and shorter like 15 characters.")
               .Matches(@"^[a-zA-Z\s]+$").WithMessage("Contains prohibited symbols");
            RuleFor(x => x.SurName)
                .NotEmpty().WithMessage("Last name is required.")
                .Length(3, 15).WithMessage($"Last name have to be longer like 3 characters and shorter like 15 characters.")
                .Matches(@"^[a-zA-Z\s]+$").WithMessage("Contains prohibited symbols")
                .MinimumLength(3);
            RuleFor(x => x.Email).NotNull().EmailAddress();
            RuleFor(x => x.BirthDate).Must(ValidationHelpers.BeAValidDate).WithMessage("Birth Date is required")
    .Must(ValidationHelpers.BeAValidBirthDate).WithMessage("Age must be in range of 12 to 100");


        }
    }
}
