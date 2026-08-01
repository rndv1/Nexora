using FluentValidation;
using Nexora.DTOs.Finance;
using Nexora.Models;

namespace Nexora.DTOs.Validators;

public class BalanceRequestValidator : AbstractValidator<BalanceRequest>
{
    public BalanceRequestValidator()
    {
        RuleFor(x => x.Currency)
            .NotEmpty().WithMessage("Currency is required")
            .Must(currency => currency is not null && Currency.All.Contains(currency))
            .WithMessage("Unsupported currency");
    }
}