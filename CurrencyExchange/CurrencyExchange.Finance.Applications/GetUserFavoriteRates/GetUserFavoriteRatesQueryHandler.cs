using CurrencyExchange.Core.Interfaces;
using MediatR;

namespace CurrencyExchange.Finance.Application.GetUserFavoriteRates
{
    public class GetUserFavoriteRatesQueryHandler : IRequestHandler<GetUserFavoriteRatesQuery, IEnumerable<GetUserFavoriteRatesDto>>
    {
        private readonly IUserRepository _userRepository;

        public GetUserFavoriteRatesQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        public async Task<IEnumerable<GetUserFavoriteRatesDto>> Handle(GetUserFavoriteRatesQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

            return user == null
                ? throw new Exception("User not found")
                : user.Favorites.Select(f => new GetUserFavoriteRatesDto
                {
                    CurrencyName = f.Currency.Name,
                    CurrencyRate = f.Currency.Rate,
                    UpdatedAt = f.Currency.UpdatedAt
                });
        }
    }
}
