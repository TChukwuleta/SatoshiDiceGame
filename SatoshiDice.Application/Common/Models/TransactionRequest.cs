using SatoshiDice.Application.Common.Interfaces.Validators.TransactionValidator;
using SatoshiDice.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SatoshiDice.Application.Common.Models
{
    public class TransactionRequest : ITransactionRequestValidator
    {
        public string UserId { get; set; }
        public string FromUser { get; set; }
        public string ToAddress { get; set; }
        public decimal Amount { get; set; }
        public TransactionType TransactionType { get; set; }
    }
}
