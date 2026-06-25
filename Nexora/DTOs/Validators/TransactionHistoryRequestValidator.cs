using FluentValidation;
using Nexora.DTOs.Finance;

namespace Nexora.DTOs.Validators;

public class TransactionHistoryRequestValidator : AbstractValidator<TransactionHistoryRequest>
{
    public TransactionHistoryRequestValidator()
    {
        RuleFor(x => x.From)
            .Must((request, from) =>
                !from.HasValue ||
                !request.To.HasValue ||
                from.Value <= request.To.Value)
            .WithMessage("From date must not be later than To date");

        RuleFor(x => x.Limit)
            .InclusiveBetween(1, 100).WithMessage("Limit must be between 1 and 100");

        RuleFor(x => x.Offset)
            .GreaterThanOrEqualTo(0).WithMessage("Offset cannot be negative");
    }
}