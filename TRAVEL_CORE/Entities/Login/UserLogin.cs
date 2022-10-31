namespace TRAVEL_CORE.Entities.Login
{
    public class UserLogin
    {
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public bool RefreshToken { get; set; } = false;
    }
}
