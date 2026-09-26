using FluentValidation;
using Nexora.API.DTOs.Finance;

namespace Nexora.API.Validators;

public class BalanceRequestValidator : AbstractValidator<BalanceRequest>
{
    public BalanceRequestValidator()
    {
        RuleFor(x => x.Currency)
            .NotEmpty().WithMessage("Currency is required")

            .Must(Nexora.Domain.Models.Currency.All.Contains).WithMessage("Unsupported currency");
    }
}
