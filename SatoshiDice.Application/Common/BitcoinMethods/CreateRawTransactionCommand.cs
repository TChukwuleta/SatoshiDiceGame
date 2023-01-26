using MediatR;
using Newtonsoft.Json.Linq;
using SatoshiDice.Application.Interfaces;
using SatoshiDice.Domain.Common.Responses;
using SatoshiDice.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SatoshiDice.Application.BitcoinMethods
{
    public class CreateRawTransactionCommand : IRequest<Result>
    {
        public string FromAddress { get; set; }
        public string ToAddress { get; set; }
        public decimal Amount { get; set; }
    }

    public class CreateRawTransactionCommandHandler : IRequestHandler<CreateRawTransactionCommand, Result>
    {
        private readonly IBitcoinCoreClient _bitcoinCoreClient;
        public CreateRawTransactionCommandHandler(IBitcoinCoreClient bitcoinCoreClient)
        {
            _bitcoinCoreClient = bitcoinCoreClient;
        }

        public async Task<Result> Handle(CreateRawTransactionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // List unspent transaction
                string senderAddress = default;
                if (string.IsNullOrEmpty(request.FromAddress))
                {
                    //var getNewAddress = await _bitcoinCoreClient.BitcoinRequestServer("getnewaddress");
                }
                var listUnspentResponse = await _bitcoinCoreClient.BitcoinRequestServer("listunspent");
                var listUnspentTransactions = JsonSerializer.Deserialize<UTXOResult>(listUnspentResponse);
                if(listUnspentTransactions.result.Count <= 0)
                {
                    return Result.Failure("No available UTXO. Kindly fund your account using a test faucet");
                }
                var totalUtxoAvailable = listUnspentTransactions.result.Sum(c => c.amount);
                var balance = totalUtxoAvailable - request.Amount;
                JContainer jArray = new JArray();
                foreach (var txn in listUnspentTransactions.result)
                {
                    JObject jFromTx = new JObject
                    {
                        { "txid", txn.txid },
                        { "vout", txn.vout }
                    };
                    jArray.Add(jFromTx);
                }
                JObject jtoTx = new JObject
                {
                    { request.ToAddress, request.Amount },
                    { request.FromAddress, balance  }
                };
                JContainer jArray2 = new JArray
                {
                    jtoTx
                };
                var data = await _bitcoinCoreClient.BitcoinRequestServer("createrawtransaction", new List<JToken>() { jArray, jArray2 });
                var rawTransactionResponse = JsonSerializer.Deserialize<CreateRawTransactionResponse>(data);
                var signRawTransaction = await _bitcoinCoreClient.BitcoinRequestServer("signrawtransactionwithwallet", rawTransactionResponse.result);
                var signRawTransactionResponse = JsonSerializer.Deserialize<SignRawTransactionResponse>(signRawTransaction);
                var sendRawTransaction = await _bitcoinCoreClient.BitcoinRequestServer("sendrawtransaction", signRawTransactionResponse.result.hex);
                var sendRawTransactionResponse = JsonSerializer.Deserialize<CreateRawTransactionResponse>(sendRawTransaction);
                return Result.Success("Creating and sending transaction was successful", sendRawTransactionResponse);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
