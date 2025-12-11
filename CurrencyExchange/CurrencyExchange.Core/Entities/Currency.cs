namespace CurrencyExchange.Core.Entities
{
    public class Currency
    {
        public Currency()
        {
            Favorites = new List<Favorite>();
        }

        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Rate { get; set; }
        public DateTime UpdatedAt { get; set; }

        public ICollection<Favorite> Favorites { get; set; }
    }
}