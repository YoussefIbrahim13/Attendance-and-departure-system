using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeesModels.Shared
{
    public class CurrentUserDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set; } = new();
        public bool IsAdmin => Roles.Contains("Admin");
        public bool IsManager => Roles.Contains("Manager");
        public bool IsUser=> Roles.Contains("User");
        // Add other properties as needed
    }
}
