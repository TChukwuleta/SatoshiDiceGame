using SatoshiDice.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SatoshiDice.Application.Common.Interfaces.Validators.TransactionValidator
{
    public interface ITransactionRequestValidator
    {
        public string FromUser { get; set; }
        public decimal Amount { get; set; }
        public string ToAddress { get; set; }
        public TransactionType TransactionType { get; set; }
    }
}
