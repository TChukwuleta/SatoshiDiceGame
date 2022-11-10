using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SatoshiDice.Domain.Common.Responses
{
    public class CreateRawTransactionResponse
    {
        public string result { get; set; }
        public string error { get; set; }
        public string id { get; set; }
    }
}
