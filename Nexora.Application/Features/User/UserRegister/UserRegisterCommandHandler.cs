using MediatR;
using Nexora.Application.Interfaces;
using Nexora.Domain.Models;

namespace Nexora.Application.Features.User.UserRegister;

public class UserRegisterCommandHandler : IRequestHandler<UserRegisterCommand, Result>
{
    private readonly IUserRepository _userRepository;

    public UserRegisterCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result> Handle(UserRegisterCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await _userRepository.GetUserByLoginAsync(request.Login, cancellationToken);
        if (existingUser != null)
        {
            return Result.Failure("User with this login already exists");
        }

        var newUser = new Nexora.Domain.Models.User
        {
            Login = request.Login,
            Name = request.Name,
            PasswordHash = Nexora.Application.Utils.PasswordHasher.Hash(request.Password),
            Accounts = Currency.All.Select(c => new Account { Balance = 0, Currency = c }).ToList()
        };

        await _userRepository.AddUserAsync(newUser, cancellationToken);
        await _userRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
