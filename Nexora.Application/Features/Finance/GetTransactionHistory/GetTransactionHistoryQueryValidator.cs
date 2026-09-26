using FluentValidation;

namespace Nexora.Application.Features.Finance.GetTransactionHistory;

public class GetTransactionHistoryQueryValidator : AbstractValidator<GetTransactionHistoryQuery>
{
    public GetTransactionHistoryQueryValidator()
    {
        RuleFor(x => x.Skip)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Offset cannot be negative");

        RuleFor(x => x.Take)
            .InclusiveBetween(1, 100)
            .WithMessage("Limit must be between 1 and 100");

        RuleFor(x => x)
            .Custom((query, context) =>
            {
                if (query.DateFrom.HasValue && query.DateTo.HasValue && query.DateFrom.Value > query.DateTo.Value)
                {
                    context.AddFailure("DateFrom", "From date must not be later than To date");
                }
            });
    }
}
