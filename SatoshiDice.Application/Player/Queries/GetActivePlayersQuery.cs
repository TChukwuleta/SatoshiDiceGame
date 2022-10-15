using MediatR;
using Microsoft.EntityFrameworkCore;
using SatoshiDice.Application.Interfaces;
using SatoshiDice.Domain.Enums;
using SatoshiDice.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SatoshiDice.Application.Player.Queries
{
    public class GetActivePlayersQuery : IRequest<Result>
    {
    }

    public class GetActivePlayersQueryHandler : IRequestHandler<GetActivePlayersQuery, Result>
    {
        private readonly IAppDbContext _context;
        public GetActivePlayersQueryHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(GetActivePlayersQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var activePlayers = await _context.Players.Where(c => c.Status == Status.Active).ToListAsync();
                if (activePlayers.Count <= 0) return Result.Failure("No active players");
                return Result.Success("Active players retrievals was successful", activePlayers);
            }
            catch (Exception ex)
            {
                return Result.Failure(new string[] { "Active players retrieval not successful", ex?.Message ?? ex?.InnerException.Message });
            }
        }
    }
}
