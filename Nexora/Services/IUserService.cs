namespace Nexora.Services
{
    public interface IUserService
    {
        Task<bool> RegisterAsync(string login, string name, string password);
    }

}
