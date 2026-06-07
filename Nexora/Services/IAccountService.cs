namespace Nexora.Services
{
    public interface IAccountService
    {
        Task CreateAccountAsync(string login);
    }
}
