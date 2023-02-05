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
    public class GenerateLightningInvoiceCommand : IRequest<Result>
    {
        public string PaymentRequest { get; set; }
    }

    public class GenerateLightningInvoiceCommandHandler : IRequestHandler<GenerateLightningInvoiceCommand, Result>
    {
        private readonly ILightningClient _lightningClient;
        public GenerateLightningInvoiceCommandHandler(ILightningClient lightningClient)
        {
            _lightningClient = lightningClient;
        }

        public async Task<Result> Handle(GenerateLightningInvoiceCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _lightningClient.SendLightning(request.PaymentRequest, UserType.User);
                return Result.Success("Invoice paid successfully");
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
