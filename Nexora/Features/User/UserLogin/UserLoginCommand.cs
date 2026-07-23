using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexora.Database;
using Nexora.Models;

namespace Nexora.Features.User.UserLogin;

public class UserLoginCommand : IRequest<Result<string>>
{
    public string Login { get; set; }
    public string PasswordHash { get; set; }

    public UserLoginCommand(string login, string password)
    {
        Login = login;
        PasswordHash = password;
    }
}

public class UserLoginCommandHandler : IRequestHandler<UserLoginCommand, Result<string>>
{
    private readonly ApplicationDbContext _context;

    public UserLoginCommandHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<string>> Handle(UserLoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users.FirstOrDefaultAsync(
            x => x.Login == request.Login && x.PasswordHash == request.PasswordHash,
            cancellationToken);
        if (user == null)
        {
            return Result<string>.Failure("Invalid login or password");
        }

        var session = new Session
        {
            UserId = user.Id,
            Token = Guid.NewGuid().ToString(),
            ExpiresAt = DateTime.UtcNow.AddHours(1)
        };

        var existingSession = await _context.Sessions.FirstOrDefaultAsync(
            x => x.UserId == user.Id,
            cancellationToken);
        if (existingSession != null)
        {
            existingSession.Token = session.Token;
            existingSession.ExpiresAt = session.ExpiresAt;
            await _context.SaveChangesAsync(cancellationToken);
        }
        else
        {
            _context.Sessions.Add(session);
            await _context.SaveChangesAsync(cancellationToken);
        }

        return Result<string>.Success(session.Token);
    }
}
