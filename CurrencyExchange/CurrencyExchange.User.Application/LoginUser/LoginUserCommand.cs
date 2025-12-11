using MediatR;

namespace CurrencyExchange.User.Application.LoginUser
{
    public class LoginUserCommand : IRequest<string>
    {
        public string Login { get; set; }
        public string Password { get; set; }
    }
}
