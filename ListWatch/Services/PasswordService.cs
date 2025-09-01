using ListWatch.Models;
using Microsoft.AspNetCore.Identity;

namespace ListWatch.Services
{
    public class PasswordService : IPasswordService
    {
        private readonly PasswordHasher<User> _hasher = new();

        public string HashPassword(User user, string password) =>
            _hasher.HashPassword(user, password);

        public bool Verify(User user, string password, string hash) =>
            _hasher.VerifyHashedPassword(user, hash, password) != PasswordVerificationResult.Failed;
    }
}
