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
    public class CreateRawTransactionHelper
    {
        private readonly IBitcoinCoreClient _bitcoinCoreClient;
        private readonly IAppDbContext _context;
        public CreateRawTransactionHelper(IAppDbContext context, IBitcoinCoreClient bitcoinCoreClient)
        {
            _context = context;
            _bitcoinCoreClient = bitcoinCoreClient;
        }

        public async Task<Result> CreateNewTransaction(CreateRawTransactionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Get list of sender utxo
                var utxoList = await _bitcoinCoreClient.BitcoinRequestServer("listunspent");
                var unspentTransactions = JsonSerializer.Deserialize<UTXOResult>(utxoList);
                if (unspentTransactions.result.Count <= 0)
                {
                    return Result.Failure("No unspent transaction found for this user.");
                }
                var availableUTXO = unspentTransactions.result.Sum(c => c.amount);
                var balance = availableUTXO - request.Amount;
                JContainer jArrayOne = new JArray();
                foreach (var item in unspentTransactions.result)
                {
                    JObject fromTxn = new JObject
                    {
                        { "txid", item.txid },
                        { "vout", item.vout }
                    };
                    jArrayOne.Add(fromTxn);
                }

                JObject toTxn = new JObject
                {
                    { request.ToAddress, request.Amount },
                    { request.FromAddress, balance  }
                };
                JContainer jArrayTwo = new JArray
                {
                    toTxn
                };
                var data = await _bitcoinCoreClient.BitcoinRequestServer("createrawtransaction", new List<JToken>() { jArrayOne, jArrayTwo });
                var rawTransactionResponse = JsonSerializer.Deserialize<CreateRawTransactionResponse>(data);
                var signTransaction = await _bitcoinCoreClient.BitcoinRequestServer("signrawtransactionwithwallet", rawTransactionResponse.result);
                var signTransactionResponse = JsonSerializer.Deserialize<SignRawTransactionResponse>(signTransaction);
                var sendRawTxn = await _bitcoinCoreClient.BitcoinRequestServer("sendrawtransaction", signTransactionResponse.result.hex);
                var sendRawTxnResponse = JsonSerializer.Deserialize<CreateRawTransactionResponse>(sendRawTxn);
                return Result.Success("Await implementation");
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
