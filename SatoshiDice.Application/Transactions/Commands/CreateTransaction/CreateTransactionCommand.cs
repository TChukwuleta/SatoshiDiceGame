using MediatR;
using SatoshiDice.Application.Common.Interfaces;
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
    public class CreateTransactionCommand : IRequest<Result>
    {
        public string UserId { get; set; }
        public decimal Amount { get; set; }
        public TransactionType TransactionType { get; set; }
    }

    public class CreateTransactionCommandHandler : IRequestHandler<CreateTransactionCommand, Result>
    {
        private readonly IAuthService _authService;
        private readonly ICacheService _cacheService;
        private readonly IAppDbContext _context;

        public CreateTransactionCommandHandler(IAuthService authService, ICacheService cacheService, IAppDbContext context)
        {
            _authService = authService;
            _cacheService = cacheService;
            _context = context;
        }

        public async Task<Result> Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
        {
            var reference = $"SatolshiDice_{DateTime.Now.Ticks}";
            try
            {
                var user = await _authService.GetUserById(request.UserId);
                if(user.user == null)
                {
                    return Result.Failure("Transaction creation failed. Invalid user details");
                }
                var entity = new Transaction
                {
                    UserId = request.UserId,
                    Amount = request.Amount,
                    TransactionType = request.TransactionType,
                    TransactionReference = reference,
                    TransactionStatus = TransactionStatus.Success
                };
                await _context.Transactions.AddAsync(entity);
                await _context.SaveChangesAsync(cancellationToken);

                _cacheService.SetData<Transaction>("transaction", entity, entity.TxnId.ToString());
                return Result.Success("Transaction creation was successful", entity);
            }
            catch (Exception ex)
            {
                return Result.Failure(new string[] { "Transaction creation was not successful", ex?.Message ?? ex?.InnerException.Message });
            }
        }
    }
}
