using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SatoshiDice.Domain.Common.Responses
{
    public class GetWalletInfoResponse
    {
        public GetWalletInfoResult result { get; set; }
        public string error { get; set; }
        public string id { get; set; }
    }

    public class GetWalletInfoResult
    {
        public string walletname { get; set; }
        public int walletversion { get; set; }
        public string format { get; set; }
        public decimal balance { get; set; }
        public decimal unconfirmed_balance { get; set; }
        public decimal immature_balance { get; set; }
        public int txcount { get; set; }
        public int keypoololdest { get; set; }
        public int keypoolsize { get; set; }
        public string hdseedid { get; set; }
        public int keypoolsize_hd_internal { get; set; }
        public decimal paytxfee { get; set; }
        public bool private_keys_enabled { get; set; }
        public bool avoid_reuse { get; set; }
        public bool scanning { get; set; }
        public bool descriptors { get; set; }
    }
}