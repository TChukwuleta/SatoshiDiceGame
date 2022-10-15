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

namespace SatoshiDice.Application.Player.Commands
{
    public class ChangePlayerStatusCommand : IRequest<Result>
    {
        public string UserId { get; set; }
    }

    public class ChangePlayerStatusCommandHandler : IRequestHandler<ChangePlayerStatusCommand, Result>
    {
        private readonly IAppDbContext _context;
        public ChangePlayerStatusCommandHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(ChangePlayerStatusCommand request, CancellationToken cancellationToken)
        {
            try
            {
                string message = default;
                var player = await _context.Players.FirstOrDefaultAsync(c => c.UserId == request.UserId);
                if (player == null) return Result.Failure("Invalid player selectred");
                switch (player.Status)
                {
                    case Status.Active:
                        player.Status = Status.Inactive;
                        message = "Player has been deactivated successfully";
                        break;
                    case Status.Inactive:
                        player.Status = Status.Active;
                        message = "Player has been activated successfully";
                        break;
                    case Status.Deactivated:
                        player.Status = Status.Active;
                        message = "Player has been activated successfully";
                        break;
                    default:
                        break;
                }
                _context.Players.Update(player);
                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success(message);
            }
            catch (Exception ex)
            {
                return Result.Failure(new string[] { "Player status change was not successful", ex?.Message ?? ex?.InnerException.Message });
            }
        }
    }
}
