using GenZApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace GenZApp.Services
{
    public class JwtTokenService : ITokenService
    {
        private readonly IConfiguration _config;
        private readonly UserManager<ApplicationUser> _users;

        public JwtTokenService(IConfiguration config, UserManager<ApplicationUser> users)
        {
            _config = config;
            _users = users;
        }

        public async Task<(string Token, DateTime ExpiresAt)> CreateAccessTokenAsync(ApplicationUser user)
        {
            var roles = await _users.GetRolesAsync(user);
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id),
                new(JwtRegisteredClaimNames.Email,user.Email ?? ""),
                new(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddMinutes(15);
            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: credentials
            );
            return (new JwtSecurityTokenHandler().WriteToken(token),expires);
        }

        public string GenerateRefreshToken()
        {
            byte[] randomBytes = new byte[64];

            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }

            return Convert.ToBase64String(randomBytes);
        }

        public string HashRefreshToken(string token)
        {
            return Convert.ToHexString(
                SHA256.HashData(
                    Encoding.UTF8.GetBytes(token)));
        }
    }
}
