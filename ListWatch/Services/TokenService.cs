using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using ListWatch.DTOs;
using ListWatch.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ListWatch.Data;

namespace ListWatch.Services
{
    public class TokenService : ITokenService
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _config;

        public TokenService(AppDbContext db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        public string CreateAccessToken(User user, out DateTime expiresAtUtc)
        {
            var issuer = _config["Jwt:Issuer"]!;
            var audience = _config["Jwt:Audience"]!;
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var minutes = int.Parse(_config["Jwt:AccessTokenMinutes"] ?? "15");

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
                new Claim(JwtRegisteredClaimNames.Email, user.Email)
            };

            expiresAtUtc = DateTime.UtcNow.AddMinutes(minutes);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: expiresAtUtc,
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public RefreshToken CreateRefreshToken(out DateTime expiresAtUtc)
        {
            var days = int.Parse(_config["Jwt:RefreshTokenDays"] ?? "1");
            expiresAtUtc = DateTime.UtcNow.AddDays(days);

            var tokenBytes = RandomNumberGenerator.GetBytes(64);
            return new RefreshToken
            {
                Token = Convert.ToBase64String(tokenBytes),
                Expires = expiresAtUtc
            };
        }

        public async Task<AuthResponseDto> BuildAuthResponseAsync(User user, RefreshToken? reuseActive = null)
        {
            var access = CreateAccessToken(user, out var accessExp);

            RefreshToken refreshEntity;
            if (reuseActive != null && reuseActive.IsActive)
            {
                refreshEntity = reuseActive;
            }
            else
            {
                refreshEntity = CreateRefreshToken(out var refreshExp);
                refreshEntity.UserId = user.Id;
                _db.RefreshTokens.Add(refreshEntity);
                await _db.SaveChangesAsync();
            }

            return new AuthResponseDto
            {
                AccessToken = access,
                RefreshToken = refreshEntity.Token,
                AccessTokenExpiresAtUtc = accessExp,
                RefreshTokenExpiresAtUtc = refreshEntity.Expires
            };
        }
    }
}
