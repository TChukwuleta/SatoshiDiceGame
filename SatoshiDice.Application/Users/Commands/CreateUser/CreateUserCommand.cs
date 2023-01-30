using MediatR;
using Microsoft.Extensions.Configuration;
using SatoshiDice.Application.Common.Interfaces;
using SatoshiDice.Application.Common.Interfaces.Validators.UserValidator;
using SatoshiDice.Application.Interfaces;
using SatoshiDice.Domain.Entities;
using SatoshiDice.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SatoshiDice.Application.Users.Commands
{
    public class CreateUserCommand : IRequest<Result>, IUserRequestValidator
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result>
    {
        private readonly IAppDbContext _context;
        private readonly IAuthService _authService;
        private readonly IConfiguration _config;
        public CreateUserCommandHandler(IAppDbContext context, IConfiguration config, IAuthService authService)
        {
            _context = context;
            _authService = authService;
            _config = config;
        }

        public async Task<Result> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var existingUser = await _authService.GetUserByEmail(request.Email);
                if(existingUser.user != null)
                {
                    return Result.Failure("User already exist with that details");
                }
                var newUser = new User
                {
                    Email = request.Email,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Password = request.Password
                };
                var result = await _authService.CreateUserAsync(newUser);
                if (!result.Succeeded)
                {
                    var errorMessage = result.Message != null ? result.Message : result.Messages.FirstOrDefault();
                    return Result.Failure($"Unable to create user. {errorMessage}");
                }
                return Result.Success("User creation was successful", newUser);
            }
            catch (Exception ex)
            {
                return Result.Failure(new string[] { "User creation failed", ex?.Message ?? ex?.InnerException.Message });
            }
        }
    }
}
