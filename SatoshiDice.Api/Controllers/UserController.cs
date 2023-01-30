using MediatR;
using Microsoft.AspNetCore.Mvc;
using SatoshiDice.Application.BitcoinMethods;
using SatoshiDice.Application.Users.Commands;
using SatoshiDice.Application.Users.Queries;
using SatoshiDice.Domain.Model;

namespace SatoshiDice.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;
        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("create")]
        public async Task<ActionResult<Result>> CreateUser(CreateUserCommand command)
        {
            try
            {
                return await _mediator.Send(command);
            }
            catch (Exception ex)
            {
                return Result.Failure($"User creation was not successful. Error: {ex?.Message ?? ex?.InnerException?.Message}");
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<Result>> Login(UserLoginCommand command)
        {
            try
            {
                return await _mediator.Send(command);
            }
            catch (Exception ex)
            {
                return Result.Failure($"User login was not successful. Error: {ex?.Message ?? ex?.InnerException?.Message}");
            }
        }

        [HttpPost("createrawtransaction")]
        public async Task<ActionResult<Result>> CreateRawTransaction(CreateRawTransactionCommand command)
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

        [HttpGet("getusersbyid/{userId}")]
        public async Task<ActionResult<Result>> GetUserByRoleId(string userId)
        {
            try
            {
                return await _mediator.Send(new GetUserByIdQuery
                {
                    UserId = userId
                });
            }
            catch (Exception ex)
            {
                return Result.Failure(new string[] { "User retrieval was not successful" + ex?.Message ?? ex?.InnerException?.Message });
            }
        }
    }
}
