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
    public class GetTransactionsByTxnIdQuery : IRequest<Result>, IBaseValidator
    {
        public string TxnId { get; set; }
        public string UserId { get; set; }
    }

    public class GetTransactionsByTxnIdQueryHandler : IRequestHandler<GetTransactionsByTxnIdQuery, Result>
    {
        private readonly IAppDbContext _context;
        private readonly IAuthService _authService;
        private readonly ICacheService _cacheService;

        public GetTransactionsByTxnIdQueryHandler(IAppDbContext context, IAuthService authService, ICacheService cacheService)
        {
            _context = context;
            _authService = authService;
            _cacheService = cacheService;
        }

        public async Task<Result> Handle(GetTransactionsByTxnIdQuery request, CancellationToken cancellationToken)
        {
            Guid TxnId = Guid.Empty;
            try
            {
                var user = await _authService.GetUserById(request.UserId);
                if (user.user == null)
                {
                    return Result.Failure("Transaction retrieval by id was not successful. Invalid user details");
                }
                var transaction = await _cacheService.GetDataById<Transaction>("transaction", request.TxnId);
                if(transaction != null)
                {
                    return Result.Success("Transaction retrieval was successful", transaction);
                }
                TxnId = new Guid(request.TxnId);
                var entity = await _context.Transactions.FirstOrDefaultAsync(c => c.TxnId == TxnId);
                if (entity == null)
                {
                    return Result.Failure("Transaction retrieval by id was not successful. Invalid transaction id specified");
                }
                return Result.Success("Transaction retrieval was successful", entity);
            }
            catch (Exception ex)
            {
                return Result.Failure(new string[] { "Transaction retrieval by id was not successful", ex?.Message ?? ex?.InnerException.Message });
            }
        }
    }
}
