using FluentValidation;
using Shared.Models;

namespace API.Validators
{
    public class TariffValidator : AbstractValidator<Tariff>
    {
        public TariffValidator()
        {
            RuleFor(t => t.Name)
                .NotEmpty().WithMessage("Name is required").WithErrorCode("ERR001")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters").WithErrorCode("ERR002");

            //RuleFor(t => t.Type)
            //    .InclusiveBetween(1, 2).WithMessage("Type must be between 1 and 2").WithErrorCode("ERR003");

            RuleFor(t => t.BaseCost)
                .GreaterThanOrEqualTo(0).WithMessage("BaseCost must be a positive number").WithErrorCode("ERR004");

            RuleFor(t => t.IncludedKwh)
                .GreaterThanOrEqualTo(0).WithMessage("IncludedKwh must be a positive number or null").WithErrorCode("ERR005");

            RuleFor(t => t.AdditionalKwhCost)
                .GreaterThanOrEqualTo(0).WithMessage("AdditionalKwhCost must be a positive number or null").WithErrorCode("ERR006");
        }
    }
}
