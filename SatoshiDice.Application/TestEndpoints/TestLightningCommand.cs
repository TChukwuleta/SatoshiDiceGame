using MediatR;
using SatoshiDice.Application.Common.Interfaces;
using SatoshiDice.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SatoshiDice.Application.TestEndpoints
{
    public class TestLightningCommand : IRequest<Result>
    {
        public string MethodName { get; set; }
    }

    public class TestLightningCommandHandler : IRequestHandler<TestLightningCommand, Result>
    {
        private readonly ILightningClient _lightningClient;
        public TestLightningCommandHandler(ILightningClient lightningClient)
        {
            _lightningClient = lightningClient;
        }

        public async Task<Result> Handle(TestLightningCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _lightningClient.LightningRequestServer(request.MethodName);
                return Result.Success(response);
            }
            catch (Exception ex)
            {

                throw ex;
            } 
        }
    }
}
