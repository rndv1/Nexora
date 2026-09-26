using FluentValidation;
using Nexora.API.DTOs.User;

namespace Nexora.API.Validators;

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Login)
            .NotEmpty().WithMessage("Login cannot be empty")
            .MinimumLength(4).WithMessage("Login must be at least 4 characters long");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password cannot be empty")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters long");
    }
}
