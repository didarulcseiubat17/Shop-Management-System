using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using ShopApplicationV.Models;

namespace ShopApplicationV.Validators
{
    class ProductValidator : AbstractValidator<Product>
    {
        public static Lazy<ProductValidator> Singleton = new Lazy<ProductValidator>(() => new ProductValidator());

        public ProductValidator()
        {
            RuleFor(x => x.Name)
               .NotNull().WithMessage("Product name is required.")
               .NotEmpty().WithMessage("Product name is required.")
               .Length(3, 15).WithMessage($"Product name have to be longer like 3 characters and shorter like 15 characters.");
//               .Must(x => Char.IsLetter(x.FirstOrDefault()) ?? x?.ToString()).WithMessage("Product name must starts with letter");
            RuleFor(x => x.Price)
                .NotEmpty().WithMessage("Price is requied")
                .Must(x => x > 0).WithMessage("Price must be greater then zero.")
                .Must(x => x < 10000000).WithMessage("Price is too much. You cant' sell such products");
            RuleFor(x => x.AvailableQuantity)
    .Must(x => x >= 0).WithMessage("Quantity cant be less then zero");
        }
    }
}

