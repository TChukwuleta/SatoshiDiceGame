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
            string paymentRequest = default;
            var helper = new LightningHelper(_config);
            try
            {
                switch (userType)
                {
                    case UserType.User:
                        var invoiceResponse = helper.CreateInvoice(satoshis, message);
                        paymentRequest = invoiceResponse.PaymentRequest;
                        break;
                    case UserType.Admin:
                        var adminInvoiceResponse = helper.CreateAdminInvoice(satoshis, message);
                        paymentRequest = adminInvoiceResponse.PaymentRequest;
                        break;
                    default:
                        break;
                }
                return paymentRequest;
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

        public async Task<string> SendLightning(string paymentRequest, UserType userType)
        {
            string result = default;
            var helper = new LightningHelper(_config);
            try
            {
                switch (userType)
                {
                    case UserType.User:
                        var client = helper.GetUserClient();
                        // Decode payment request to get intended amount
                        PayReqString paymentReq = new PayReqString();
                        paymentReq.PayReq = paymentRequest;
                        var decodedPaymentReq = client.DecodePayReq(paymentReq, new Metadata() { new Metadata.Entry("macaroon", helper.GetMacaroon()) });
                        var sendRequest = new SendRequest();
                        sendRequest.Amt = decodedPaymentReq.NumSatoshis;
                        sendRequest.PaymentRequest = paymentRequest;
                        var response = client.SendPaymentSync(sendRequest, new Metadata() { new Metadata.Entry("macaroon", helper.GetMacaroon()) });
                        Console.WriteLine(response);
                        result = response.PaymentError;
                        break;
                    case UserType.Admin:
                        var adminClient = helper.GetAdminClient();
                        // Decode payment request to get intended amount
                        PayReqString adminPaymentReq = new PayReqString();
                        adminPaymentReq.PayReq = paymentRequest;
                        var decodedAdminPaymentReq = adminClient.DecodePayReq(adminPaymentReq, new Metadata() { new Metadata.Entry("macaroon", helper.GetMacaroon()) });
                        var sendAdminRequest = new SendRequest();
                        sendAdminRequest.Amt = decodedAdminPaymentReq.NumSatoshis;
                        sendAdminRequest.PaymentRequest = paymentRequest;
                        var adminResponse = adminClient.SendPaymentSync(sendAdminRequest, new Metadata() { new Metadata.Entry("macaroon", helper.GetMacaroon()) });
                        Console.WriteLine(adminResponse);
                        result = adminResponse.PaymentError;
                        break;
                    default:
                        break;
                }
                return result;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
