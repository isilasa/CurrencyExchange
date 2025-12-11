
using MediatR;

namespace CurrencyExchange.Finance.Application.GetUserFavoriteRates
{
    public class GetUserFavoriteRatesQuery : IRequest<IEnumerable<GetUserFavoriteRatesDto>>
    {
        public Guid UserId { get; set; }
    }
}
