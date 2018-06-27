using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShopApplicationV.ViewModels;

namespace ShopApplicationV.Validators
{
   
    class SettingsValidation : AbstractValidator<SettingsPageViewModel>
    {
        public static Lazy<SettingsValidation> Singleton = new Lazy<SettingsValidation>(() => new SettingsValidation());

        public SettingsValidation()
        {
            RuleFor(x => x.RequiredCustomer)
                .NotEmpty().WithMessage("Required sum to get discont is requied")
                .Must(x => x >= 0).WithMessage("Price must be greater then zero.")
                .Must(x => x < 10000000).WithMessage("Its too much");
            RuleFor(x => x.DiscontRegularCustomer)
                .NotEmpty().WithMessage("Discont percents is required")
                .Must(x => x >= 0).WithMessage("Price must be greater then zero.")
                .Must(x => x < 100).WithMessage("Its too much");
            RuleFor(x => x.Host).NotNull().MinimumLength(3);
            RuleFor(x => x.Email).NotNull().EmailAddress();
        }
    }
}
