using Microsoft.AspNetCore.Identity;

namespace EmployeesModels.Shared

{
    public class ApplicationRole : IdentityRole
    {
        public Roles RoleType { get; set; }= Roles.User; // Default role is User
    }
}
