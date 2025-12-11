
namespace CurrencyExchange.Finance.Application.GetUserFavoriteRates
{
    public class GetUserFavoriteRatesDto
    {
        public string CurrencyName { get; set; }
        public decimal CurrencyRate { get; set; }
        public DateTime UpdatedAt {  get; set; }
    }
}
