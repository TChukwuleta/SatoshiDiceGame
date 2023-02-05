using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SatoshiDice.Application.Player;
using SatoshiDice.Application.TestEndpoints;
using SatoshiDice.Application.TestEndpoints.LightningCommands;
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

        [HttpPost("payinvoice")]
        public async Task<ActionResult<Result>> LightningPayInvoice(PayInvoiceCommand command)
        {
            try
            {
                return await _mediator.Send(command);
            }
            catch (Exception ex)
            {
                return Result.Failure($"Failed to make invoice payment. Error: {ex?.Message ?? ex?.InnerException?.Message}");
            }
        }

        [HttpPost("generateinvoice")]
        public async Task<ActionResult<Result>> LightningGenerateInvoice(GenerateLightningInvoiceCommand command)
        {
            try
            {
                return await _mediator.Send(command);
            }
            catch (Exception ex)
            {
                return Result.Failure($"Failed to generate invoice. Error: {ex?.Message ?? ex?.InnerException?.Message}");
            }
        }

        [HttpPost("walletbalance")]
        public async Task<ActionResult<Result>> LightningWalletBalance(GetLightningWalletCommand command)
        {
            try
            {
                return await _mediator.Send(command);
            }
            catch (Exception ex)
            {
                return Result.Failure($"Failed to get lightning wallet balance. Error: {ex?.Message ?? ex?.InnerException?.Message}");
            }
        }

        [HttpPost("outboundbalance")]
        public async Task<ActionResult<Result>> LightningOutboundBalance(GetChannelOutboundBalanceCommand command)
        {
            try
            {
                return await _mediator.Send(command);
            }
            catch (Exception ex)
            {
                return Result.Failure($"Failed to get channel outbound balance. Error: {ex?.Message ?? ex?.InnerException?.Message}");
            }
        }

        [HttpPost("inboundbalance")]
        public async Task<ActionResult<Result>> LightningInboundBalance(GetChannelInbountBalanceCommand command)
        {
            try
            {
                return await _mediator.Send(command);
            }
            catch (Exception ex)
            {
                return Result.Failure($"Failed to get cahnnel inbound balance. Error: {ex?.Message ?? ex?.InnerException?.Message}");
            }
        }
    }
}
