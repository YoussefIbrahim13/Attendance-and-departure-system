using Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace Domain.Entities
{


    public class ApplicationRole : IdentityRole
    {
        public Roles RoleType { get; set; } = Roles.User; // Default role is User
    }
}

