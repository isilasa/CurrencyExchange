namespace CurrencyExchange.Core.Services
{
    public interface IPasswordHasher
    {
        string Hash(string password);
        bool VerifyHash(string password, string hash);
    }
}
