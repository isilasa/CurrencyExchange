using CurrencyExchange.Core.Entities;

namespace CurrencyExchange.Core.Services
{
    public interface IJwtService
    {
        string GenerateToken(User user);
    }
}
