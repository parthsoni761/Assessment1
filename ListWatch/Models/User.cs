namespace ListWatch.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string FName { get; set; }
        public string LName { get; set; }

        public ICollection<WatchListItems> WatchListItems { get; set; } = new List<WatchListItems>();

        public List<RefreshToken> RefreshTokens { get; set; } = new();
    }
}
