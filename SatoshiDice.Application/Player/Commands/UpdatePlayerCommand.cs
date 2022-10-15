using MediatR;
using Microsoft.EntityFrameworkCore;
using SatoshiDice.Application.Interfaces;
using SatoshiDice.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SatoshiDice.Application.Player.Commands
{
    public class UpdatePlayerCommand : IRequest<Result>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string UserId { get; set; }
    }
    public class UpdatePlayerCommandHandler : IRequestHandler<UpdatePlayerCommand, Result>
    {
        private readonly IAppDbContext _context;
        public UpdatePlayerCommandHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(UpdatePlayerCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var player = await _context.Players.FirstOrDefaultAsync(c => c.UserId == request.UserId);
                if (player == null) return Result.Failure("Invalid user selected");
                player.FirstName = request.FirstName;
                player.LastName = request.LastName;
                player.UserName = request.UserName;
                player.LastModifiedDate = DateTime.Now;
                _context.Players.Update(player);
                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success("Player was updated successfully");
            }
            catch (Exception ex)
            {
                return Result.Failure(new string[] { "User update was not successful", ex?.Message ?? ex?.InnerException.Message });
            }
        }
    }
}
