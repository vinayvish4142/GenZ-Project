using EntityModel.Models;
using GenZApp.Models;
using Microsoft.AspNetCore.Identity;
using Repositorys.Authenticate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenZApp.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _users;
        private readonly ITokenService _tokens;
        private readonly IRefreshTokenRepository _repository;

        public AuthService(UserManager<ApplicationUser> users, ITokenService tokens, IRefreshTokenRepository repository)
        {
            _users = users;
            _tokens = tokens;
            _repository = repository;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
        {
            if (await _users.FindByEmailAsync(dto.Email) != null)
            {
                return new AuthResponseDto
                {
                    SuccessMsg = false,
                    Message = "Email already registered."
                };
            }

            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FullName = dto.FullName,
                   PhoneNumber = dto.Mobile
            };

            var result = await _users.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
                throw new InvalidOperationException(string.Join("; ", result.Errors.Select(x => x.Description)));
            var roleResult = await _users.AddToRoleAsync(user, "Student");
            if (!roleResult.Succeeded)
                throw new InvalidOperationException("Unable to assign default role.");
            return await IssueTokensAsync(user);
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            var user = await _users.FindByEmailAsync(dto.Email);

            if (user == null ||
                !await _users.CheckPasswordAsync(user, dto.Password))
            {
                throw new UnauthorizedAccessException("Invalid email or password.");
            }
            return await IssueTokensAsync(user);
        }

        private async Task<AuthResponseDto> IssueTokensAsync(ApplicationUser user)
        {
            var (accessToken, expiresAt) = await _tokens.CreateAccessTokenAsync(user);
            var refreshToken = _tokens.GenerateRefreshToken();
            await _repository.AddAsync(new RefreshToken
            {
                UserId = user.Id,
                TokenHash = _tokens.HashRefreshToken(
                    refreshToken),
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            });
            await _repository.SaveAsync();
            return new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = expiresAt,
                Email = user.Email ?? "",
                Roles = await _users.GetRolesAsync(user)
            };
        }

        public async Task<AuthResponseDto> RefreshAsync(string refreshToken)
        {
            var hash = _tokens.HashRefreshToken(refreshToken);
            var saved = await _repository.GetByHashAsync(hash);

            if (saved == null ||
                saved.RevokedAt != null ||
                saved.ExpiresAt <= DateTime.UtcNow)
            {
                throw new UnauthorizedAccessException("Invalid or expired refresh token.");
            }
            saved.RevokedAt = DateTime.UtcNow;
            var user = saved.User;
            var (accessToken, expiresAt) = await _tokens.CreateAccessTokenAsync(user);
            var newRefreshToken = _tokens.GenerateRefreshToken();
            await _repository.AddAsync(new RefreshToken
            {
                UserId = user.Id,
                TokenHash = _tokens.HashRefreshToken(
                    newRefreshToken),
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            });
            await _repository.SaveAsync();
            return new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = newRefreshToken,
                ExpiresAt = expiresAt,
                Email = user.Email ?? "",
                Roles = await _users.GetRolesAsync(user)
            };
        }

        public async Task LogoutAsync(string refreshToken)
        {
            var hash = _tokens.HashRefreshToken(refreshToken);
            var saved = await _repository.GetByHashAsync(hash);

            if (saved != null && saved.RevokedAt == null)
            {
                saved.RevokedAt = DateTime.UtcNow;
                await _repository.SaveAsync();
            }
        }
    }
}
