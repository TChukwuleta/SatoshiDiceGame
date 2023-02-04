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
    public class GetLightningWalletCommand : IRequest<Result>
    {
    }

    public class GetLightningWalletCommandHandler : IRequestHandler<GetLightningWalletCommand, Result>
    {
        private readonly ILightningClient _lightningClient;
        public GetLightningWalletCommandHandler(ILightningClient lightningClient)
        {
            _lightningClient = lightningClient;
        }

        public async Task<Result> Handle(GetLightningWalletCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var balance = await _lightningClient.GetWalletBalance();
                return Result.Success(balance);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
