using CurrencyExchange.Core.Interfaces;
using CurrencyExchange.Core.Services;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace CurrencyExchange.User.Application.RegisterUser
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Guid>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;

        public RegisterUserCommandHandler(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
        }

        public async Task<Guid> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByNameAsync(request.Login, cancellationToken);

            if (user is not null)
                throw new ValidationException("User already exists");

            var passwordHash = _passwordHasher.Hash(request.Password);

            return await _userRepository.AddAsync(new Core.Entities.User { Name = request.Login, PasswordHash = passwordHash }, cancellationToken);
        }
    }
}