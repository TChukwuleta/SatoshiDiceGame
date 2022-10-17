using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SatoshiDice.Application.Player.Commands;
using SatoshiDice.Application.PlayGame.Commands;
using SatoshiDice.Domain.Model;

namespace SatoshiDice.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameController : ControllerBase
    {
        private readonly IMediator _mediator;
        public GameController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("playgame")]
        public async Task<ActionResult<Result>> PlayGame(PlayGameCommand command)
        {
            try
            {
                return await _mediator.Send(command);
            }
            catch (Exception ex)
            {
                return Result.Failure($"Failed to play game. Error: {ex?.Message ?? ex?.InnerException?.Message}");
            }
        }
    }
}
