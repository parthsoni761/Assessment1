using ListWatch.Data;
using ListWatch.DTOs;
using ListWatch.Models;
using ListWatch.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ListWatch.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IPasswordService _passwords;
        private readonly ITokenService _tokens;

        public AuthController(AppDbContext db, IPasswordService passwords, ITokenService tokens)
        {
            _db = db;
            _passwords = passwords;
            _tokens = tokens;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterDto dto)
        {
            if (await _db.Users.AnyAsync(u => u.Username == dto.Username || u.Email == dto.Email))
                return Conflict("Username or Email already exists.");

            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                FName = dto.FName,
                LName = dto.LName
            };
            user.PasswordHash = _passwords.HashPassword(user, dto.Password);

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            var auth = await _tokens.BuildAuthResponseAsync(user);
            return Ok(auth);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto dto)
        {
            var user = await _db.Users
                .Include(u => u.RefreshTokens)
                .FirstOrDefaultAsync(u =>
                    u.Username == dto.UsernameOrEmail || u.Email == dto.UsernameOrEmail);

            if (user == null) return Unauthorized("Invalid credentials.");

            if (!_passwords.Verify(user, dto.Password, user.PasswordHash))
                return Unauthorized("Invalid credentials.pass");

            // Reuse the newest active refresh token if you prefer (optional)
            var active = user.RefreshTokens.LastOrDefault(rt => rt.IsActive);

            var auth = await _tokens.BuildAuthResponseAsync(user, active);
            return Ok(auth);
        }

        [HttpPost("refresh")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResponseDto>> Refresh([FromBody] RefreshRequestDto dto)
        {
            var rt = await _db.RefreshTokens
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Token == dto.RefreshToken);

            if (rt == null || !rt.IsActive) return Unauthorized("Invalid refresh token.");

            // Rotate refresh token (best practice)
            rt.Revoked = DateTime.UtcNow;
            var newRt = _tokens.CreateRefreshToken(out _);
            newRt.UserId = rt.UserId;
            rt.ReplacedByToken = newRt.Token;

            _db.RefreshTokens.Add(newRt);
            await _db.SaveChangesAsync();

            var resp = await _tokens.BuildAuthResponseAsync(rt.User, newRt);
            return Ok(resp);
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout([FromBody] RefreshRequestDto dto)
        {
            var rt = await _db.RefreshTokens.FirstOrDefaultAsync(r => r.Token == dto.RefreshToken);
            if (rt == null) return NotFound();

            rt.Revoked = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
