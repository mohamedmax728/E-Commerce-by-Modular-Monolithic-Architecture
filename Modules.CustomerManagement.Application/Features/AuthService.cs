using AutoMapper;
using EasyRepository.EFCore.Generic;
using EmployeeManagementSystem.Application.Dtos.User;
using EmployeeManagementSystem.Domain.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Modules.CustomerManagement.Application.Interfaces;
using Modules.CustomerManagement.Domain.Entities;
using Modules.PaymentProcessing.Domain.Helpers;
using Stripe;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Utility = Modules.CustomerManagement.Application.Helpers.Utility;

namespace Modules.CustomerManagement.Application.Features
{
    public class AuthService(IUnitOfWork unitOfWork, IConfiguration configuration, IMapper mapper, IHttpContextAccessor httpContextAccessor, IOptions<StripeSettings> stripeSettings) : IAuthService
    {
        public async Task<ServiceResponse<string>> Login(UserLoginDto loginDto)
        {
            ServiceResponse<string> response = new ServiceResponse<string>();
            var user = await unitOfWork.Users.Value.GetAsync(s => s.UserName.Equals(loginDto.Username));
            if (user == null || !VerifyPasswordHash(loginDto.Password, user.PasswordHash, user.PasswordSalt))
            {
                response.Success = false;
                response.Message = "Wrong Credintials";
            }
            else
            {
                response.Data = CreateToken(user);
            }

            return response;
        }
        public async Task<ServiceResponse<string>> Register(UserRegisterDto registerDto)
        {
            ServiceResponse<string> response = new ServiceResponse<string>();
            if (unitOfWork.Users.Value.Exists
                (s => s.UserName.ToLower().Equals(registerDto.Username.ToLower())))
            {
                response.Success = false;
                response.Message = "User already exists.";
                return response;
            }
            try
            {
                var customerIdTask = AddCustomer(registerDto);
                Utility.CreatePasswordHash(registerDto.Password, out byte[] passwordHash, out byte[] passwordSalt);
                var user = mapper.Map<User>(registerDto);
                user.PasswordSalt = passwordSalt;
                user.PasswordHash = passwordHash;
                user.CustomerId = await customerIdTask;
                ArgumentNullException.ThrowIfNull(customerIdTask.Result);
                await unitOfWork.Users.Value.AddAsync(user);
                int res = await unitOfWork.SaveChangesAsync();
                return new ServiceResponse<string>
                {
                    Data = res > 0 ? CreateToken(user) : null,
                    Success = res > 0,
                    StatusCode = res > 0 ? StatusCodes.Status200OK : StatusCodes.Status500InternalServerError,
                    Message = res > 0 ? "Register Successfully!!" : "Failure To Register!!"
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>
                {
                    Success = false,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "Failure To Register!!"
                };
            }
        }
        private async Task<string?> AddCustomer(UserRegisterDto registerDto)
        {
            try
            {
                string? customerId = null;
                StripeConfiguration.ApiKey = stripeSettings.Value.SecretKey;
                var options = new CustomerCreateOptions
                {
                    Name = registerDto.Username,
                    Phone = registerDto.PhoneNumber
                };
                var service = new CustomerService();
                var s = await service.CreateAsync(options);

                return s?.Id;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != passwordHash[i])
                    {
                        return false;
                    }
                }
                return true;
            }
        }
        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, user.IsAdmin ? "Admin" : "Customer")
            };

            SymmetricSecurityKey key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(configuration.GetSection("AppSettings:Token").Value)
            );

            SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
        public string GetCurrentUserId()
        {
            return httpContextAccessor
                .HttpContext
                .User
                .Claims
                .First(p => p.Type == ClaimTypes.NameIdentifier).Value;
        }
    }
}
