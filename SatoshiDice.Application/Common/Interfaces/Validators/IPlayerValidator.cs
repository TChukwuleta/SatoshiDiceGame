using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SatoshiDice.Application.Common.Interfaces.Validators
{
    public interface IPlayerValidator : IBaseValidator
    {
    }

    public interface IPlayerRequestValidator
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
    }

    public interface ILoginRequestValidator
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
