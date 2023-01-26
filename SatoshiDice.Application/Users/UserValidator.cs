using FluentValidation;
using SatoshiDice.Application.Common.Interfaces.Validators.UserValidator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SatoshiDice.Application.Users
{
    class UserValidator : AbstractValidator<IUserRequestValidator>
    {
        public UserValidator()
        {
            RuleFor(c => c.FirstName).NotNull().NotEmpty().WithMessage("First name must be specified");
            RuleFor(c => c.LastName).NotNull().NotEmpty().WithMessage("Last name must be specified");
            RuleFor(c => c.Email).NotNull().NotEmpty().WithMessage("Email must be specified");
            RuleFor(c => c.Password).NotNull().NotEmpty().WithMessage("Password must be specified");
        }
    }
}
