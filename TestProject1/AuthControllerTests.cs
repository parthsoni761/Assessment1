using ListWatch.Controllers;
using ListWatch.Data;
using ListWatch.DTOs;
using ListWatch.Models;
using ListWatch.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace ListWatch.Tests.Controllers
{
    public class AuthControllerTests
    {
        private readonly AppDbContext _db;
        private readonly Mock<IPasswordService> _mockPasswords;
        private readonly Mock<ITokenService> _mockTokens;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _db = new AppDbContext(options);
            _mockPasswords = new Mock<IPasswordService>();
            _mockTokens = new Mock<ITokenService>();
            _controller = new AuthController(_db, _mockPasswords.Object, _mockTokens.Object);
        }

        [Fact]
        public async Task Register_ShouldCreateUser_WhenValid()
        {
            // Arrange
            var dto = new RegisterDto
            {
                Username = "newuser",
                Email = "test@test.com",
                Password = "password",
                FName = "Test",
                LName = "User"
            };

            _mockPasswords
                .Setup(p => p.HashPassword(It.IsAny<User>(), dto.Password))
                .Returns("hashed");

            _mockTokens
                .Setup(t => t.BuildAuthResponseAsync(It.IsAny<User>(), null))
                .ReturnsAsync(new AuthResponseDto { AccessToken = "token", RefreshToken = "refresh" });

            // Act
            var result = await _controller.Register(dto);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var auth = Assert.IsType<AuthResponseDto>(ok.Value);
            Assert.Equal("token", auth.AccessToken);
        }

        [Fact]
        public async Task Register_ShouldReturnConflict_WhenUserExists()
        {
            var existing = new User { Username = "exists", Email = "exists@test.com", PasswordHash = "hash" };
            _db.Users.Add(existing);
            await _db.SaveChangesAsync();

            var dto = new RegisterDto
            {
                Username = "exists",
                Email = "exists@test.com",
                Password = "password"
            };

            // Act
            var result = await _controller.Register(dto);

            // Assert
            var conflict = Assert.IsType<ConflictObjectResult>(result.Result);
            Assert.Equal("Username or Email already exists.", conflict.Value);
        }

        [Fact]
        public async Task Login_ShouldReturnOk_WhenValid()
        {
            var user = new User
            {
                Username = "testuser",
                Email = "test@test.com",
                PasswordHash = "hashed"
            };
            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            _mockPasswords
                .Setup(p => p.Verify(user, "password", "hashed"))
                .Returns(true);

            _mockTokens
                .Setup(t => t.BuildAuthResponseAsync(user, It.IsAny<RefreshToken>()))
                .ReturnsAsync(new AuthResponseDto { AccessToken = "token" });

            var dto = new LoginDto { UsernameOrEmail = "testuser", Password = "password" };

            // Act
            var result = await _controller.Login(dto);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var auth = Assert.IsType<AuthResponseDto>(ok.Value);
            Assert.Equal("token", auth.AccessToken);
        }

        [Fact]
        public async Task Login_ShouldReturnUnauthorized_WhenInvalid()
        {
            var dto = new LoginDto { UsernameOrEmail = "nouser", Password = "wrong" };

            // Act
            var result = await _controller.Login(dto);

            // Assert
            var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result.Result);
            Assert.Equal("Invalid credentials.", unauthorized.Value);
        }

        [Fact]
        public async Task Refresh_ShouldReturnNewAuthResponse_WhenTokenValid()
        {
            // Arrange
            var user = new User { Username = "refreshuser", Email = "refresh@test.com" };
            var oldToken = new RefreshToken
            {
                Token = "old-token",
                Expires = DateTime.UtcNow.AddMinutes(10),
                User = user
            };
            _db.Users.Add(user);
            _db.RefreshTokens.Add(oldToken);
            await _db.SaveChangesAsync();

            DateTime dummyExpiry = DateTime.UtcNow.AddDays(7);
            var newToken = new RefreshToken
            {
                Token = "new-token",
                Expires = dummyExpiry,
                Created = DateTime.UtcNow,
                UserId = user.Id,
                User = user
            };

            _mockTokens
                .Setup(t => t.CreateRefreshToken(out dummyExpiry))
                .Returns(newToken);

            _mockTokens
                .Setup(t => t.BuildAuthResponseAsync(user, newToken))
                .ReturnsAsync(new AuthResponseDto { AccessToken = "access", RefreshToken = "new-token" });

            var dto = new RefreshRequestDto { RefreshToken = "old-token" };

            // Act
            var result = await _controller.Refresh(dto);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var resp = Assert.IsType<AuthResponseDto>(ok.Value);
            Assert.Equal("new-token", resp.RefreshToken);
        }

        [Fact]
        public async Task Refresh_ShouldReturnUnauthorized_WhenTokenInvalid()
        {
            var dto = new RefreshRequestDto { RefreshToken = "bad-token" };

            // Act
            var result = await _controller.Refresh(dto);

            // Assert
            var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result.Result);
            Assert.Equal("Invalid refresh token.", unauthorized.Value);
        }

        [Fact]
        public async Task Logout_ShouldRevokeToken_WhenValid()
        {
            var rt = new RefreshToken
            {
                Token = "logout-token",
                Expires = DateTime.UtcNow.AddMinutes(10),
                User = new User { Username = "logout", Email = "logout@test.com" }
            };
            _db.RefreshTokens.Add(rt);
            await _db.SaveChangesAsync();

            var dto = new RefreshRequestDto { RefreshToken = "logout-token" };

            // Act
            var result = await _controller.Logout(dto);

            // Assert
            Assert.IsType<NoContentResult>(result);
            Assert.NotNull(rt.Revoked);
        }

        [Fact]
        public async Task Logout_ShouldReturnNotFound_WhenTokenMissing()
        {
            var dto = new RefreshRequestDto { RefreshToken = "missing" };

            // Act
            var result = await _controller.Logout(dto);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
