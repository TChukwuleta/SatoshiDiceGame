using MediatR;
using Microsoft.EntityFrameworkCore;
using SatoshiDice.Application.Interfaces;
using SatoshiDice.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SatoshiDice.Application.Player.Queries
{
    public class GetPlayerByIdQuery : IRequest<Result>
    {
        public string UserId { get; set; }
    }

    public class GetPlayerByIdQueryHandler : IRequestHandler<GetPlayerByIdQuery, Result>
    {
        private readonly IAppDbContext _context;
        public GetPlayerByIdQueryHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(GetPlayerByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var player = await _context.Players.FirstOrDefaultAsync(c => c.UserId == request.UserId);
                if (player == null) return Result.Failure("No player found");
                return Result.Success("Player retrieval was successful", player);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
