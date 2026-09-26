using MediatR;
using Nexora.Application.Interfaces;
using Nexora.Domain.Models;

namespace Nexora.Application.Features.User.UserRegister;

public class UserRegisterCommandHandler : IRequestHandler<UserRegisterCommand, Result<bool>>
{
    private readonly IUserRepository _userRepository;

    public UserRegisterCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result<bool>> Handle(UserRegisterCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await _userRepository.GetUserByLoginAsync(request.Login, cancellationToken);
        if (existingUser != null)
        {
            return Result<bool>.Failure("User with this login already exists");
        }

        var newUser = new Domain.Models.User
        {
            Login = request.Login,
            Name = request.Name,
            PasswordHash = request.Password
        };

        await _userRepository.AddUserAsync(newUser, cancellationToken);
        await _userRepository.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}


