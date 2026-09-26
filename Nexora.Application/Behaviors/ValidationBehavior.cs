using FluentValidation;
using MediatR;

namespace Nexora.Application.Behaviors;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);

        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var failures = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToList();

        if (failures.Count != 0)
        {
            var errorMessages = string.Join("; ", failures.Select(f => f.ErrorMessage));
            var failureMethod = typeof(TResponse).GetMethod("Failure", new[] { typeof(string) });

            if (failureMethod != null)
            {
                return (TResponse)failureMethod.Invoke(null, new object[] { errorMessages })!;
            }

            throw new ValidationException(failures);
        }

        return await next();
    }
}
