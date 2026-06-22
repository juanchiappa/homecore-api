using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using HomeCore.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.Text;

namespace HomeCore.BLL
{
    public class TokenService
    {
        private readonly IConfiguration _configuration;
        public TokenService(IConfiguration Configuration)
        {
            _configuration = Configuration;
        }

        public (string Token, DateTime ExpiresAt) GenerateToken(User usuario)
        {
            var jwtKey = _configuration["Jwt:Key"]
                ?? throw new InvalidOperationException("Jwt:Key no está configurada.");

            var expirationMinutes = _configuration.GetValue<int>("Jwt:ExpirationMinutes", 60);
            var expiresAt = DateTime.UtcNow.AddMinutes(expirationMinutes);

            var claims = new[]
            {
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new Claim(ClaimTypes.Name,           usuario.UserName),
            new Claim(ClaimTypes.Role,           usuario.Role)
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: expiresAt,
                signingCredentials: credentials
            );

            return (new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
        }
    }
}
