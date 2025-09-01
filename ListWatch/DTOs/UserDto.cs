namespace ListWatch.DTOs
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = "";
        public string Email { get; set; } = "";
        public string FName { get; set; } = "";
        public string LName { get; set; } = "";
    }

    public class CreateUserDto
    {
        public string Username { get; set; } = "";
        public string Email { get; set; } = "";
        public string Passwordhash { get; set; } = "";
        public string FName { get; set; } = "";
        public string LName { get; set; } = "";
    }
}
