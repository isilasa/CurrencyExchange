using CurrencyExchange.Core.Entities;
using CurrencyExchange.Core.Interfaces;
using CurrencyExchange.Finance.Application.GetUserFavoriteRates;
using Moq;

namespace CurrencyExchange.Tests
{
    public class FinanceTests
    {
        [Fact]
        public async Task HandleUserExistsWithFavoritesShouldReturnRates()
        {
            var userId = Guid.NewGuid();
            var user = new Core.Entities.User
            {
                Id = userId,
                Favorites = new List<Favorite>
            {
                new Favorite
                {
                    Currency = new Currency
                    {
                        Name = "USD",
                        Rate = 90.5m,
                        UpdatedAt = DateTime.UtcNow
                    }
                },
                new Favorite
                {
                    Currency = new Currency
                    {
                        Name = "EUR",
                        Rate = 100.2m,
                        UpdatedAt = DateTime.UtcNow.AddDays(-1)
                    }
                }
            }
            };

            var repoMock = new Mock<IUserRepository>();
            repoMock.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Core.Entities.User?)user);

            var handler = new GetUserFavoriteRatesQueryHandler(repoMock.Object);
            var query = new GetUserFavoriteRatesQuery { UserId = userId };

            var result = await handler.Handle(query, CancellationToken.None);

            var ratesList = result.ToList();
            Assert.Equal(2, ratesList.Count);

            Assert.Equal("USD", ratesList[0].CurrencyName);
            Assert.Equal(90.5m, ratesList[0].CurrencyRate);
            Assert.Equal(user.Favorites.ToList()[0].Currency.UpdatedAt, ratesList[0].UpdatedAt);

            Assert.Equal("EUR", ratesList[1].CurrencyName);
            Assert.Equal(100.2m, ratesList[1].CurrencyRate);
            Assert.Equal(user.Favorites.ToList()[1].Currency.UpdatedAt, ratesList[1].UpdatedAt);
        }
    }
}