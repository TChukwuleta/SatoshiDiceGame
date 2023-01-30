using MediatR;
using SatoshiDice.Application.Common.Interfaces;
using SatoshiDice.Application.Common.Interfaces.Validators.UserValidator;
using SatoshiDice.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SatoshiDice.Application.Users.Commands
{
    public class UserLoginCommand : IRequest<Result>, IUserLoginValidator
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class UserLoginCommandHandler : IRequestHandler<UserLoginCommand, Result>
    {
        private readonly IAuthService _authService;
        public UserLoginCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<Result> Handle(UserLoginCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _authService.LoginAsync(request.Email, request.Password);
                return Result.Success(result);
            }
            catch (Exception ex)
            {
                return Result.Failure(new string[] { "User login was not successful", ex?.Message ?? ex?.InnerException.Message });
            }
        }
    }
}
