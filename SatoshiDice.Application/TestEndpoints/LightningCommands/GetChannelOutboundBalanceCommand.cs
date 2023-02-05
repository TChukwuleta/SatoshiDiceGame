using MediatR;
using SatoshiDice.Application.Common.Interfaces;
using SatoshiDice.Domain.Enums;
using SatoshiDice.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SatoshiDice.Application.TestEndpoints.LightningCommands
{
    public class GetChannelOutboundBalanceCommand : IRequest<Result>
    {
    }

    public class GetChannelOutboundBalanceCommandHandler : IRequestHandler<GetChannelOutboundBalanceCommand, Result>
    {
        private readonly ILightningClient _lightningClient;
        public GetChannelOutboundBalanceCommandHandler(ILightningClient lightningClient)
        {
            _lightningClient = lightningClient;
        }

        public async Task<Result> Handle(GetChannelOutboundBalanceCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var balance = await _lightningClient.GetChannelOutboundBalance(UserType.User);
                return Result.Success(balance);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}


