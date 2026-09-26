using MediatR;
using Nexora.Application.Interfaces;
using Nexora.Domain.Models;

namespace Nexora.Application.Features.User.UserLogin;

public class UserLoginCommand : IRequest<Result<string>>
{
    public string Phone { get; set; }
    public string Password { get; set; }

    public UserLoginCommand(string phone, string password)
    {
        Phone = phone;
        Password = password;
    }
}

