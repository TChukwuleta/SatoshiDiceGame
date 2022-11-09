using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SatoshiDice.Domain.Common.Responses
{
    public class UTXOResult
    {
        public List<ListUnspentResponse> result { get; set; }
        public string error { get; set; }
        public string id { get; set; }
    }
    public class ListUnspentResponse
    {
        public string txid { get; set; }
        public int vout { get; set; }
        public string address { get; set; }
        public string scriptPubKey { get; set; }
        public decimal amount { get; set; }
        public int confirmations { get; set; }
        public bool spendable { get; set; }
        public string desc { get; set; }
        public bool solvable { get; set; }
        public bool safe { get; set; }
        public string label { get; set; }
    }
}
