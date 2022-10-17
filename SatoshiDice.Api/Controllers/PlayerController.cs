using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SatoshiDice.Application.Player.Commands;
using SatoshiDice.Application.Player.Queries;
using SatoshiDice.Domain.Model;

namespace SatoshiDice.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayerController : ControllerBase
    {
        private readonly IMediator _mediator;
        public PlayerController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("createplayer")]
        public async Task<ActionResult<Result>> CreatePlayer(CreatePlayerCommand command)
        {
            try
            {
                return await _mediator.Send(command);
            }
            catch (Exception ex)
            {
                return Result.Failure($"Failed to create player. Error: {ex?.Message ?? ex?.InnerException?.Message}");
            }
        }

        [HttpPost("update")]
        public async Task<ActionResult<Result>> UpdatePlayer(UpdatePlayerCommand command)
        {
            try
            {
                return await _mediator.Send(command);
            }
            catch (Exception ex)
            {
                return Result.Failure($"Failed to update player. Error: {ex?.Message ?? ex?.InnerException?.Message}");
            }
        }

        [HttpPost("changeplayerstatus")]
        public async Task<ActionResult<Result>> ChangePlayerStatus(ChangePlayerStatusCommand command)
        {
            try
            {
                return await _mediator.Send(command);
            }
            catch (Exception ex)
            {
                return Result.Failure($"Failed to chang player status. Error: {ex?.Message ?? ex?.InnerException?.Message}");
            }
        }

        [HttpGet("getactiveplayers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Result>> GetActivePlayers()
        {
            try
            {
                return await _mediator.Send(new GetActivePlayersQuery());
            }
            catch (Exception ex)
            {
                return Result.Failure($"Active players retrieval failed. Error: {ex?.Message ?? ex?.InnerException?.Message}");
            }
        }

        [HttpGet("getbyid/{userid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Result>> GetPlayerById(string userid)
        {
            try
            {
                return await _mediator.Send(new GetPlayerByIdQuery { UserId = userid });
            }
            catch (Exception ex)
            {
                return Result.Failure($"Player retrieval by id failed. Error: {ex?.Message ?? ex?.InnerException?.Message}");
            }
        }

        [HttpGet("getall/{skip}/{take}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Result>> GetAllPlayers(int skip, int take)
        {
            try
            {
                return await _mediator.Send(new GetPlayersQuery { Skip = skip, Take = take });
            }
            catch (Exception ex)
            {
                return Result.Failure($"Players retrieval failed. Error: {ex?.Message ?? ex?.InnerException?.Message}");
            }
        }
    }
}
