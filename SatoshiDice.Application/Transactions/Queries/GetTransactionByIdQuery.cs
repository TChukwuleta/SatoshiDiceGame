using MediatR;
using Microsoft.EntityFrameworkCore;
using SatoshiDice.Application.Common.Interfaces;
using SatoshiDice.Application.Common.Interfaces.Validators;
using SatoshiDice.Application.Interfaces;
using SatoshiDice.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SatoshiDice.Application.Transactions.Queries
{
    public class GetTransactionByIdQuery: IRequest<Result>, IIdValidator
    {
        public int Id { get; set; }
        public string UserId { get; set; }
    }

    public class GetTransactionByIdQueryHandler : IRequestHandler<GetTransactionByIdQuery, Result>
    {
        private readonly IAuthService _authService;
        private readonly IAppDbContext _context;
        public GetTransactionByIdQueryHandler(IAuthService authService, IAppDbContext context)
        {
            _authService = authService;
            _context = context;
        }

        public async Task<Result> Handle(GetTransactionByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _authService.GetUserById(request.UserId);
                if (user.user == null)
                {
                    return Result.Failure("Transaction retrieval by id was not successful. Invalid user details");
                }
                var entity = await _context.Transactions.FirstOrDefaultAsync(c => c.Id == request.Id);
                if(entity == null)
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
