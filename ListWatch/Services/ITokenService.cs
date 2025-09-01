using ListWatch.DTOs;
using ListWatch.Models;

namespace ListWatch.Services
{
    public interface ITokenService
    {
        string CreateAccessToken(User user, out DateTime expiresAtUtc);
        RefreshToken CreateRefreshToken(out DateTime expiresAtUtc);
        Task<AuthResponseDto> BuildAuthResponseAsync(User user, RefreshToken? reuseActive = null);
    }
}
