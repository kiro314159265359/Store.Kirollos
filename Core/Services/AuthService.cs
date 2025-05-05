using Domain.Exceptions;
using Domain.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.ObjectPool;
using Services.Abstractions;
using Shared;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Shared.OrderModels;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace Services
{
    public class AuthService(
        UserManager<AppUser> _userManager,
        IOptions<JwtOptions> options,
        IMapper mapper) :
        IAuthService
    {
        public async Task<UserResultDto> LoginAsync(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user is null)
                throw new UnAuthorizedExecption();

            var flag = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            if (!flag)
                throw new UnAuthorizedExecption();

            return new UserResultDto()
            {
                DisplayName = user.DisplayName,
                Email = user.Email,
                Token = await GenerateJwtTokenAsync(user)
            };
        }

        public async Task<UserResultDto> RegisterAsync(RegisterDto registerDto)
        {
            var user = new AppUser()
            {
                DisplayName = registerDto.DisplayName,
                Email = registerDto.Email,
                UserName = registerDto.UserName,
                PhoneNumber = registerDto.PhoneNumber,
            };

            if (await CheckEmailExistsAsync(registerDto.Email))
                throw new DuplicatedEmailBadRequestExecption(registerDto.Email);

            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(error => error.Description);
                throw new ValidationExecption(errors);
            }

            return new UserResultDto()
            {
                DisplayName = user.DisplayName,
                Email = user.Email,
                Token = await GenerateJwtTokenAsync(user)
            };
        }

        public async Task<bool> CheckEmailExistsAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email) != null ? true : false;
        }

        public async Task<UserResultDto> GetCurrentUserAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
                throw new UserNotFoundExecption(email);

            return new UserResultDto()
            {
                DisplayName = user.DisplayName,
                Email = user.Email,
                Token = await GenerateJwtTokenAsync(user)
            };
        }

        public async Task<AddressDto> GetCurrentUserAddressAsync(string email)
        {
            var user = await _userManager.Users
                             .Include(u => u.Address)
                             .FirstOrDefaultAsync(u => u.Email == email);
            if (user is null)
                throw new UserNotFoundExecption(email);

            var result = mapper.Map<AddressDto>(user.Address);

            return result;
        }

        public async Task<AddressDto> UpdateCurrentUserAddressAsync(AddressDto address, string email)
        {
            var user = await _userManager.Users
                                .Include(u => u.Address)
                                .FirstOrDefaultAsync(u => u.Email == email);
            if (user is null)
                throw new UserNotFoundExecption(email);

            if (user.Address is not null)
            {
                user.Address.FirstName = address.FirstName;
                user.Address.LastName = address.LastName;
                user.Address.Street = address.Street;
                user.Address.City = address.City;
                user.Address.Country = address.Country;
            }
            else
            {
                var addressResult = mapper.Map<Address>(address);
                user.Address = addressResult;
            }
            await _userManager.UpdateAsync(user);
            return address;
        }

        private async Task<string> GenerateJwtTokenAsync(AppUser user)
        {
            // Header 
            // Payload
            // Signature

            var jwtOptions = options.Value;

            var authClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name , user.UserName),
                new Claim(ClaimTypes.Email , user.Email),
            };

            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey));

            var token = new JwtSecurityToken(
                issuer: jwtOptions.Issuer,
                audience: jwtOptions.Audience,
                claims: authClaims,
                expires: DateTime.UtcNow.AddDays(jwtOptions.DurationInDays),
                signingCredentials: new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256Signature)
            );

            // Token
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
