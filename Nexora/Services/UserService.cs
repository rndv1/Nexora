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

        public async Task<bool> RegisterAsync(string login, string name, string password) 
        {

            var existing = await _context.Users.FirstOrDefaultAsync(predicate: x => x.Login == login);
            if (existing != null)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
            {
                return false;
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

            return true;
        }
    }

}
