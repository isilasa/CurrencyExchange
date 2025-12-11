using MediatR;

namespace CurrencyExchange.User.Application.RegisterUser
{
    public class RegisterUserCommand : IRequest<Guid>
    {
        public string Login { get; set; }
        public string Password { get; set; }
    }
}
