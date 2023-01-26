using MediatR;
using Microsoft.Extensions.Configuration;
using SatoshiDice.Application.Common.Interfaces.Validators.UserValidator;
using SatoshiDice.Application.Interfaces;
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
        private readonly IConfiguration _config;
        public CreateUserCommandHandler(IAppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public Task<Result> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
