using CurrencyExchange.Core.Interfaces;
using CurrencyExchange.Core.Services;
using CurrencyExchange.User.Application.LoginUser;
using CurrencyExchange.User.Application.RegisterUser;
using Moq;
using System.ComponentModel.DataAnnotations;
using System.Security.Authentication;
using Xunit.Abstractions;

namespace CurrencyExchange.Tests
{
    public class UserTests
    {
        [Fact]
        public async Task HandleValidUserShouldAddToRepository()
        {
            var repoMock = new Mock<IUserRepository>();

            repoMock.Setup(r => r.GetByNameAsync("test1", It.IsAny<CancellationToken>()))
                .ReturnsAsync((Core.Entities.User?)null);

            repoMock.Setup(r => r.AddAsync(It.IsAny<Core.Entities.User>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Guid.NewGuid());

            var hashMock = new Mock<IPasswordHasher>();
            hashMock.Setup(h => h.Hash("securePassword"))
                .Returns("hashed_password");

            var handler = new RegisterUserCommandHandler(repoMock.Object, hashMock.Object);
            var command = new RegisterUserCommand { Login = "test1", Password = "securePassword" };

            var userId = await handler.Handle(command, CancellationToken.None);

            Assert.NotEqual(Guid.Empty, userId);
            repoMock.Verify(r => r.AddAsync(It.IsAny<Core.Entities.User>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task HandleUserExistsShouldThrowValidationException()
        {
            var existingUser = new Core.Entities.User
            {
                Id = Guid.NewGuid(),
                Name = "test2"
            };

            var repoMock = new Mock<IUserRepository>();
            repoMock.Setup(r => r.GetByNameAsync("test2", It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingUser);

            var hashMock = new Mock<IPasswordHasher>();
            hashMock.Setup(h => h.VerifyHash("password", "hashed_password"))
                .Returns(true);

            var handler = new RegisterUserCommandHandler(repoMock.Object, hashMock.Object);
            var command = new RegisterUserCommand { Login = "test2", Password = "password" };

            await Assert.ThrowsAsync<ValidationException>(async () =>
                await handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task HandleValidCredentialsShouldReturnJwtToken()
        {
            var user = new Core.Entities.User
            {
                Id = Guid.NewGuid(),
                Name = "test3",
                PasswordHash = "hashedPassword"
            };

            var repoMock = new Mock<IUserRepository>();
            repoMock.Setup(r => r.GetByNameAsync("test3", It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            var hashMock = new Mock<IPasswordHasher>();
            hashMock.Setup(h => h.VerifyHash("securePassword", "hashedPassword"))
                .Returns(true);

            var jwtMock = new Mock<IJwtService>();
            jwtMock.Setup(j => j.GenerateToken(user))
                .Returns("mock.jwt.token");

            var handler = new LoginUserCommandHandler(repoMock.Object, hashMock.Object, jwtMock.Object);
            var query = new LoginUserCommand { Login = "test3", Password = "securePassword" };

            var token = await handler.Handle(query, CancellationToken.None);

            Assert.Equal("mock.jwt.token", token);
        }

        [Fact]
        public async Task HandleInvalidPasswordShouldThrowAuthenticationException()
        {
            var user = new Core.Entities.User
            {
                Id = Guid.NewGuid(),
                Name = "test4",
                PasswordHash = "correctHash"
            };

            var repoMock = new Mock<IUserRepository>();
            repoMock.Setup(r => r.GetByNameAsync("test4", It.IsAny<CancellationToken>()))
                .ReturnsAsync((Core.Entities.User?)user);

            var hashMock = new Mock<IPasswordHasher>();
            hashMock.Setup(h => h.VerifyHash("wrongPassword", "correctHash"))
                .Returns(false);

            var handler = new LoginUserCommandHandler(repoMock.Object, hashMock.Object, Mock.Of<IJwtService>());
            var query = new LoginUserCommand { Login = "test4", Password = "wrongPassword" };

            await Assert.ThrowsAsync<AuthenticationException>(async () =>
                await handler.Handle(query, CancellationToken.None));
        }
    }
}
