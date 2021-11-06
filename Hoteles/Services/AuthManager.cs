using Hoteles.Data.DTOs;
using Hoteles.Data.Users;
using Hoteles.Services.contracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Hoteles.Services
{
    public class AuthManager : IAuthManager
    {
        private readonly UserManager<ApiUser> _userManager;
        private readonly IConfiguration _configuration;
        private ApiUser _user;
        public AuthManager(UserManager<ApiUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task<bool> ValidateUser(LoginUserDTO loginUserDto)
        {
            _user = await _userManager.FindByNameAsync(loginUserDto.Email);
            var validPassword = await _userManager.CheckPasswordAsync(_user, loginUserDto.Password);
            return (_user != null && validPassword);
        }
        

        public async Task<string> CreateToken()
        {
            var signingCredentials = GetSigningCredentials();
            var claims = await GetClaims();
            var token = GenerateTokenOptions(signingCredentials, claims);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims)
        {
            var expiration = DateTime.Now.AddMinutes(Convert.ToDouble(
                _configuration["Jwt:Time"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                claims: claims,
                expires: expiration,
                signingCredentials: signingCredentials
            );

            return token;
        }

        private async Task<List<Claim>> GetClaims()
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, _user.UserName)
            };

            var roles = await _userManager.GetRolesAsync(_user);

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            return claims;
        }

        private SigningCredentials GetSigningCredentials()
        {        
            var secret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
        }
    }
}

