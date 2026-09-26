using FluentValidation;
using Nexora.API.DTOs.Finance;

namespace Nexora.API.Validators;

public class BalanceRequestValidator : AbstractValidator<BalanceRequest>
{
    public BalanceRequestValidator()
    {
        RuleFor(x => x.Currency)
            .NotEmpty().WithMessage("Currency is required")

            .Must(c => Nexora.Domain.Models.Currency.All.Contains(c)).WithMessage("Unsupported currency");
    }
}
