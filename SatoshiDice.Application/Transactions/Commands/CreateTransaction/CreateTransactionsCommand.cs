using FluentValidation.Results;
using MediatR;
using SatoshiDice.Application.Common.Interfaces;
using SatoshiDice.Application.Common.Interfaces.Validators;
using SatoshiDice.Application.Common.Interfaces.Validators.TransactionValidator;
using SatoshiDice.Application.Common.Models;
using SatoshiDice.Application.Interfaces;
using SatoshiDice.Domain.Entities;
using SatoshiDice.Domain.Enums;
using SatoshiDice.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SatoshiDice.Application.Transactions.Commands
{
    public class CreateTransactionsCommand : IRequest<Result>, IBaseValidator
    {
        public List<TransactionRequest> Transactions { get; set; }
        public string UserId { get; set; }
    }

    public class CreateTransactionsCommandHandler : IRequestHandler<CreateTransactionsCommand, Result>
    {
        private readonly IAuthService _authService;
        private readonly ICacheService _cacheService;
        private readonly IAppDbContext _context;

        public CreateTransactionsCommandHandler(IAuthService authService, ICacheService cacheService, IAppDbContext context)
        {
            _authService = authService;
            _cacheService = cacheService;
            _context = context;
        }

        public async Task<Result> Handle(CreateTransactionsCommand request, CancellationToken cancellationToken)
        {
            var transactionReference = $"SatoshiDice_{DateTime.Now.Ticks}";
            try
            {
                var user = await _authService.GetUserById(request.UserId);
                if (user.user == null)
                {
                    return Result.Failure("Transaction creation failed. Invalid user details");
                }
                var transactions = new List<Transaction>();
                foreach (var item in request.Transactions)
                {
                    this.ValidateItem(item);
                    var entity = new Transaction
                    {
                        UserId = item.UserId,
                        Amount = item.Amount,
                        ToUser = item.ToAddress,
                        TransactionType = item.TransactionType,
                        TransactionReference = transactionReference,
                        FromUser = item.FromUser,
                        TransactionStatus = TransactionStatus.Success,
                        CreatedDate = DateTime.Now,
                        Status = Status.Active
                    };
                    transactions.Add(entity);
                }
                await _context.Transactions.AddRangeAsync(transactions);
                await _context.SaveChangesAsync(cancellationToken);

                return Result.Success("Transactions created successfully", transactions);
            }
            catch (Exception ex)
            {
                return Result.Failure(new string[] { "Transaction creation was not successful", ex?.Message ?? ex?.InnerException.Message });
            }
        }

        private void ValidateItem(TransactionRequest item)
        {
            TransactionRequestValidator validator = new TransactionRequestValidator();

            ValidationResult validateResult = validator.Validate(item);
            string validateError = null;

            if (!validateResult.IsValid)
            {
                foreach (var failure in validateResult.Errors)
                {
                    validateError += "Property " + failure.PropertyName + " failed validation. Error was: " + failure.ErrorMessage + "\n";
                }
                throw new Exception(validateError);
            }
        }
    }
}
