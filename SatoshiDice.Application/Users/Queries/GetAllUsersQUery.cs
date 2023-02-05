using MediatR;
using SatoshiDice.Application.Common.Interfaces;
using SatoshiDice.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SatoshiDice.Application.Users.Queries
{
    public class GetAllUsersQUery : IRequest<Result>
    {
        public int Skip { get; set; }
        public int Take { get; set; }
        public string Email { get; set; }
    }
    public class GetAllUsersQUeryHandler : IRequestHandler<GetAllUsersQUery, Result>
    {
        private readonly IAuthService _authService;
        public GetAllUsersQUeryHandler(IAuthService authService)
        {
            _authService = authService;
        }
        public async Task<Result> Handle(GetAllUsersQUery request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _authService.GetAllUsers(request.Skip, request.Take);
                if (result.users == null)
                {
                    return Result.Failure("No users found");
                }
                return Result.Success(result.users);
            }
            catch (Exception ex)
            {
                return Result.Failure(new string[] { "Getting all system users was not successful", ex?.Message ?? ex?.InnerException.Message });
            }
        }
    }
}
