using Microsoft.EntityFrameworkCore;
using Nexora.Database;
using Nexora.Models;


namespace Nexora.Services
{
    public class UserService : IUserService
    {

        private readonly ApplicationDbContext _context;
        private readonly IAccountService _accountServices;

        public UserService(ApplicationDbContext context, IAccountService accountServices)
        {
            _context = context;
            _accountServices=accountServices;
        }

        public async Task<Result> RegisterAsync(string login, string name, string password) 
        {

            var existing = await _context.Users.FirstOrDefaultAsync(predicate: x => x.Login == login);
            if (existing != null)
            {
                return Result.Failure("Login already exists");
            }

            var user = new User
            {
                Login = login,
                Name = name,
                PasswordHash = password
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            await _accountServices.CreateAccountAsync(login);

            return Result.Success();
        }

        public async Task<Result<string>> LoginAsync(string login, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(predicate: x => x.Login == login && x.PasswordHash == 
             password);
            if(user == null)
            {
                return Result<string>.Failure("Invalid login or password");
            }

            var session = new Session
            {
                UserId = user.Id,
                Token = Guid.NewGuid().ToString(),
                ExpiresAt = DateTime.UtcNow.AddHours(1)
            };

            var existingSession = await _context.Sessions.FirstOrDefaultAsync(predicate: x => x.UserId == user.Id);
            if (existingSession != null)
            {
                existingSession.Token = session.Token; 
                existingSession.ExpiresAt = session.ExpiresAt;
                await _context.SaveChangesAsync();
            }
            else
            {
                _context.Sessions.Add(session);
                await _context.SaveChangesAsync();
            }

            return Result<string>.Success(session.Token);
        }
    }
}

