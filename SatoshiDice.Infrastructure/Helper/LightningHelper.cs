using Grpc.Core;
using Lnrpc;
using Microsoft.Extensions.Configuration;

namespace SatoshiDice.Infrastructure.Helper
{
    public class LightningHelper
    {
        private readonly IConfiguration _config;
        private readonly string pathToUserMacaroon;
        private readonly string pathToUserSslCertificate;
        private readonly string userGRPCHost;
        private readonly string pathToAdminMacaroon;
        private readonly string pathToAdminSslCertificate;
        private readonly string adminGRPCHost;
        public LightningHelper(IConfiguration config)
        {
            _config = config;
            pathToUserMacaroon = _config["Lightning:UserMacaroonPath"];
            pathToUserSslCertificate = _config["Lightning:UserCertPath"];
            userGRPCHost = _config["Lightning:UserRpcHost"];
            pathToAdminMacaroon = _config["Lightning:AdminMacaroonPath"];
            pathToAdminSslCertificate = _config["Lightning:AdminCertPath"];
            adminGRPCHost = _config["Lightning:AdminRpcHost"];
        }

        public Lnrpc.Lightning.LightningClient GetUserClient()
        {
            var sslCreds = GetSslCredentials();
            // Create channel (Not a lightning channel but a channel to your lightning node)
            var channel = new Grpc.Core.Channel(userGRPCHost, sslCreds);
            var client = new Lnrpc.Lightning.LightningClient(channel);
            return client;
        }

        public Lnrpc.Lightning.LightningClient GetAdminClient()
        {
            var sslCreds = GetSslCredentials();
            // Create channel (Not a lightning channel but a channel to your lightning node)
            var channel = new Grpc.Core.Channel(userGRPCHost, sslCreds);
            var client = new Lnrpc.Lightning.LightningClient(channel);
            return client;
        }

        public string GetMacaroon()
        {
            byte[] macaroonBytes = File.ReadAllBytes(pathToUserMacaroon);
            var macaroon = BitConverter.ToString(macaroonBytes).Replace("-", "");
            return macaroon;
        }

        public string GetAdminMacaroon()
        {
            byte[] macaroonBytes = File.ReadAllBytes(pathToAdminMacaroon);
            var macaroon = BitConverter.ToString(macaroonBytes).Replace("-", "");
            return macaroon;
        }

        public SslCredentials GetSslCredentials()
        {
            // Due to updated ECDSA generated tls.cert we need to let gprc know that
            // we need to use that cipher suite otherwise there will be a handshake
            // error when we communicate with the lnd rpc server.
            Environment.SetEnvironmentVariable("GRPC_SSL_CIPHER_SUITES", "HIGH+ECDSA");
            var cert = File.ReadAllText(pathToUserSslCertificate);
            var sslCreds = new SslCredentials(cert);
            return sslCreds;
        }

        public SslCredentials GetAdminSslCredentials()
        {
            // Due to updated ECDSA generated tls.cert we need to let gprc know that
            // we need to use that cipher suite otherwise there will be a handshake
            // error when we communicate with the lnd rpc server.
            Environment.SetEnvironmentVariable("GRPC_SSL_CIPHER_SUITES", "HIGH+ECDSA");
            var cert = File.ReadAllText(pathToAdminSslCertificate);
            var sslCreds = new SslCredentials(cert);
            return sslCreds;
        }

        public AddInvoiceResponse CreateInvoice(long satoshi, string memo)
        {
            try
            {
                var client = GetUserClient();
                var invoice = new Invoice();
                invoice.Memo = memo;
                invoice.Value = satoshi; // Value in satoshis
                var metadata = new Metadata() { new Metadata.Entry("macaroon", GetMacaroon()) };
                var invoiceResponse = client.AddInvoice(invoice, metadata);
                return invoiceResponse;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public AddInvoiceResponse CreateAdminInvoice(long satoshi, string memo)
        {
            try
            {
                var client = GetAdminClient();
                var invoice = new Invoice();
                invoice.Memo = memo;
                invoice.Value = satoshi; // Value in satoshis
                var metadata = new Metadata() { new Metadata.Entry("macaroon", GetAdminMacaroon()) };
                var invoiceResponse = client.AddInvoice(invoice, metadata);
                return invoiceResponse;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
