using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Entities;
using Microsoft.IdentityModel.Tokens;
using API.Helpers;
using Microsoft.Extensions.Options;

namespace API.Services
{
    public class TokenService
    {
        private readonly SymmetricSecurityKey _key;

        public TokenService(IOptions<ApiSettings> apiSettings)
        {
            var tokenKey = apiSettings.Value.TokenKey;
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));
        }

        public string CreateToken(AppUser user)
        {
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.UniqueName, user.Email),
                new(JwtRegisteredClaimNames.NameId, user.Id.ToString()),
                new(ClaimTypes.Role, user.UserRole)
            };

            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}