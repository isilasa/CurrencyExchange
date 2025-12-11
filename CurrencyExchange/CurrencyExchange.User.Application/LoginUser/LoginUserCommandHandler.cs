using CurrencyExchange.Core.Interfaces;
using CurrencyExchange.Core.Services;
using MediatR;
using System.Security.Authentication;

namespace CurrencyExchange.User.Application.LoginUser
{
    public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, string>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtService _jwtService;

        public LoginUserCommandHandler(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            IJwtService jwtService)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
            _jwtService = jwtService ?? throw new ArgumentNullException(nameof(jwtService));
        }

        public async Task<string> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByNameAsync(request.Login, cancellationToken);

            if (user == null || !_passwordHasher.VerifyHash(request.Password, user.PasswordHash))
                throw new AuthenticationException("Invalid credentials");

            return _jwtService.GenerateToken(user);
        }
    }
}
