namespace CurrencyExchange.Core.Entities
{
    public class User
    {
        public User()
        {
            Favorites = new List<Favorite>();
        }
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;

        public ICollection<Favorite> Favorites { get; set; }
    }
}