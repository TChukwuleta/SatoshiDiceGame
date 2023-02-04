using Grpc.Core;
using Grpc.Net.Client;
using Lnrpc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SatoshiDice.Application.Common.Interfaces;
using SatoshiDice.Domain.Enums;
using SatoshiDice.Infrastructure.Helper;
using System.Net;
using System.Text;

namespace SatoshiDice.Infrastructure.Services
{
    public class LightningClient : ILightningClient
    {
        private readonly IConfiguration _config;
        public LightningClient(IConfiguration config)
        {
            _config = config;
        }

        public async Task<string> CreateInvoice(long satoshis, string message, UserType userType)
        {
            try
            {
                /*var helper = new LightningHelper(_config);
                var invoiceResponse = helper.CreateInvoice(satoshis, message);
                var result = new
                {
                    invoice = invoiceResponse,
                    date = DateTime.Now
                };
                var invoice = JsonConvert.DeserializeObject(result);*/
                return "Ok";
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<long> GetChannelInboundBalance(UserType userType)
        {
            long response = default;
            try
            {
                switch (userType)
                {
                    case UserType.User:
                        var helper = new LightningHelper(_config);
                        var client = helper.GetUserClient();
                        var channelBalanceRequest = new ChannelBalanceRequest();
                        response = client.ChannelBalance(channelBalanceRequest, new Metadata() { new Metadata.Entry("macaroon", helper.GetMacaroon()) }).Balance;
                        break;
                    case UserType.Admin:
                        break;
                    default:
                        throw new ArgumentException("Invalid user type specified");
                }
                return response;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<long> GetChannelOutboundBalance(UserType userType)
        {
            long response = default;
            var helpers = new LightningHelper(_config);
            try
            {
                switch (userType)
                {
                    case UserType.User:
                        var client = helpers.GetUserClient();
                        var channelBalanceRequest = new ChannelBalanceRequest();
                        response = client.ChannelBalance(channelBalanceRequest, new Metadata() { new Metadata.Entry("macaroon", helpers.GetMacaroon()) }).Balance;
                        break;
                    case UserType.Admin:
                        var adminClient = helpers.GetAdminClient();
                        var channelAdminBalanceRequest = new ChannelBalanceRequest();
                        response = adminClient.ChannelBalance(channelAdminBalanceRequest, new Metadata() { new Metadata.Entry("macaroon", helpers.GetMacaroon()) }).Balance;
                        break;
                    default:
                        throw new ArgumentException("Invalid user type specified");
                }
                
                return response;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<long> GetWalletBalance(UserType userType)
        {
            try
            {
                var helpers = new LightningHelper(_config);
                var client = helpers.GetUserClient();
                var response = client.WalletBalance(new WalletBalanceRequest(), new Metadata() { new Metadata.Entry("macaroon", helpers.GetMacaroon()) });
                return response.TotalBalance;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void SendLightning(string paymentRequest, UserType userType)
        {
            try
            {
                var helper = new LightningHelper(_config);
                var client = helper.GetUserClient();
                var sendRequest = new SendRequest();
                sendRequest.Amt = 1000;
                sendRequest.PaymentRequest = paymentRequest;
                var response = client.SendPaymentSync(sendRequest, new Metadata() { new Metadata.Entry("macaroon", helper.GetMacaroon())});
                Console.WriteLine(response);

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
