using Microsoft.AspNetCore.Identity;

namespace AttendanceSystem.Auth.API.Models
{
    public class ApplicationRole : IdentityRole
    {
        public Roles RoleType { get; set; }= Roles.User; // Default role is User
    }
}
