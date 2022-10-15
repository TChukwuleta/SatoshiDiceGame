using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SatoshiDice.Application.Interfaces;
using SatoshiDice.Domain.Entities;
using SatoshiDice.Domain.Enums;
using SatoshiDice.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SatoshiDice.Application.PlayGame.Commands
{
    public class PlayGameCommand : IRequest<Result>
    {
        public decimal Amount { get; set; }
        public int MaximimNumberGuess { get; set; }
        public int MinimumNumberGuess { get; set; }
        public bool GameInsured { get; set; }
        public string UserId { get; set; }
    }

    public class PlayGameCommandHandler : IRequestHandler<PlayGameCommand, Result>
    {
        private readonly IAppDbContext _context;
        private readonly IConfiguration _config;
        public PlayGameCommandHandler(IAppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public async Task<Result> Handle(PlayGameCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var uniqueId = $"TobeDice_{DateTime.Now.Ticks}";
                var transactionRequests = new List<Transaction>();
                var player = await _context.Players.FirstOrDefaultAsync(x => x.UserId == request.UserId && x.Status == Status.Active);
                if (player == null) return Result.Failure("Invalid active player");
                var minimumDiceNumber = _config["MinimumDiceNumber"];
                var maximumDiceNumber = _config["MaximumDiceNumber"];
                if(request.MaximimNumberGuess > int.Parse(maximumDiceNumber)) return Result.Failure("Maximum number guess greater than guess limit. Please guess a number between 1 and 100");
                if (request.MinimumNumberGuess < int.Parse(minimumDiceNumber)) return Result.Failure("Minimum number guess less than guess limit. Please guess a number between 1 and 100");
                if (request.MaximimNumberGuess > int.Parse(maximumDiceNumber) && request.MinimumNumberGuess < int.Parse(minimumDiceNumber)) return Result.Failure("Both guess numbers out of range");
                if (player.Balance < request.Amount) return Result.Failure("Insufficient balance. Kindly top up your account");
                player.Balance -= request.Amount;
                _context.Players.Update(player);
                transactionRequests.Add(new Transaction
                {
                    UserId = request.UserId,
                    CreatedDate = DateTime.Now,
                    Amount = request.Amount,
                    TransactionReference = uniqueId,
                    TransactionType = TransactionType.Debit,
                    TransactionStatus = TransactionStatus.Success
                });
                transactionRequests.Add(new Transaction
                {
                    UserId = "gameowner userid",
                    CreatedDate = DateTime.Now,
                    Amount = request.Amount,
                    TransactionReference = uniqueId,
                    TransactionType = TransactionType.Credit
                    TransactionStatus = TransactionStatus.Success
                });
                await _context.Transactions.AddRangeAsync(transactionRequests);
                await _context.SaveChangesAsync(cancellationToken);

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
