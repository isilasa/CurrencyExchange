using CurrencyExchange.Finance.Application.GetUserFavoriteRates;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyExchange.Finance.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FinancesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public FinancesController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpGet("user/{userid}")]
        public async Task<IActionResult> GetMyRates([FromRoute] Guid userId)
        {
            var result = await _mediator.Send(new GetUserFavoriteRatesQuery
            {
                UserId = userId
            });

            return Ok(result);
        }
    }
}