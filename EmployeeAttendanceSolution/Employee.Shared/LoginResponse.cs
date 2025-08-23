namespace EmployeesModels.Shared
{
    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public DateTime Expiration { get; set; }
        public object User { get; set; } = new();
    }
}
