using FluentValidation;
using Nexora.DTOs.User;

namespace Nexora.DTOs.Validators;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Login)
            .NotEmpty().WithMessage("Login cannot be empty")
            .MinimumLength(4).WithMessage("Login must be at least 4 characters long");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name cannot be empty");

        RuleFor(x => x.PasswordHash)
            .NotEmpty().WithMessage("Password cannot be empty")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters long");
    }
}