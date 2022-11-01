using MediatR;
using SatoshiDice.Application.Interfaces;
using SatoshiDice.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SatoshiDice.Application.TestEndpoints
{
    public class ValidateAddressCommand : IRequest<Result>
    {
        public List<string> Address { get; set; }
    }

    public class ValidateAddressCommandHandler : IRequestHandler<ValidateAddressCommand, Result>
    {
        private readonly IBitcoinCoreClient _btcClient;
        public ValidateAddressCommandHandler(IBitcoinCoreClient btcClient)
        {
            _btcClient = btcClient;
        }

        public async Task<Result> Handle(ValidateAddressCommand request, CancellationToken cancellationToken)
        {
            try
            {
               var response = _btcClient.BitcoinRequestServer("gettransaction", request.Address);
                return Result.Success("wallet created successfully", response);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
