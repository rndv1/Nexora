using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexora.Database;
using Nexora.Models;

namespace Nexora.Features.User.UserRegister;

public class UserRegisterCommand : IRequest<Result>
{
    public string Login { get; set; }
    public string Name { get; set; }
    public string PasswordHash { get; set; }

    public UserRegisterCommand(string login, string name, string passwordHash)
    {
        Login = login;
        Name = name;
        PasswordHash = passwordHash;
    }
}

public class UserRegisterCommandHandler : IRequestHandler<UserRegisterCommand, Result>
{
    private readonly ApplicationDbContext _context;

    public UserRegisterCommandHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(UserRegisterCommand request, CancellationToken cancellationToken)
    {
        var existing = await _context.Users.FirstOrDefaultAsync(
            x => x.Login == request.Login,
            cancellationToken);
        if (existing != null)
        {
            return Result.Failure("Login already exists");
        }

        var user = new Models.User
        {
            Login = request.Login,
            Name = request.Name,
            PasswordHash = request.PasswordHash,
            Account = new Account
            {
                Balance = 0
            }
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
