namespace CurrencyExchange.Core.Entities
{
    public class Favorite
    {
        public Favorite() 
        {
            User = new User();
            Currency = new Currency();
        }

        public Guid UserId { get; set; }
        public Guid CurrencyId { get; set; }

        public User User { get; set; }
        public Currency Currency { get; set; }
    }
}