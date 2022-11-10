using SatoshiDice.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SatoshiDice.Domain.Common.Responses
{
    public class SignRawTransactionResponse
    {
        public RawTransaction result { get; set; }
        public string error { get; set; }
        public string id { get; set; }
    }

    public class RawTransaction
    {
        public string hex { get; set; }
        public bool complete { get; set; }
    }
}
