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
    public class GetPlayersQuery : IRequest<Result>
    {
        public int Skip { get; set; }
        public int Take { get; set; }
    }

    public class GetPlayersQueryHandler : IRequestHandler<GetPlayersQuery, Result>
    {
        private readonly IAppDbContext _context;
        public GetPlayersQueryHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(GetPlayersQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var players = await _context.Players.ToListAsync();
                if (players.Count <= 0) return Result.Failure("No players found");
                if(request.Skip == 0 && request.Take == 0) return Result.Success("Players retrieval was successful", new { Players = players, Count = players.Count() });
                return Result.Success("Players retrieval was successful", new { Players = players.Skip(request.Skip).Take(request.Take).ToList(), Count = players.Count() });
            }
            catch (Exception ex)
            {
                return Result.Failure(new string[] { "Players retrieval was not successful", ex?.Message ?? ex?.InnerException.Message });
            }
        }
    }
}
