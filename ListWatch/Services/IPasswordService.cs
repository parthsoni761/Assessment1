using ListWatch.Models;

namespace ListWatch.Services
{
    public interface IPasswordService
    {
        string HashPassword(User user, string password);
        bool Verify(User user, string password, string hash);
    }
}
