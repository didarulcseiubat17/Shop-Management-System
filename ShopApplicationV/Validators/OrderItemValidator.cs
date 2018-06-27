
using FluentValidation;
using ShopApplicationV.Models;
using System;

namespace ShopApplicationV.Validators
{

    public class OrderItemValidator : AbstractValidator<OrderItem>
    {
        public static Lazy<OrderItemValidator> Singleton = new Lazy<OrderItemValidator>(() => new OrderItemValidator());

        public OrderItemValidator()
        {
            RuleFor(x => x.Product)
                .NotNull()
                .WithMessage("Select product");
            RuleFor(x => x.Quantity)
                .GreaterThan(0)
                .NotEqual(0).WithMessage("Cant be zero")
                .LessThanOrEqualTo(x => x.Product!=null ? x.Product.AvailableQuantity : 0);
        }
    }

}
