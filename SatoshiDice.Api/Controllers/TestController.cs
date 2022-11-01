using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SatoshiDice.Application.Player.Commands;
using SatoshiDice.Application.TestEndpoints;
using SatoshiDice.Domain.Model;

namespace SatoshiDice.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly IMediator _mediator;
        public TestController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("testbitcoinmethod")]
        public async Task<ActionResult<Result>> CreatePlayer(ValidateAddressCommand command)
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
    }
}
