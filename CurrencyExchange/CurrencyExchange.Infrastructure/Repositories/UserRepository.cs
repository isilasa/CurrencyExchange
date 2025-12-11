using CurrencyExchange.Core.Entities;
using CurrencyExchange.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CurrencyExchange.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _dbContext;

        public UserRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<Guid> AddAsync(User user, CancellationToken cancellationToken)
        {
            await _dbContext.Users.AddAsync(user, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return user.Id;
        }

        public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _dbContext.Users.Include(u => u.Favorites).ThenInclude(f => f.Currency)
                       .FirstOrDefaultAsync(u => u.Id.Equals(id), cancellationToken);
        }

        public async Task<User?> GetByNameAsync(string name, CancellationToken cancellationToken)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(u => u.Name.Equals(name), cancellationToken);
        }
    }
}
