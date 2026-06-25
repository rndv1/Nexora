using FluentValidation;
using Nexora.DTOs.Finance;

namespace Nexora.DTOs.Validators;

public class TransferRequestValidator : AbstractValidator<TransferRequest>
{
    public TransferRequestValidator()
    {
        RuleFor(r => r.ReceiverLogin)
            .NotEmpty().WithMessage("Receiver login is required");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Amount must be greater than 0");
    }
}