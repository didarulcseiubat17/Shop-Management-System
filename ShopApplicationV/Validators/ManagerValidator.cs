using System;
using FluentValidation;
using ShopApplicationV.Models;

namespace ShopApplicationV.Validators
{
    public class ManagerValidator : AbstractValidator<Manager>
    {
        public static Lazy<ManagerValidator> Singleton = new Lazy<ManagerValidator>(() => new ManagerValidator());

        public ManagerValidator()
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
            RuleFor(x => x.Salary)
                .Must(x => x >= 0).WithMessage("Salary must be greater then zero.")
                .Must(x => x < 1000000).WithMessage("Salary is too much.");
            RuleFor(x => x.BirthDate).Must(ValidationHelpers.BeAValidDate).WithMessage("Birth Date is required")
    .Must(ValidationHelpers.BeAValidBirthDate).WithMessage("Age must be in range of 12 to 100");

        }
    }
}
