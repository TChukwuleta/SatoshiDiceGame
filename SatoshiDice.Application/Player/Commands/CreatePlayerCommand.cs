using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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
    public class CreatePlayerCommand : IRequest<Result>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
    }

    public class CreatePlayerCommandHandler : IRequestHandler<CreatePlayerCommand, Result>
    {
        private readonly IAppDbContext _context;
        private readonly IConfiguration _config;
        public CreatePlayerCommandHandler(IAppDbContext context, IConfiguration config)
        {
            _config = config;
            _context = context;
        }
        public async Task<Result> Handle(CreatePlayerCommand request, CancellationToken cancellationToken)
        {
            var password = _config["GeneralPlayersPassword"];
            try
            {
                var player = await _context.Players.FirstOrDefaultAsync(c => c.Email == request.Email);
                if (player != null) return Result.Failure("Player already exists with that email");
                var newPlayer = new Domain.Entities.Player
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Email = request.Email,
                    Password =  password,
                    UserName = request.UserName,
                    Status = Status.Active,
                    UserId = Guid.NewGuid().ToString()
                };

                await _context.Players.AddAsync(newPlayer);
                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success("User creation was successful", newPlayer);
            }
            catch (Exception ex)
            {
                return Result.Failure(new string[] { "User creation was not successful", ex?.Message ?? ex?.InnerException.Message });
            }
        }
    }
}
