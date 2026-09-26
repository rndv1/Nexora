using FluentValidation;
using Nexora.API.DTOs.Finance;

namespace Nexora.API.Validators;

public class TransferRequestValidator : AbstractValidator<TransferRequest>
{
    public TransferRequestValidator()
    {
        RuleFor(r => r.ReceiverLogin)
            .NotEmpty().WithMessage("Receiver login is required");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Amount must be greater than 0");

        RuleFor(x => x.Currency)
            .NotEmpty().WithMessage("Currency is required")

            .WithMessage("Unsupported currency");
    }
}
