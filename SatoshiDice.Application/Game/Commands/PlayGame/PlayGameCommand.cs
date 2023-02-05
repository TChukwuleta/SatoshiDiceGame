using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SatoshiDice.Application.Common.Interfaces;
using SatoshiDice.Application.Game.Commands.PlayGame;
using SatoshiDice.Application.Interfaces;
using SatoshiDice.Domain.Enums;
using SatoshiDice.Domain.Model;

namespace SatoshiDice.Application.Game.Commands
{
    public class PlayGameCommand : IRequest<Result>
    {
        public PaymentModeType PaymentModeType { get; set; }
        public decimal Amount { get; set; }
        public int FirstDice { get; set; }
        public int SecondDice { get; set; }
        public string UserId { get; set; }
    }

    public class PlayGameCommandHandler : IRequestHandler<PlayGameCommand, Result>
    {
        private readonly IAppDbContext _context;
        private readonly IConfiguration _config;
        private readonly IBitcoinCoreClient _bitcoinCoreClient;
        private readonly IAuthService _authService;
        private readonly ICacheService _cacheService;
        private readonly ILightningClient _lightningClient;

        public PlayGameCommandHandler(IAppDbContext context, IConfiguration config, IBitcoinCoreClient bitcoinCoreClient, 
            IAuthService authService, ICacheService cacheService, ILightningClient lightningClient)
        {
            _context = context;
            _config = config;
            _authService = authService;
            _cacheService = cacheService;
            _bitcoinCoreClient = bitcoinCoreClient;
            _lightningClient = lightningClient;
        }
        public async Task<Result> Handle(PlayGameCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _authService.GetUserById(request.UserId);
                if(user.user == null)
                {
                    return Result.Failure("Unable to play game. Invalid user details");
                }
                if (request.FirstDice > 6) return Result.Failure("Dice value selected greater than the dice limit. Please guess a number between 1 and 6");
                if (request.SecondDice > 6) return Result.Failure("Dice value selected greater than the dice limit. Please guess a number between 1 and 6");
                if (request.FirstDice < 1) return Result.Failure("Dice value selected less than the dice limit. Please guess a number between 1 and 6");
                if (request.SecondDice < 1) return Result.Failure("Dice value selected less than the dice limit. Please guess a number between 1 and 6");
                if (request.FirstDice > 6 && request.SecondDice < 6) return Result.Failure("Both guess values are out of range");

                switch (request.PaymentModeType)
                {
                    case PaymentModeType.Bitcoin:
                        var bitcoinGameRequest = new PlayGameWithBitcoinCommand
                        {
                            FirstDice = request.FirstDice,
                            SecondDice = request.SecondDice,
                            Amount = request.Amount,
                            UserId = request.UserId
                        };
                        var bitcoinGame = await new PlayGameWithBitcoinCommandHandler(_context, _config, _bitcoinCoreClient, _authService, _cacheService).Handle(bitcoinGameRequest, cancellationToken);
                        var bitcoinGameMessage = bitcoinGame.Message != null ? bitcoinGame.Message : bitcoinGame.Messages.FirstOrDefault();
                        if (!bitcoinGame.Succeeded)
                        {
                            return Result.Failure($"An error occured while trying to play game. {bitcoinGameMessage}");
                        }
                        return Result.Success(bitcoinGameMessage);
                    case PaymentModeType.Lightning:
                        var lightningGameRequest = new PlayGameWithLightningCommand
                        {
                            FirstDice = request.FirstDice,
                            SecondDice = request.SecondDice,
                            Amount = request.Amount,
                            UserId = request.UserId
                        };
                        var lightningGame = await new PlayGameWithLightningCommandHandler(_context, _config,_lightningClient, _authService, _cacheService, _bitcoinCoreClient).Handle(lightningGameRequest, cancellationToken);
                        var lightningGameMessage = lightningGame.Message != null ? lightningGame.Message : lightningGame.Messages.FirstOrDefault();
                        if (!lightningGame.Succeeded)
                        {
                            return Result.Failure($"An error occured while trying to play game. {lightningGameMessage}");
                        }
                        return Result.Success(lightningGameMessage);
                        break;
                    case PaymentModeType.Fiat:
                        return Result.Success("We don't do that here");
                        break;
                    default:
                        return Result.Failure("Please indicate a valid payment mode");
                }
                return Result.Success("Game played successfully");
            }
            catch (Exception ex)
            {
                return Result.Failure(new string[] { "Play game command was not successful", ex?.Message ?? ex?.InnerException.Message });
            }
        }
    }
}
