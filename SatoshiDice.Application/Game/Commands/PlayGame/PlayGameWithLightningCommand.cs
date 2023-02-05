using MediatR;
using Microsoft.Extensions.Configuration;
using SatoshiDice.Application.Common.Interfaces;
using SatoshiDice.Application.Common.Models;
using SatoshiDice.Application.Interfaces;
using SatoshiDice.Application.Transactions.Commands;
using SatoshiDice.Domain.Common.Responses;
using SatoshiDice.Domain.Enums;
using SatoshiDice.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SatoshiDice.Application.Game.Commands.PlayGame
{
    public class PlayGameWithLightningCommand : IRequest<Result>
    {
        public int FirstDice { get; set; }
        public int SecondDice { get; set; }
        public decimal Amount { get; set; }
        public string UserId { get; set; }
    }

    public class PlayGameWithLightningCommandHandler : IRequestHandler<PlayGameWithLightningCommand, Result>
    {
        private readonly IAppDbContext _context;
        private readonly IConfiguration _config;
        private readonly IBitcoinCoreClient _bitcoinCoreClient;
        private readonly ILightningClient _lightningClient;
        private readonly IAuthService _authService;
        private readonly ICacheService _cacheService;

        public PlayGameWithLightningCommandHandler(IAppDbContext context, IConfiguration config, ILightningClient lightningClient, IAuthService authService, ICacheService cacheService, IBitcoinCoreClient bitcoinCoreClient)
        {
            _context = context;
            _config = config;
            _lightningClient = lightningClient;
            _authService = authService;
            _cacheService = cacheService;
            _bitcoinCoreClient = bitcoinCoreClient;
        }

        public async Task<Result> Handle(PlayGameWithLightningCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _authService.GetUserById(request.UserId);
                if (user.user == null)
                {
                    return Result.Failure("Unable to play game. Invalid user details");
                }

                // Check the user lightning wallet balance
                var walletBalance = await _lightningClient.GetWalletBalance(UserType.User);
                if(walletBalance < request.Amount)
                {
                    return Result.Failure("Insufficient funds. Kindly topup your lightning wallet");
                }

                // Admin generates invoice and player pays the invoice before we proceed to the game
                // Admin generates invoice
                var gameInvoice = await _lightningClient.CreateInvoice((long)request.Amount, "About to pay for game", UserType.Admin);
                if (string.IsNullOrEmpty(gameInvoice))
                {
                    return Result.Failure("An error occured while generating invoice");
                }

                // User/ Player play the invoice
                var payGameInvoice = await _lightningClient.SendLightning(gameInvoice, UserType.User);
                if (!string.IsNullOrEmpty(payGameInvoice))
                {
                    return Result.Failure($"An error occured while paying for the invoice. {payGameInvoice}");
                }

                // Perform Bitcoin transaction: create raw transaction
                bool transactionSuccess = false;
                // If transaction success, update transactions and balance

                // Create transactions 
                var txnReqs = new List<TransactionRequest>();
                txnReqs.Add(new TransactionRequest
                {
                    UserId = _config["AdminUserId"], // Change to system UserId
                    FromUser = user.user.UserId,
                    ToAddress = _config["AdminUserId"],
                    Amount = request.Amount,
                    TransactionType = TransactionType.Credit
                });
                txnReqs.Add(new TransactionRequest
                {
                    UserId = user.user.UserId,
                    FromUser = user.user.UserId,
                    ToAddress = _config["AdminUserId"],
                    Amount = request.Amount,
                    TransactionType = TransactionType.Debit
                });
                var initTxnRequest = new CreateTransactionsCommand
                {
                    UserId = user.user.UserId,
                    Transactions = txnReqs

                };
                var initializeGameTransactions = await new CreateTransactionsCommandHandler(_authService, _cacheService, _context).Handle(initTxnRequest, cancellationToken);
                if (!initializeGameTransactions.Succeeded)
                {
                    return Result.Failure("An error occured while trying to process initial transaction");
                }



                // Get True Value
                var random = new Random();
                var guessValue = random.Next(1, 13);
                decimal rate = default;
                decimal winAmount = default;
                var userGuess = request.FirstDice + request.SecondDice;
                string message = string.Empty;
                var guessDiff = Math.Abs(guessValue - userGuess);
                switch (guessDiff)
                {
                    case 0:
                        rate = request.Amount * (decimal)0.05;
                        winAmount = request.Amount + rate;
                        message = "Yaaayyy.. Congratulations. You won";
                        break;
                    case 1:
                        rate = request.Amount * (decimal)0.02;
                        winAmount = request.Amount + rate;
                        message = "Nice. That was close. Just one number different. You got a reward for that at least Welldone.";
                        break;
                    case 2:
                        rate = request.Amount * (decimal)0.01;
                        winAmount = request.Amount + rate;
                        message = "You did good, but you didn't hit the mark. You got a reward for that at least Welldone.";
                        break;
                    case >= 3:
                        winAmount = 0;
                        message = "Oops. You missed it. Try again";
                        break;
                    default:
                        break;
                }

                if(winAmount > 0)
                {
                    // User generates invoice with win amount
                    var winningInvoice = await _lightningClient.CreateInvoice((long)winAmount, "Win from the game", UserType.User);
                    if (string.IsNullOrEmpty(winningInvoice))
                    {
                        return Result.Failure("An error occured while generating invoice");
                    }

                    var payWiningInvoice = await _lightningClient.SendLightning(winningInvoice, UserType.Admin);
                    if (!string.IsNullOrEmpty(payGameInvoice))
                    {
                        return Result.Failure($"An error occured while paying for the invoice. {payWiningInvoice}");
                    }

                    // Create transactions 
                    var winTxnReqs = new List<TransactionRequest>();
                    winTxnReqs.Add(new TransactionRequest
                    {
                        UserId = _config["AdminUserId"], // Change to system UserId
                        ToAddress = user.user.UserId,
                        FromUser = _config["AdminUserId"],
                        Amount = winAmount,
                        TransactionType = TransactionType.Debit
                    });
                    winTxnReqs.Add(new TransactionRequest
                    {
                        UserId = user.user.UserId,
                        ToAddress = user.user.UserId,
                        FromUser = _config["AdminUserId"],
                        Amount = winAmount,
                        TransactionType = TransactionType.Credit
                    });
                    var finalTxnRequest = new CreateTransactionsCommand
                    {
                        UserId = user.user.UserId,
                        Transactions = winTxnReqs

                    };
                    var finalizeeGameTransactions = await new CreateTransactionsCommandHandler(_authService, _cacheService, _context).Handle(finalTxnRequest, cancellationToken);
                    if (!finalizeeGameTransactions.Succeeded)
                    {
                        return Result.Failure("An error occured while trying to process initial transaction");
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

                

                var response = new
                {
                    AmountWon = winAmount,
                    DiceValue = guessValue
                };

                return Result.Success(message, response);
            }
            catch (Exception ex)
            {
                return Result.Failure(new string[] { "Play game command was not successful", ex?.Message ?? ex?.InnerException.Message });
            }
        }
    }
}
