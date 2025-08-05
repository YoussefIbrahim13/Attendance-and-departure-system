namespace AttendanceSystem.Auth.API.Models
{
    public class UserResponseDto
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Department { get; set; }
        public string Position { get; set; }
        public bool IsApproved { get; set; }
        public IEnumerable<string> Roles { get; set; }
    }
}
