using System.ComponentModel.DataAnnotations;

namespace AttendanceSystem.Auth.API.Models
{
    public class UserCreateDto
    {
        [Required] public string Email { get; set; }
        [Required] public string Password { get; set; }
        [Required] public string Name { get; set; }
        [Required] public string Department { get; set; }
        [Required] public string Position { get; set; }
    }
}
