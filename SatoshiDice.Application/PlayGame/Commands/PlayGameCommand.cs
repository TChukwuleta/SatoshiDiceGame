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
        /*public bool GameInsured { get; set; }*/
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
                var gameOwnerUserId = _config["GameOwnerId"];
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
                    TransactionStatus = TransactionStatus.Processing,
                    Status = Status.Active
                });
                transactionRequests.Add(new Transaction
                {
                    UserId = gameOwnerUserId,
                    CreatedDate = DateTime.Now,
                    Amount = request.Amount,
                    TransactionReference = uniqueId,
                    TransactionType = TransactionType.Credit,
                    TransactionStatus = TransactionStatus.Processing,
                    Status = Status.Active
                });
                await _context.Transactions.AddRangeAsync(transactionRequests);
                await _context.SaveChangesAsync(cancellationToken);
                decimal winAmount = request.Amount;
                decimal rate = default;
                var guessDiff = request.MaximimNumberGuess - request.MinimumNumberGuess;
                switch (guessDiff)
                {
                    case 0:
                        break;
                    case int i when i > 0 && i <= 10:
                        rate = winAmount * (decimal)0.04;
                        break;
                    case int i when i > 10 && i <= 20:
                        rate = winAmount * (decimal)0.03;
                        break;
                    case int i when i > 20 && i <= 30:
                        rate = winAmount * (decimal)0.02;
                        break;
                    case int i when i > 30 && i <= 40:
                        rate = winAmount * (decimal)0.01;
                        break;
                    case int i when i > 40 && i <= 50:
                        break;
                    case int i when i > 50 && i <= 60:
                        break;
                    case int i when i > 60 && i <= 70:
                        break;
                    case int i when i > 70 && i <= 80:
                        break;
                    case int i when i > 80 && i <= 90:
                        break;
                    case int i when i > 90 && i <= 100:
                        break;
                    default:
                        break;
                }
                // Perform Bitcoin transaction
                bool transactionSuccess = false;
                // If transaction success, update transaction status from processing to completed
                if (transactionSuccess)
                {
                    var transactions = await _context.Transactions.Where(c => c.TransactionReference == uniqueId).ToListAsync();
                    if(transactions.Count > 0)
                    {
                        foreach (var item in transactions)
                        {
                            item.TransactionStatus = TransactionStatus.Success;
                            _context.Transactions.Update(item);
                        }
                        await _context.SaveChangesAsync(cancellationToken);
                    }
                }
                else
                {
                    var transactions = await _context.Transactions.Where(c => c.TransactionReference == uniqueId).ToListAsync();
                    if (transactions.Count > 0)
                    {
                        player.Balance += request.Amount;
                        foreach (var item in transactions)
                        {
                            item.TransactionStatus = TransactionStatus.Failed;
                            _context.Transactions.Update(item);
                        }
                        await _context.SaveChangesAsync(cancellationToken);
                    }
                    return Result.Failure("An error occured while performing the bitcoin transaction");
                }
                Random rand = new Random();
                int gameNumber = rand.Next(1, 100);
                if (request.MinimumNumberGuess == gameNumber || request.MaximimNumberGuess == gameNumber)
                {
                    var successfulTransaction = new List<Transaction>();
                    // Perform Bitcoin transaction
                    // If transaction success, update transaction status from processing to completed
                    if (transactionSuccess)
                    {
                        player.Balance += (winAmount + rate);
                        _context.Players.Update(player);
                        successfulTransaction.Add(new Transaction
                        {
                            UserId = request.UserId,
                            CreatedDate = DateTime.Now,
                            Amount = winAmount,
                            TransactionReference = uniqueId,
                            TransactionType = TransactionType.Credit,
                            TransactionStatus = TransactionStatus.Success
                        });
                        successfulTransaction.Add(new Transaction
                        {
                            UserId = gameOwnerUserId,
                            CreatedDate = DateTime.Now,
                            Amount = winAmount,
                            TransactionReference = uniqueId,
                            TransactionType = TransactionType.Debit,
                            TransactionStatus = TransactionStatus.Success
                        });
                        await _context.Transactions.AddRangeAsync(transactionRequests);
                        await _context.SaveChangesAsync(cancellationToken);
                    }
                    else
                    {
                        return Result.Failure("An error occured while performing the bitcoin transaction");
                    }
                    var response = new
                    {
                        AmountWon = winAmount,
                        DiceValue = gameNumber
                    };
                    return Result.Success("Yaaayyyyyy... You win", response);
                }
                if(rate > 0)
                {
                    var successfulTransaction = new List<Transaction>();
                    // Perform Bitcoin transaction
                    // If transaction success, update transaction status from processing to completed
                    if (transactionSuccess)
                    {
                        player.Balance += (winAmount + rate);
                        _context.Players.Update(player);
                        successfulTransaction.Add(new Transaction
                        {
                            UserId = request.UserId,
                            CreatedDate = DateTime.Now,
                            Amount = winAmount,
                            TransactionReference = uniqueId,
                            TransactionType = TransactionType.Credit,
                            TransactionStatus = TransactionStatus.Success
                        });
                        successfulTransaction.Add(new Transaction
                        {
                            UserId = gameOwnerUserId,
                            CreatedDate = DateTime.Now,
                            Amount = winAmount,
                            TransactionReference = uniqueId,
                            TransactionType = TransactionType.Debit,
                            TransactionStatus = TransactionStatus.Success
                        });
                        await _context.Transactions.AddRangeAsync(successfulTransaction);
                    }
                    else
                    {
                        return Result.Failure("An error occured while performing the bitcoin transaction");
                    }
                }
                /*if (request.GameInsured)
                {
                    if (request.MinimumNumberGuess - gameNumber <= 10 || gameNumber - request.MaximimNumberGuess <= 10)
                    {
                        winAmount *= (decimal)0.3;
                        player.Balance += winAmount;
                        _context.Players.Update(player);
                        transactionRequests.Add(new Transaction
                        {
                            UserId = request.UserId,
                            CreatedDate = DateTime.Now,
                            Amount = winAmount,
                            TransactionReference = uniqueId,
                            TransactionType = TransactionType.Credit,
                            TransactionStatus = TransactionStatus.Success
                        });
                        transactionRequests.Add(new Transaction
                        {
                            UserId = "gameowner userid",
                            CreatedDate = DateTime.Now,
                            Amount = winAmount,
                            TransactionReference = uniqueId,
                            TransactionType = TransactionType.Debit,
                            TransactionStatus = TransactionStatus.Success
                        });
                        await _context.Transactions.AddRangeAsync(transactionRequests);
                        await _context.SaveChangesAsync(cancellationToken);
                    }
                    var response = new
                    {
                        AmountWon = winAmount,
                        DiceValue = gameNumber
                    };
                    return Result.Success("Opps... Please play again", response);
                }*/
                await _context.Transactions.AddRangeAsync(transactionRequests);
                await _context.SaveChangesAsync(cancellationToken);
                var failedResponse = new
                {
                    AmountWon = 0,
                    DiceValue = gameNumber
                };
                return Result.Failure("Oops... You lost, please play again", failedResponse);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
