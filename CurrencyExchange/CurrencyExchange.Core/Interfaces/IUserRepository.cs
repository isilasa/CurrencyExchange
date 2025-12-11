using CurrencyExchange.Core.Entities;

namespace CurrencyExchange.Core.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<User?> GetByNameAsync(string name, CancellationToken cancellationToken);
        Task<Guid> AddAsync(User user, CancellationToken cancellationToken);
    }
}
