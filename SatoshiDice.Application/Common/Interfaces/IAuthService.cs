using SatoshiDice.Domain.Entities;
using SatoshiDice.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SatoshiDice.Application.Common.Interfaces
{
    public interface IAuthService
    {
        Task<Result> CreateUserAsync(User user);
        Task<Result> LoginAsync(string email, string password);
        Task<Result> EmailVerification(string email, string otp);
        Task<Result> ChangePasswordAsync(string email, string oldPassword, string newPassword);
        Task<Result> ResetPassword(string email, string password);
        Task<Result> GenerateOtp(string email);
        Task<Result> ValidateOtp(string email, string otp);
        Task<Result> UpdateUserAsync(User user);
        Task<(Result result, User user)> GetUserByEmail(string email);
        Task<(Result result, User user)> GetUserById(string userid);
        Task<(Result result, List<User> users)> GetAllUsers(int skip, int take);
    }
}
