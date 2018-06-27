using System;
using System.Linq;
using FluentValidation;
using ShopApplicationV.Models;

namespace ShopApplicationV.Validators
{
    class OrderValidator : AbstractValidator<Order>
    {
        public static Lazy<OrderValidator> Singleton = new Lazy<OrderValidator>(() => new OrderValidator());


        public OrderValidator()
        {
            RuleFor(x => x.ShipDate).NotEmpty().Must(ValidationHelpers.BeAValidDate).GreaterThan(x => x.CreationDate);
            RuleFor(x => x.CreationDate).NotEmpty().Must(ValidationHelpers.BeAValidDate);
            RuleFor(x => x.Total).NotNull();
            RuleFor(x => x.Manager).NotNull();
            RuleFor(x => x.Customer).NotNull();
        }


    }
}
