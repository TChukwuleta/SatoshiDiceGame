﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SatoshiDice.Application.Interfaces
{
    public interface IBitcoinCoreClient
    {
        Task<string> BitcoinRequestServer(string methodName, List<JToken> parameters);
        Task<string> BitcoinRequestServer(string methodName, List<string> parameters);
        Task<string> BitcoinRequestServer(string methodName, string parameters);
        Task<string> BitcoinRequestServer(string methodName);
        string GetRawTransaction(string txid);
    }
}
