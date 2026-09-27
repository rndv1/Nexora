using MediatR;
using Nexora.Application.Interfaces;
using Nexora.Domain.Models;

namespace Nexora.Application.Features.User.UserRegister;

public class UserRegisterCommand : IRequest<Result>
{
    public string Login { get; set; }
    public string Name { get; set; }
    public string Password { get; set; }

    public UserRegisterCommand(string login, string name, string password)
    {
        Login = login;
        Name = name;
        Password = password;
    }
}

