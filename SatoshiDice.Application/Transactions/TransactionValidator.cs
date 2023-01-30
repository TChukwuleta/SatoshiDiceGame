using FluentValidation;
using SatoshiDice.Application.Common.Interfaces.Validators.TransactionValidator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SatoshiDice.Application.Transactions
{
    public class TransactionValidator
    {
    }

    public class TransactionRequestValidator : AbstractValidator<ITransactionRequestValidator>
    {
        public TransactionRequestValidator()
        {
            RuleFor(c => c.Amount).NotEmpty().WithMessage("Amount must be specified");
            RuleFor(c => c.TransactionType).NotEmpty().WithMessage("Transaction type must be speicified");
            RuleFor(c => c.FromUser).NotEmpty().WithMessage("Sender (FromUser) must be specified");
            RuleFor(c => c.ToAddress).NotEmpty().WithMessage("Recipient (ToAddress) must be specified");
        }
    }
}
