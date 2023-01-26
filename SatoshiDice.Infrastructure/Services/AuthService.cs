using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using SatoshiDice.Application.Common.Interfaces;
using SatoshiDice.Application.Interfaces;
using SatoshiDice.Domain.Entities;
using SatoshiDice.Domain.Enums;
using SatoshiDice.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SatoshiDice.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAppDbContext _context;
        private readonly IConfiguration _config;
        private readonly UserManager<ApplicationUser> _userManager;
        public AuthService(IAppDbContext context, IConfiguration config, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _config = config;
            _userManager = userManager;
        }

        public async Task<Result> ChangePasswordAsync(string email, string oldPassword, string newPassword)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if(user == null)
                {
                    return Result.Failure("User not found");
                }
                var checkOldPassword = await _userManager.CheckPasswordAsync(user, oldPassword);
                if (!checkOldPassword)
                {
                    return Result.Failure("Please enter a valid current password");
                }
                // Check that the user isnt reusing the old password as new
                var checkNewPassword = await _userManager.CheckPasswordAsync(user, newPassword);
                if (checkNewPassword)
                {
                    return Result.Failure("Please enter a new password");
                }
                var changedPassword = await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);
                if (!changedPassword.Succeeded)
                {
                    var errors = changedPassword.Errors.Select(c => c.Description);
                    return Result.Failure(errors);
                }
                return Result.Success("Password change was successful");
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<Result> CreateUserAsync(User user)
        {
            try
            {
                var existingUser = await _userManager.FindByEmailAsync(user.Email);
                if(existingUser != null)
                {
                    return Result.Failure("User with that email already exist");
                }
                var newUser = new ApplicationUser
                {
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Status = Status.Inactive,
                    NormalizedEmail = user.Email
                };
                var result = await _userManager.CreateAsync(newUser, user.Password);
                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(c => c.Description);
                    return Result.Failure(errors);
                }
                newUser.UserId = newUser.Id;
                await _userManager.UpdateAsync(newUser);
                await _context.SaveChangesAsync(new CancellationToken());
                return Result.Success("User creation was successful", newUser);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public Task<Result> EmailVerification(string email, string otp)
        {
            throw new NotImplementedException();
        }

        public async Task<Result> GenerateOtp(string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if(user == null)
                {
                    return Result.Failure("Invalid user specified");
                }
                var otp = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");
                if(otp == null)
                {
                    return Result.Failure("Unable to create OTP. Kindly contact support");
                }
                return Result.Success(otp);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<(Result result, User user)> GetUserByEmail(string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if(user == null)
                {
                    return (Result.Failure("Invalid user specified"), null);
                }
                var userDetails = new User
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = email
                };
                return (Result.Success("Email retreval was successful"), userDetails);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public Task<(Result result, User user)> GetUserById(string userid)
        {
            throw new NotImplementedException();
        }

        public Task<Result> LoginAsync(string email, string password)
        {
            throw new NotImplementedException();
        }

        public Task<Result> ResetPassword(string email, string password)
        {
            throw new NotImplementedException();
        }

        public Task<Result> UpdateUserAsync(User user)
        {
            throw new NotImplementedException();
        }

        public Task<Result> ValidateOtp(string email, string otp)
        {
            throw new NotImplementedException();
        }
    }
}
