using MediatR;
using Microsoft.EntityFrameworkCore;
using SatoshiDice.Application.Common.Interfaces;
using SatoshiDice.Application.Common.Interfaces.Validators;
using SatoshiDice.Application.Interfaces;
using SatoshiDice.Domain.Entities;
using SatoshiDice.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SatoshiDice.Application.Transactions.Queries
{
    public class GetAllTransactionsQuery : IRequest<Result>, IBaseValidator
    {
        public int Skip { get; set; }
        public int Take { get; set; }
        public string UserId { get; set; }
    }

    public class GetAllTransactionsQueryHandler : IRequestHandler<GetAllTransactionsQuery, Result>
    {
        private readonly IAuthService _authService;
        private readonly ICacheService _cacheService;
        private readonly IAppDbContext _context;

        public GetAllTransactionsQueryHandler(IAuthService authService, ICacheService cacheService, IAppDbContext context)
        {
            _authService = authService;
            _cacheService = cacheService;
            _context = context;
        }

        public async Task<Result> Handle(GetAllTransactionsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _authService.GetUserById(request.UserId);
                if(user.user == null)
                {
                    return Result.Failure("Transactions retrieval was not successful. Invalid user details");
                }
                var allTransactions = await _cacheService.GetData<Transaction>("transaction");
                if(allTransactions != null && allTransactions.Count() > 0)
                {
                    var transactions = allTransactions.Where(c => c.UserId == request.UserId).ToList();
                    if(transactions.Count <= 0)
                    {
                        return Result.Failure("No transactions found for this user");
                    }
                    return Result.Success("Transactions retrieval was successful", transactions.Skip(request.Skip).Take(request.Take).ToList());
                }
                var entity = await _context.Transactions.Where(c => c.UserId == request.UserId).ToListAsync();
                if(entity.Count <= 0)
                {
                    return Result.Failure("No transactions found for this user");
                }
                return Result.Success("Transactions retrieval was successful", entity.Skip(request.Skip).Take(request.Take).ToList());
            }
            catch (Exception ex)
            {
                return Result.Failure(new string[] { "User transactions retrieval was not successful", ex?.Message ?? ex?.InnerException.Message });
            }
        }
    }
}
