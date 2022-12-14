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
        public string ToAddress { get; set; }
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
                var listUnspentResponse = await _bitcoinCoreClient.BitcoinRequestServer("listunspent");
                var listUnspentTransactions = JsonSerializer.Deserialize<UTXOResult>(listUnspentResponse);
                var paymentTx = listUnspentTransactions.result.FirstOrDefault(c => c.amount > 1);
                var maxTx = listUnspentTransactions.result.Max(c => c.amount);
                if(paymentTx != null)
                {
                    // Pay with that particular transaction
                }
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
                    { request.ToAddress, 0.000005 }
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
                return Result.Success("Getting list of UTXO was successful", sendRawTransactionResponse);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
