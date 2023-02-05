using MediatR;
using SatoshiDice.Application.Common.Interfaces;
using SatoshiDice.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SatoshiDice.Application.TestEndpoints.LightningCommands
{
    public class GetChannelInbountBalanceCommand: IRequest<Result>
    {
    }

    public class GetChannelInbountBalanceCommandHandler : IRequestHandler<GetChannelInbountBalanceCommand, Result>
    {
        private readonly ILightningClient _lightningClient;
        public GetChannelInbountBalanceCommandHandler(ILightningClient lightningClient)
        {
            _lightningClient = lightningClient;
        }

        public async Task<Result> Handle(GetChannelInbountBalanceCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var balance = await _lightningClient.GetChannelInboundBalance(Domain.Enums.UserType.User);
                return Result.Success(balance);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
