using ExampleApi.Entities;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExampleApi.Validations
{
    public class UpdateProductValidation : AbstractValidator<Product>
    {
        public UpdateProductValidation()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Name is required");
            RuleFor(x => x.Price)
                .NotEmpty()
                .WithMessage("Price is required")
                .GreaterThan(0)
                .WithMessage("Price must be greather than 0");
            RuleFor(x => x.CategoryId)
                .NotEmpty()
                .WithMessage("Price is required")
                .Must(id => InMemmoryDatabase.Categories.Any(y => y.Id == id))
                .WithMessage("Category not exist");
        }
    }
}
