using MediatR;
using Microsoft.EntityFrameworkCore;
using SatoshiDice.Application.Common.Interfaces;
using SatoshiDice.Application.Common.Interfaces.Validators;
using SatoshiDice.Application.Interfaces;
using SatoshiDice.Domain.Entities;
using SatoshiDice.Domain.Enums;
using SatoshiDice.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SatoshiDice.Application.Transactions.Queries
{
    public class GetCreditTransactionsByUserIdQuery : IRequest<Result>, IBaseValidator
    {
        public int Skip { get; set; }
        public int Take { get; set; }
        public string UserId { get; set; }
    }

    public class GetCreditTransactionsByUserIdQueryHandler : IRequestHandler<GetCreditTransactionsByUserIdQuery, Result>
    {
        private readonly ICacheService _cacheService;
        private readonly IAuthService _authService;
        private readonly IAppDbContext _context;
        public GetCreditTransactionsByUserIdQueryHandler(ICacheService cacheService, IAuthService authService, IAppDbContext context)
        {
            _cacheService = cacheService;
            _authService = authService;
            _context = context;
        }

        public async Task<Result> Handle(GetCreditTransactionsByUserIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _authService.GetUserById(request.UserId);
                if (user.user == null)
                {
                    return Result.Failure("Credit transactions retrieval was not successful. Invalid user details");
                }
                var allTransactions = await _cacheService.GetData<Transaction>("transaction");
                if (allTransactions != null && allTransactions.Count() > 0)
                {
                    var transactions = allTransactions.Where(c => c.UserId == request.UserId && c.TransactionType == TransactionType.Credit).ToList();
                    if (transactions.Count <= 0)
                    {
                        return Result.Failure("No credit transactions found for this user");
                    }
                    return Result.Success("Credit transactions retrieval was successful", transactions.Skip(request.Skip).Take(request.Take).ToList());
                }
                var entity = await _context.Transactions.Where(c => c.UserId == request.UserId && c.TransactionType == TransactionType.Credit).ToListAsync();
                if (entity.Count <= 0)
                {
                    return Result.Failure("No credit transactions found for this user");
                }
                return Result.Success("Credit transactions retrieval was successful", entity.Skip(request.Skip).Take(request.Take).ToList());
            }
            catch (Exception ex)
            {
                return Result.Failure(new string[] { "User credit transactions retrieval was not successful", ex?.Message ?? ex?.InnerException.Message });
            }
        }
    }
}
