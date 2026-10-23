using GenZApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenZApp.Services
{
    public interface ITokenService
    {
        Task<(string Token, DateTime ExpiresAt)>
       CreateAccessTokenAsync(ApplicationUser user);

        string GenerateRefreshToken();

        string HashRefreshToken(string token);
    }
}
