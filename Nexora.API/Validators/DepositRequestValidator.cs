using FluentValidation;
using Nexora.API.DTOs.Finance;

namespace Nexora.API.Validators;

public class DepositRequestValidator : AbstractValidator<DepositRequest>
{
    public DepositRequestValidator()
    {
        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Amount must be greater than 0");
        RuleFor(x => x.Currency)
            .NotEmpty().WithMessage("Currency is required")

            .Must(Nexora.Domain.Models.Currency.All.Contains).WithMessage("Unsupported currency");
    }
}
