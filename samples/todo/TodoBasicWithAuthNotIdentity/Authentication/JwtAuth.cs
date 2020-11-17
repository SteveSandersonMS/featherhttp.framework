using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Authentication
{
    public class JwtAuth
    {
        private readonly SigningCredentials _signingCredentials;

        public JwtAuth(IConfiguration configuration)
        {
            var keyText = configuration["jwt:key"] ?? throw new InvalidOperationException("No JWT key is configured");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyText));
            
            ValidationParameters = new TokenValidationParameters
            {
                IssuerSigningKey = key,
                ValidateAudience = false,
                ValidateIssuer = false,
            };

            _signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        }

        public TokenValidationParameters ValidationParameters { get; }

        public string GenerateToken(string username)
        {
            var token = new JwtSecurityToken(
                claims: new List<Claim>
                {
                    new Claim(ClaimTypes.Name, username)
                },
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: _signingCredentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
