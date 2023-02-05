using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SatoshiDice.Application.BitcoinMethods;
using SatoshiDice.Application.Game.Commands;
using SatoshiDice.Domain.Model;

namespace SatoshiDice.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameController : ApiController
    {
        protected readonly IHttpContextAccessor _contextAccessor;
        private readonly string accessToken;
        public GameController(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
            accessToken = _contextAccessor.HttpContext.Request.Headers["Authorization"].ToString();
            if (string.IsNullOrEmpty(accessToken))
            {
                throw new Exception("You are not authorized!");
            }
        }

        [HttpPost("playgame")]
        public async Task<ActionResult<Result>> PlayGame(PlayGameCommand command)
        {
            try
            {
                return await Mediator.Send(command);
            }
            catch (Exception ex)
            {
                return Result.Failure($"Failed to play game. Error: {ex?.Message ?? ex?.InnerException?.Message}");
            }
        }

        [HttpPost("createrawtransaction")]
        public async Task<ActionResult<Result>> CreateRawTransaction(CreateRawTransactionCommand command)
        {
            try
            {
                return await Mediator.Send(command);
            }
            catch (Exception ex)
            {
                return Result.Failure($"Failed to play game. Error: {ex?.Message ?? ex?.InnerException?.Message}");
            }
        }
    }
}
