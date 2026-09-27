using MediatR;
using Nexora.Application.Interfaces;
using Nexora.Domain.Models;

namespace Nexora.Application.Features.User.UserLogin;

public class UserLoginCommandHandler : IRequestHandler<UserLoginCommand, Result<string>>
{
    private readonly IUserRepository _userRepository;
    private readonly ISessionRepository _sessionRepository;

    public UserLoginCommandHandler(IUserRepository userRepository, ISessionRepository sessionRepository)
    {
        _userRepository = userRepository;
        _sessionRepository = sessionRepository;
    }

    public async Task<Result<string>> Handle(UserLoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserByLoginAsync(request.Login, cancellationToken);

        if (user == null)
        {
            return Result<string>.Failure("Invalid login or password");
        }

        bool isPasswordValid = Nexora.Application.Utils.PasswordHasher.Verify(request.Password, user.PasswordHash);

        if (!isPasswordValid)
        {
            return Result<string>.Failure("Invalid login or password");
        }

        var token = Guid.NewGuid().ToString("N");

        var session = new Session
        {
            UserId = user.Id,
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddHours(1)
        };

        await _sessionRepository.UpsertAsync(session, cancellationToken);
        await _sessionRepository.SaveChangesAsync(cancellationToken);

        return Result<string>.Success(token);
    }
}



