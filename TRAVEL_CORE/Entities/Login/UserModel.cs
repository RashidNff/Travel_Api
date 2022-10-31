namespace TRAVEL_CORE.Entities.Login
{
    public class User
    {
        public int UserId { get; set; }
        public int UserType { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? Email { get; set; }
        public string? PhoneNo { get; set; }
        public string? Token { get; set; }
        public int ChangePassword { get; internal set; }
    }
}
