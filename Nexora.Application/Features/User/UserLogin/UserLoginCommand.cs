using MediatR;
using Nexora.Application.Interfaces;
using Nexora.Domain.Models;

namespace Nexora.Application.Features.User.UserLogin;

public class UserLoginCommand : IRequest<Result<string>>
{
    public string Login { get; set; }
    public string Password { get; set; }

    public UserLoginCommand(string login, string password)
    {
        Login = login;
        Password = password;
    }
}

