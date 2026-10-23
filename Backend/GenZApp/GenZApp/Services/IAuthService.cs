using GenZApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenZApp.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterDto dto);

        Task<AuthResponseDto> LoginAsync(LoginDto dto);

        Task<AuthResponseDto> RefreshAsync(string refreshToken);

        Task LogoutAsync(string refreshToken);
    }
}
