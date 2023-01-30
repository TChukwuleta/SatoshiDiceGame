using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SatoshiDice.Application.Common.Interfaces;
using SatoshiDice.Application.Interfaces;
using SatoshiDice.Domain.Entities;
using SatoshiDice.Domain.Enums;
using SatoshiDice.Domain.Model;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
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
                return Result.Success(newUser);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<Result> EmailVerification(string email, string otp)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    return Result.Failure("Invalid user specified");
                }
                var otpValidation = await ValidateOtp(email, otp);
                if (!otpValidation.Succeeded)
                {
                    return Result.Failure(otpValidation.Message);
                }
                user.EmailConfirmed = true;
                user.Status = Status.Active;
                var updatedResponse = await _userManager.UpdateAsync(user);
                if (!updatedResponse.Succeeded)
                {
                    return Result.Failure("An error occured while verifying email. Please contact support");
                }
                return Result.Success("Email verification was successful");
            }
            catch (Exception ex)
            {

                throw ex;
            }
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
                var otp = await _userManager.GenerateTwoFactorTokenAsync(user, email);
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
                    Email = email,
                    UserId = user.UserId
                };
                return (Result.Success("User details retreval was successful"), userDetails);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<(Result result, User user)> GetUserById(string userid)
        {
            try
            {
                var exisingUser = await _userManager.FindByIdAsync(userid);
                if(exisingUser == null)
                {
                    return (Result.Failure("Invalid user specified"), null);
                }
                var user = new User
                {
                    FirstName = exisingUser.FirstName,
                    LastName = exisingUser.LastName,
                    Email = exisingUser.Email,
                    UserId = userid
                };
                return (Result.Success("User details retreval was successful"), user);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<Result> LoginAsync(string email, string password)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if(user == null)
                {
                    return Result.Failure("Invalid email or password specified");
                }
                var checkPassword = await _userManager.CheckPasswordAsync(user, password);
                if (!checkPassword)
                {
                    return Result.Failure("Invalid email or password specified");
                }
                var jwtToken = GenerateJwtToken(user.UserId, user.Email);
                return Result.Success(jwtToken);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<Result> ResetPassword(string email, string password)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if(user == null)
                {
                    return Result.Failure("Invalid user specified");
                }
                var resetPasswordToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                if(resetPasswordToken == null)
                {
                    return Result.Failure("An error occured. Please contact support");
                }
                var resetPassword = await _userManager.ResetPasswordAsync(user, resetPasswordToken, password);
                if (!resetPassword.Succeeded)
                {
                    return Result.Failure("Password reset was not successful. Kindly contact support");
                }
                return Result.Success("Password reset was successful");
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<Result> UpdateUserAsync(User user)
        {
            try
            {
                var existingUser = await _userManager.FindByEmailAsync(user.Email);
                if (existingUser == null)
                {
                    return Result.Failure("Invalid user specified");
                }
                existingUser.FirstName = user.FirstName;
                existingUser.LastName = user.LastName;
                var updatedResponse = await _userManager.UpdateAsync(existingUser);
                if (!updatedResponse.Succeeded)
                {
                    return Result.Failure("An error occured while updating user details. Please contact support");
                }
                return Result.Success("User update was successful");
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<Result> ValidateOtp(string email, string otp)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if(user == null)
                {
                    return Result.Failure("Invalid user specified");
                }
                var validate = await _userManager.VerifyTwoFactorTokenAsync(user, email, otp);
                if (!validate)
                {
                    return Result.Failure("Error while validating OTP");
                }
                return Result.Success("OTP validated successfully");
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }



        private string GenerateJwtToken(string UserId, string email)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config["TokenConstants:key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(JwtRegisteredClaimNames.Email, email),
                    new Claim("userId", UserId),
                    new Claim(JwtRegisteredClaimNames.Sub, email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(6),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = jwtTokenHandler.WriteToken(token);
            return jwtToken;
        }
    }
}
