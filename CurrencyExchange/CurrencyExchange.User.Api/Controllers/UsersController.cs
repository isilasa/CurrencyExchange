using CurrencyExchange.User.Api.Models.Requests;
using CurrencyExchange.User.Application.LoginUser;
using CurrencyExchange.User.Application.RegisterUser;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyExchange.User.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;
        public UsersController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (string.IsNullOrEmpty(dto.Login) || string.IsNullOrEmpty(dto.Password))
                return BadRequest("Incorrect request");

            var id = await _mediator.Send(new RegisterUserCommand
            {
                Login = dto.Login,
                Password = dto.Password
            });

            return CreatedAtAction(null, new { id });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (string.IsNullOrEmpty(dto.Login) || string.IsNullOrEmpty(dto.Password))
                return BadRequest("Incorrect request");

            var token = await _mediator.Send(new LoginUserCommand
            {
                Login = dto.Login,
                Password = dto.Password
            });

            Response.Cookies.Append("UserService", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true, 
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddMinutes(10)
            });

            return Ok();
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(JwtBearerDefaults.AuthenticationScheme);

            return Ok(new { message = "Вы успешно вышли из системы." });
        }
    }
}
